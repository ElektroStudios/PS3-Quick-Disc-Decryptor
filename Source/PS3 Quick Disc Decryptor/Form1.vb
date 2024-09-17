Option Strict On
Option Explicit On
Option Infer Off

Imports System.ComponentModel
Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.InteropServices
Imports System.Runtime.Versioning
Imports System.Text
Imports System.Threading
Imports System.Windows.Media.Animation

Imports DevCase.Extensions
Imports DevCase.Win32
Imports DevCase.Win32.Enums

Imports DiscUtils

Imports DiscUtils.Iso9660

Imports Microsoft.WindowsAPICodePack.Taskbar

Imports MS.WindowsAPICodePack.Internal

Friend NotInheritable Class Form1

#Region " Private Fields "

    Friend Shared ReadOnly Settings As New ProgramSettings()

    Private Shared logFileWriter As StreamWriter

    Private isos As IEnumerable(Of FileInfo)
    Private keys As IEnumerable(Of FileInfo)
    Private isoAndKeyPairs As IDictionary(Of FileInfo, FileInfo)

    ''' <summary>
    ''' Directory where to unzip zipped isos and decryption keys.
    ''' </summary>
    Private ReadOnly tempFolderPath As String = Path.Combine(Path.GetTempPath, My.Application.Info.Title)

    ''' <summary>
    ''' CMD process to embed its window in the UI, and where PS3Dec.exe will run.
    ''' <para></para>
    ''' This is only used when <see cref="Global.ProgramSettings.CompactMode"/> is False.
    ''' </summary>
    Private cmdProcess As Process

    ''' <summary>
    ''' PS3Dec.exe process. This is only used when <see cref="Global.ProgramSettings.CompactMode"/> is True.
    ''' </summary>
    Private ps3DecProcess As Process

    ''' <summary>
    ''' Open file handle that prevents PS3Dec.exe file from being deleted, moved or modified from disk.
    ''' </summary>
    Private ps3DecOpenHandle As FileStream

    ''' <summary>
    ''' Flag to request user cancellation of the asynchronous decryption procedure.
    ''' </summary>
    Private cancelRequested As Boolean

    ''' <summary>
    ''' Keeps track of the row height where the <see cref="Form1.TextBox_PS3Dec_Output"/> control is,
    ''' for toggling between compact and regular view mode.
    ''' </summary>
    Private lastCmdRowHeight As Integer

#End Region

#Region " Constructors "

    Public Sub New()
        ' This call is required by the designer.
        Me.InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.Opacity = 0
    End Sub

#End Region

#Region " Event-Handlers "

    Private Sub Form1_Load(sender As Object, e As EventArgs) _
    Handles MyBase.Load

        Me.PropertyGrid_Settings.SelectedObject = Form1.Settings
        Me.Text = $"{My.Application.Info.Title} (PS3QDD) v{My.Application.Info.Version} | By ElektroStudios"
    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) _
    Handles MyBase.Shown
        Me.MinimumSize = Me.Size
        Me.LoadUserSettings()
        Me.Opacity = 100

        Me.InitializeLogger()
        Me.UpdateStatus("Program has been initialized.", writeToLogFile:=True, TraceEventType.Information)
        Me.UpdateStatus("Program is ready. Press the ""Start Decryption"" button and you will see the decryption status here.", writeToLogFile:=False)
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) _
    Handles MyBase.FormClosing

        If e.CloseReason = CloseReason.UserClosing Then
            If Not Me.cmdProcess?.HasExited OrElse Not Me.ps3DecProcess?.HasExited Then
                Dim question As DialogResult =
                    MessageBox.Show(Me, $"PS3Dec.exe is currently running and writing a decrypted disc in the output directory, if you exit this program PS3Dec.exe process will be killed.{Environment.NewLine & Environment.NewLine}Do you really want to exit?.", My.Application.Info.Title,
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation)

                If question = DialogResult.Yes Then
                    Try
                        Me.cmdProcess?.Kill(entireProcessTree:=True)
                        Me.ps3DecProcess?.Kill(entireProcessTree:=False)
                        Me.UpdateStatus("PS3Dec.exe was killed on user demand (force application closure).", writeToLogFile:=True)
                    Catch ex As Exception
                    End Try
                Else
                    e.Cancel = True
                End If
            End If
        End If

        If Not e.Cancel Then
            Me.SaveUserSettings()
            Me.UpdateStatus("Program is being closed...", writeToLogFile:=True)
            Me.DeinitializeLogger()
        End If
    End Sub

    Private Sub Button_StartDecryption_Click(sender As Object, e As EventArgs) _
    Handles Button_StartDecryption.Click

        If Not Me.BackgroundWorker1.IsBusy Then
            Me.PropertyGrid_Settings.Enabled = False
            Me.Button_StartDecryption.Enabled = False
            Me.Button_Abort.Enabled = True

            Me.BackgroundWorker1.RunWorkerAsync()
        End If
    End Sub

    Private Sub Button_Abort_Click(sender As Object, e As EventArgs) _
    Handles Button_Abort.Click

        If Me.BackgroundWorker1.IsBusy Then
            Dim btn As Button = DirectCast(sender, Button)
            btn.Enabled = False
            Me.cancelRequested = True

            Me.UpdateStatus("Aborting, please wait until the current disc gets decrypted...", writeToLogFile:=False)
        End If
    End Sub

    Private Sub TextBox_PS3Dec_Output_VisibleChanged(sender As Object, e As EventArgs) _
    Handles TextBox_PS3Dec_Output.VisibleChanged

        If Me.MinimumSize = Size.Empty Then
            ' Prevents continuing if the main form was not shown once.
            Return
        End If

        Dim tb As TextBox = DirectCast(sender, TextBox)

        Dim table As TableLayoutPanel = Me.TableLayoutPanel1
        Dim rowIndex As Integer = table.GetRow(tb)
        Dim rowStyle As RowStyle = table.RowStyles(rowIndex)

        table.SuspendLayout()
        If Form1.Settings.CompactMode Then
            rowStyle.Height = 0 ' percent
            Dim rowHeight As Integer = table.GetRowHeights()(rowIndex)
            Me.MinimumSize = New Size(Me.MinimumSize.Width, Me.MinimumSize.Height - rowHeight)
            Me.Height -= rowHeight
            Me.lastCmdRowHeight = rowHeight
        Else
            rowStyle.Height = 40 ' percent
            Dim tbHeight As Integer = tb.Height
            Me.MinimumSize = New Size(Me.MinimumSize.Width, Me.MinimumSize.Height + Me.lastCmdRowHeight)
        End If
        table.ResumeLayout(performLayout:=True)
    End Sub

    Private Sub TableLayoutPanel1_Resize(sender As Object, e As EventArgs) _
    Handles TableLayoutPanel1.Resize

        If Form1.Settings.CompactMode OrElse Me.cmdProcess Is Nothing Then
            Exit Sub
        End If

        Dim hWnd As IntPtr = Me.cmdProcess.MainWindowHandle
        If hWnd = IntPtr.Zero Then
            Exit Sub
        End If

        Dim tb As TextBox = Me.TextBox_PS3Dec_Output
        If Not NativeMethods.User32.MoveWindow(hWnd, 0, 0, tb.Width, tb.Height, repaint:=True) Then
            ' Ignore.
        End If
    End Sub

    Private Sub BackgroundWorker1_DoWork(sender As Object, e As DoWorkEventArgs) _
    Handles BackgroundWorker1.DoWork

        Me.UpdateStatus($"Starting a new decryption procedure...", writeToLogFile:=True)
        Me.ResetProgressBar()

        Me.ClearTempFiles("*")

        If Not Me.FetchISOs() OrElse
           Not Me.FetchDecryptionKeys() OrElse
           Not Me.BuildisoAndKeyPairs() OrElse
           Not Me.ValidatePS3DecExe() Then

            e.Cancel = True
            Exit Sub
        End If

        Dim totalIsoCount As Integer = Me.isoAndKeyPairs.Count
        Dim currentIsoIndex As Integer = 0
        Me.UpdateProgressBar(totalIsoCount, 0)

        If Me.isoAndKeyPairs?.Any() Then
            For Each pair As KeyValuePair(Of FileInfo, FileInfo) In Me.isoAndKeyPairs
                If Me.cancelRequested Then
                    Me.BackgroundWorker1.CancelAsync()
                    e.Cancel = True
                    Exit For
                End If

                If Not Me.BackgroundWorker1.CancellationPending Then
                    Me.ProcessDecryption(pair, currentIsoIndex, totalIsoCount)
                    Me.ClearTempFiles(Path.GetFileNameWithoutExtension(pair.Key.Name))
                    Me.UpdateProgressBar(totalIsoCount, Interlocked.Increment(currentIsoIndex))
                End If
            Next
        End If
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) _
    Handles BackgroundWorker1.RunWorkerCompleted

        If e.Error IsNot Nothing Then
            If e.Error?.InnerException IsNot Nothing Then
                Form1.ShowMessageBoxInUIThread(Me, "Unknown error", e.Error.InnerException.Message, MessageBoxIcon.Error)
            Else
                Form1.ShowMessageBoxInUIThread(Me, "Unknown error", e.Error.Message, MessageBoxIcon.Error)
            End If
        End If

        If e.Cancelled AndAlso Me.cancelRequested Then
            Me.UpdateStatus("Decryption procedure aborted on demand.", writeToLogFile:=False)
            Form1.ShowMessageBoxInUIThread(Me, My.Application.Info.Title, "Decryption procedure aborted on demand.", MessageBoxIcon.Information)
        ElseIf e.Cancelled Then
            Me.UpdateStatus("Decryption procedure cancelled due an error.", writeToLogFile:=True, TraceEventType.Stop)
        Else
            Me.UpdateStatus("Decryption procedure completed.", writeToLogFile:=False)
            Form1.ShowMessageBoxInUIThread(Me, My.Application.Info.Title, "Decryption procedure completed.", MessageBoxIcon.Information)
        End If

        Try
            Me.ps3DecOpenHandle?.Close()
            Me.ps3DecOpenHandle = Nothing
        Catch ex As Exception
            Me.UpdateStatus($"Error releasing PS3Dec.exe file handle. Error message: {ex.Message}", writeToLogFile:=True, TraceEventType.Warning)
            ' Form1.ShowMessageBoxInUIThread(Me, "Error releasing PS3Dec.exe file handle", ex.Message, MessageBoxIcon.Warning)
        End Try

        Me.isos = Nothing
        Me.keys = Nothing
        Me.isoAndKeyPairs = Nothing
        Me.cmdProcess = Nothing

        Me.cancelRequested = False

        Me.PropertyGrid_Settings.Enabled = True
        Me.Button_StartDecryption.Enabled = True
        Me.Button_Abort.Enabled = False
    End Sub

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Fetches the encrypted PS3 *.iso / *.zip files.
    ''' </summary>
    ''' <returns><see langword="True"/> if successful, <see langword="False"/> otherwise.</returns>
    Private Function FetchISOs() As Boolean

        Me.UpdateStatus("Fetching encrypted PS3 disc images...", writeToLogFile:=True)
        Try
            Me.isos = Form1.Settings.EncryptedPS3DiscsDir?.
                                     GetFiles("*.*", SearchOption.TopDirectoryOnly).
                                     Where(Function(x) x.Extension.ToLowerInvariant() = ".iso" OrElse
                                                       x.Extension.ToLowerInvariant() = ".zip")

        Catch ex As Exception
            Form1.ShowMessageBoxInUIThread(Me, "Error fetching encrypted PS3 disc images", ex.Message, MessageBoxIcon.Error)
            Return False
        End Try

        If Not Me.isos.Any() Then
            Form1.ShowMessageBoxInUIThread(Me, "Error fetching encrypted PS3 disc images", $"Can't find any ISO or ZIP file in the specified directory: '{Form1.Settings.EncryptedPS3DiscsDir}'", MessageBoxIcon.Error)
            Return False
        End If

        Return True
    End Function

    ''' <summary>
    ''' Fetches the decryption key (*.dkey or *.txt) files.
    ''' </summary>
    ''' <returns><see langword="True"/> if successful, <see langword="False"/> otherwise.</returns>
    Private Function FetchDecryptionKeys() As Boolean

        Me.UpdateStatus("Fetching decryption keys...", writeToLogFile:=True)
        Try
            Me.keys = Form1.Settings.DecryptionKeysDir?.
                                     GetFiles("*.*", SearchOption.TopDirectoryOnly).
                                     Where(Function(x) x.Extension.ToLowerInvariant() = ".dkey" OrElse
                                                       x.Extension.ToLowerInvariant() = ".txt" OrElse
                                                       x.Extension.ToLowerInvariant() = ".zip")
        Catch ex As Exception
            Form1.ShowMessageBoxInUIThread(Me, "Error fetching decryption keys", ex.Message, MessageBoxIcon.Error)
            Return False
        End Try

        If Not Me.keys?.Any() Then
            Form1.ShowMessageBoxInUIThread(Me, "Error fetching decryption keys", $"Can't find any decryption key files in the specified directory: {Form1.Settings.DecryptionKeysDir}", MessageBoxIcon.Error)
            Return False
        End If

        Return True
    End Function

    ''' <summary>
    ''' Creates a <see cref="Dictionary(Of FileInfo, Fileinfo)"/> with 
    ''' <see langword="Key"/> = Encrypted ISO, and <see langword="Value"/> = Decryption key. 
    ''' </summary>
    ''' <returns><see langword="True"/> if successful, <see langword="False"/> otherwise.</returns>
    Private Function BuildisoAndKeyPairs() As Boolean

        Me.UpdateStatus("Matching encrypted PS3 ISOs with decryption keys...", writeToLogFile:=True)
        Try
            Me.isoAndKeyPairs = New Dictionary(Of FileInfo, FileInfo)
            For Each iso As FileInfo In Me.isos
                Dim match As FileInfo =
                    (From dkey As FileInfo In Me.keys
                     Where Path.GetFileNameWithoutExtension(dkey.Name).Equals(Path.GetFileNameWithoutExtension(iso.Name), StringComparison.OrdinalIgnoreCase)
                    ).SingleOrDefault()

                If match IsNot Nothing Then
                    Me.isoAndKeyPairs.Add(iso, match)
                End If
            Next iso

        Catch ex As Exception
            Form1.ShowMessageBoxInUIThread(Me, "Error matching encrypted PS3 ISOs with decryption keys", ex.Message, MessageBoxIcon.Error)
            Return False
        End Try

        Dim isosCount As Integer = Me.isos.Count
        Dim diffCount As Integer = isosCount - Me.isoAndKeyPairs.Count
        If diffCount = isosCount Then
            Form1.ShowMessageBoxInUIThread(Me, "Missing decryption key matches", $"The program could not find any matching decryption keys for the available PS3 ISOs.", MessageBoxIcon.Error)
            Return False
        ElseIf diffCount <> 0 Then
            Form1.ShowMessageBoxInUIThread(Me, "Missing decryption key matches", $"The program could not find matching decryption keys for {diffCount} out of {isosCount} PS3 ISOs.{Environment.NewLine & Environment.NewLine}The program will proceed now decrypting the remaining ISOs.", MessageBoxIcon.Warning)
        End If

        Return True
    End Function

    ''' <summary>
    ''' Validates a PS3 ISO file.
    ''' </summary>
    ''' <returns><see langword="True"/> if successful, <see langword="False"/> otherwise.</returns>
    Private Function ValidatePS3Iso(ByRef refIso As FileInfo, percentage As Integer, currentIsoIndex As Integer, totalIsoCount As Integer) As Boolean

        refIso = Me.UnzipFileIfNecessary(refIso, percentage, currentIsoIndex, totalIsoCount, "PS3 ISO")
        If refIso Is Nothing Then
            Return False
        End If

        Me.UpdateStatus($"{percentage}% ({currentIsoIndex}/{totalIsoCount}) | {refIso.Name} | Validating encrypted PS3 ISO...", writeToLogFile:=True)
        Try
            Using isoStream As FileStream = File.OpenRead(refIso.FullName),
                  cd As New CDReader(isoStream, joliet:=False)

                Dim isExpectedClusterSize As Boolean = cd.ClusterSize = 2048

                Dim existsPS3GAMEDir As Boolean =
                    cd.Root.GetDirectories("PS3_GAME", SearchOption.TopDirectoryOnly).Any()

                If Not isExpectedClusterSize OrElse
                   Not existsPS3GAMEDir Then
                    Form1.ShowMessageBoxInUIThread(Me, "Error validating encrypted PS3 ISO", $"The ISO file is not a PS3 disc image: {refIso.FullName}", MessageBoxIcon.Error)
                    Return False
                End If
            End Using

        Catch ex As Exception
            Form1.ShowMessageBoxInUIThread(Me, $"Error validating encrypted PS3 PS3 ISO.{Environment.NewLine & Environment.NewLine}File: {refIso.FullName}{Environment.NewLine & Environment.NewLine}Error message: {ex.Message}", ex.Message, MessageBoxIcon.Error)
            Return False
        End Try

        Return True
    End Function

    ''' <summary>
    ''' Validates the PS3Dec.exe file.
    ''' </summary>
    ''' <returns><see langword="True"/> if successful, <see langword="False"/> otherwise.</returns>
    Private Function ValidatePS3DecExe() As Boolean

        Me.UpdateStatus("Acquiring PS3Dec.exe file handle...", writeToLogFile:=True)
        If Not Form1.Settings.PS3DecExeFile.Exists Then
            Form1.ShowMessageBoxInUIThread(Me, "Error acquiring PS3Dec.exe file handle", $"PS3Dec.exe was not found at: {Form1.Settings.PS3DecExeFile.FullName}", MessageBoxIcon.Error)
            Return False
        End If

        Try
            Me.ps3DecOpenHandle = Form1.Settings.PS3DecExeFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read)
        Catch ex As Exception
            Form1.ShowMessageBoxInUIThread(Me, "Error acquiring PS3Dec.exe file handle", ex.Message, MessageBoxIcon.Error)
            Return False
        End Try

        Return True
    End Function

    ''' <summary>
    ''' Processes the decryption of an encrypted PS3 ISO.
    ''' </summary>
    Private Sub ProcessDecryption(pair As KeyValuePair(Of FileInfo, FileInfo), currentIsoIndex As Integer, totalIsoCount As Integer)

        Dim percentage As Integer = CInt(currentIsoIndex / totalIsoCount * 100)

        Dim dkeyString As String = Nothing
        If Not Me.ReadDecryptionKey(pair.Value, dkeyString, percentage, currentIsoIndex, totalIsoCount) Then
            Exit Sub
        End If

        Dim refIso As FileInfo = pair.Key
        Dim sizeString As String = Me.FormatFileSize(refIso.Length, StrFormatByteSizeFlags.RoundToNearest)

        If Not Me.ValidatePS3Iso(refIso, percentage, currentIsoIndex, totalIsoCount) Then
            Exit Sub
        End If

        Me.UpdateStatus($"{percentage}% ({currentIsoIndex}/{totalIsoCount}) | {refIso.Name} | Writing decrypted PS3 ISO ({sizeString}) to output directory...", writeToLogFile:=True)
        If Not Me.EnsureOutputDirectoryExists() Then
            Exit Sub
        End If

        Dim drive As DriveInfo = Form1.Settings.DecryptionKeysDir.GetDriveInfo()
        Dim requiredSizeString As String = Me.FormatFileSize(refIso.Length, StrFormatByteSizeFlags.RoundToNearest)
        If Not Me.DriveHasFreeSpace(drive, refIso.Length) Then
            Form1.ShowMessageBoxInUIThread(Me, $"Error writing decrypted PS3 ISO.", $"Drive {drive} requires {requiredSizeString} of free space to write the file.", MessageBoxIcon.Error)
            Exit Sub
        End If

        Me.ExecutePS3Dec(pair, refIso, dkeyString, currentIsoIndex, totalIsoCount)

    End Sub

    ''' <summary>
    ''' Reads and validates the decryption key from a file.
    ''' </summary>
    ''' <returns><see langword="True"/> if successful, <see langword="False"/> otherwise.</returns>
    Private Function ReadDecryptionKey(keyFile As FileInfo, ByRef refKeyString As String, percentage As Integer, currentIsoIndex As Integer, totalIsoCount As Integer) As Boolean

        keyFile = Me.UnzipFileIfNecessary(keyFile, percentage, currentIsoIndex, totalIsoCount, "Decryption key")
        If keyFile Is Nothing Then
            Return False
        End If

        Me.UpdateStatus($"{percentage}% ({currentIsoIndex}/{totalIsoCount}) | {keyFile.Name} | Parsing decryption key file content...", writeToLogFile:=True)
        Try
            Dim dkeyString As String = File.ReadAllText(keyFile.FullName, Encoding.Default).Trim()
            If dkeyString.Length <> 32 Then
                Form1.ShowMessageBoxInUIThread(Me, "Error parsing decryption key file content", $"File: {keyFile.FullName}{Environment.NewLine & Environment.NewLine}Error message: Decryption key has an invalid length of {dkeyString.Length} (it must be 32 character length).", MessageBoxIcon.Error)
                Return False
            End If

#If (NET7_0_OR_GREATER) Then
            Dim isHex As Boolean = dkeyString.All(Function(c As Char) Char.IsAsciiHexDigit(c)) AndAlso (dkeyString.Length Mod 2) = 0 ' is even.
#Else
            Dim isHexString As Boolean = StringExtensions.IsHexadecimal(dkeyString)
#End If
            If Not isHexString Then
                Form1.ShowMessageBoxInUIThread(Me, "Error parsing decryption key file content", $"File: {keyFile.FullName}{Environment.NewLine & Environment.NewLine}Error message: Decryption key has not a valid hexadecimal format.", MessageBoxIcon.Error)
                Return False
            End If

            refKeyString = dkeyString

        Catch ex As Exception
            Form1.ShowMessageBoxInUIThread(Me, "Error reading decryption key file", $"File: {keyFile.FullName}{Environment.NewLine & Environment.NewLine}Error message: {ex.Message}", MessageBoxIcon.Error)
            Return False

        End Try

        Return True
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns><see langword="True"/> if successful, <see langword="False"/> otherwise.</returns>
    Private Function EnsureOutputDirectoryExists() As Boolean

        Form1.Settings.OutputDir.Refresh()
        If Not Form1.Settings.OutputDir.Exists Then
            Try
                Form1.Settings.OutputDir.Create()
            Catch ex As Exception
                Form1.ShowMessageBoxInUIThread(Me, "Error creating output directory", ex.Message, MessageBoxIcon.Error)
                Return False
            End Try
        End If
        Return True
    End Function

    Private Sub ExecutePS3Dec(pair As KeyValuePair(Of FileInfo, FileInfo), isoFile As FileInfo, dkeyString As String, currentIsoIndex As Integer, totalIsoCount As Integer)

        Dim currentProcess As Process

        If Not Form1.Settings.CompactMode Then
            ' Embed a CMD window instance and run PS3Dec.exe inside.

            Me.cmdProcess = New Process()
            currentProcess = Me.cmdProcess
            With Me.cmdProcess
                .StartInfo.FileName = Environment.GetEnvironmentVariable("COMSPEC")
                .StartInfo.Arguments = $"/C ""(TIMEOUT /T 1 /NOBREAK)>NUL & ""{Form1.Settings.PS3DecExeFile.FullName}"" d key ""{dkeyString}"" ""{isoFile.FullName}"" ""{Form1.Settings.OutputDir.FullName}\{isoFile.Name}"""""
                .StartInfo.UseShellExecute = True
                .StartInfo.CreateNoWindow = False
                .StartInfo.WindowStyle = ProcessWindowStyle.Minimized
            End With
            Try
                Me.cmdProcess.Start()
            Catch ex As Exception
                Form1.ShowMessageBoxInUIThread(Me, "Error executing PS3Dec.exe", ex.Message, MessageBoxIcon.Error)
                Exit Sub
            End Try

            Dim hWnd As IntPtr = IntPtr.Zero
            Do Until hWnd <> IntPtr.Zero
                Thread.Sleep(100)
                hWnd = Me.cmdProcess.MainWindowHandle
                If Me.cancelRequested Then
                    Exit Sub
                End If
            Loop

            Dim tb As TextBox = Me.TextBox_PS3Dec_Output
            tb.Invoke(Sub()
                          tb.Enabled = False
                          tb.ResetText()

                          Try
                              If NativeMethods.User32.SetParent(hWnd, tb.Handle) = IntPtr.Zero Then
                                  Throw New Win32Exception(Marshal.GetLastWin32Error())
                              End If

                              If Not NativeMethods.User32.MoveWindow(hWnd, 0, 0, tb.Width, tb.Height, repaint:=True) Then
                                  Throw New Win32Exception(Marshal.GetLastWin32Error())
                              End If

                          Catch ex As Exception
                              Me.TextBox_PS3Dec_Output.Enabled = True
                              Me.TextBox_PS3Dec_Output.Text = $"Can't embed CMD window. Don't panic, this is an aesthetic error that does not affect the behavior.{Environment.NewLine & Environment.NewLine}Process Id.: {cmdProcess?.Id}, Main Window Handle: {hWnd}{Environment.NewLine & Environment.NewLine}Error message: {ex.Message}"

                          Finally
                              ' Ensure the CMD window gets visible but also disabled
                              ' to avoid user interaction and window closure by mistake.
                              If hWnd <> IntPtr.Zero Then
                                  NativeMethods.User32.EnableWindow(hWnd, enable:=False)
                              End If
                          End Try
                      End Sub)

            Try
                Me.cmdProcess.WaitForExit()
            Catch ex As Exception
                Form1.ShowMessageBoxInUIThread(Me, "Error executing PS3Dec.exe", ex.Message, MessageBoxIcon.Error)
                Exit Sub
            End Try

        Else ' Run PS3Dec.exe directly without embedding a CMD window.

            Me.ps3DecProcess = New Process()
            currentProcess = Me.ps3DecProcess
            With Me.ps3DecProcess
                .StartInfo.FileName = Form1.Settings.PS3DecExeFile.FullName
                .StartInfo.Arguments = $"d key ""{dkeyString}"" ""{isoFile.FullName}"" ""{Form1.Settings.OutputDir.FullName}\{isoFile.Name}"""
                .StartInfo.CreateNoWindow = True
                .StartInfo.RedirectStandardError = True
            End With

            Try
                Me.ps3DecProcess.Start()
                Me.ps3DecProcess.WaitForExit()
            Catch ex As Exception
                Form1.ShowMessageBoxInUIThread(Me, "Error executing PS3Dec.exe", ex.Message, MessageBoxIcon.Error)
                Exit Sub
            End Try
        End If

        If currentProcess.ExitCode = 0 Then
            Dim percentage As Integer = CInt(currentIsoIndex / totalIsoCount * 100)
            Me.UpdateStatus($"{percentage}% ({currentIsoIndex}/{totalIsoCount}) | {isoFile.Name} | Decryption completed.", writeToLogFile:=True)
            Me.CanDeleteEncryptedISO(pair.Key)
            Me.CanDeleteDecryptionKey(pair.Value)
        Else
            If currentProcess.StartInfo.RedirectStandardError Then
                Dim errorString As String = currentProcess.StandardError.ReadToEnd()
                Me.UpdateStatus($"PS3Dec.exe failed to decrypt, with process exit code: {currentProcess.ExitCode} and error message: {errorString}", writeToLogFile:=True)
            Else
                Me.UpdateStatus($"PS3Dec.exe failed to decrypt, with process exit code: {currentProcess.ExitCode}", writeToLogFile:=True)
            End If
        End If
    End Sub

    Private Sub CanDeleteEncryptedISO(iso As FileInfo)
        If Form1.Settings.DeleteDecryptedISOs Then
            Me.UpdateStatus($"Deleting encrypted PS3 ISO: {iso.Name}...", writeToLogFile:=True)
            Try
                iso.Delete()
            Catch ex As Exception
                Me.UpdateStatus($"Error deleting encrypted PS3 ISO. Error message: {ex.Message}", writeToLogFile:=True, TraceEventType.Warning)
                ' Form1.ShowMessageBoxInUIThread(Me, "Error deleting encrypted PS3 ISO", ex.Message, MessageBoxIcon.Warning)
            End Try
        End If
    End Sub

    Private Sub CanDeleteDecryptionKey(key As FileInfo)
        If Form1.Settings.DeleteKeysAfterUse Then
            Me.UpdateStatus($"Deleting decryption key: {key.Name}...", writeToLogFile:=True)
            Try
                key.Delete()
            Catch ex As Exception
                Me.UpdateStatus($"Error deleting decryption key. Error message: {ex.Message}", writeToLogFile:=True, TraceEventType.Warning)
                ' Form1.ShowMessageBoxInUIThread(Me, "Error deleting decryption key", ex.Message, MessageBoxIcon.Warning)
            End Try
        End If
    End Sub

    Private Sub ClearTempFiles(fileNamePattern As String)
        Dim dir As New DirectoryInfo(Me.tempFolderPath)
        If Not dir.Exists Then
            Return
        End If

        Me.UpdateStatus("Clearing temp files...", writeToLogFile:=False)
        For Each file As FileInfo In dir.GetFiles($"{fileNamePattern}.*", SearchOption.TopDirectoryOnly)
            Try
                file.Delete()
            Catch ex As Exception
                ' Ignore.
            End Try
        Next file
    End Sub

    ''' <summary>
    ''' If the source file is a zip archive, unzips it to the temporary folder (<see cref="Form1.tempFolderPath"/>).
    ''' </summary>
    ''' <returns>
    ''' If the source file is a zip archive, returns a <see cref="FileInfo"/> pointing to the unzipped file.
    ''' Otherwise, returns the source <see cref="FileInfo"/>.
    ''' </returns>
    Private Function UnzipFileIfNecessary(file As FileInfo, percentage As Integer, currentIsoIndex As Integer, totalIsoCount As Integer, additionalStatusInfoString As String) As FileInfo

        If Not file.Extension.Equals(".zip", StringComparison.OrdinalIgnoreCase) Then
            Return file
        End If

        Me.UpdateStatus($"{percentage}% ({currentIsoIndex}/{totalIsoCount}) | {file.Name} | Validating zip archive ({additionalStatusInfoString})...", writeToLogFile:=True)
        If Not Me.ValidateZipArchive(file) Then
            Return Nothing
        End If

        Me.UpdateStatus($"{percentage}% ({currentIsoIndex}/{totalIsoCount}) | {file.Name} | Unzipping file ({additionalStatusInfoString})...", writeToLogFile:=True)
        Try
            Dim drive As DriveInfo = New DirectoryInfo(tempFolderPath).GetDriveInfo()
            Const safetyExtraSize As Long = CLng(1024 ^ 2) * 300 ' 300 MB
            Dim requiredSizeString As String = Me.FormatFileSize(file.Length + safetyExtraSize, StrFormatByteSizeFlags.RoundToNearest)
            If Not Me.DriveHasFreeSpace(drive, file.Length) Then
                Form1.ShowMessageBoxInUIThread(Me, $"Error extracting zip archive.", $"Drive {drive} requires {requiredSizeString} of free space to extract the zip archive.", MessageBoxIcon.Error)
                Return Nothing
            End If

            Using archive As ZipArchive = ZipFile.OpenRead(file.FullName)

                Dim entry As ZipArchiveEntry = archive.Entries.Single()
                Dim destinationPath As String = Path.Combine(Me.tempFolderPath, entry.Name)
                Dim unzippedFile As New FileInfo(destinationPath)

                If Not Directory.Exists(Me.tempFolderPath) Then
                    Directory.CreateDirectory(Me.tempFolderPath)
                End If

                entry.ExtractToFile(unzippedFile.FullName, overwrite:=True)
                unzippedFile.Refresh()
                Return unzippedFile
            End Using

        Catch ex As Exception
            Form1.ShowMessageBoxInUIThread(Me, $"Error extracting zip archive.", ex.Message, MessageBoxIcon.Error)

        End Try

        Return Nothing

    End Function

    ''' <summary>
    ''' Validates whether the specified file is a zip archive, 
    ''' and whether the zip archive only contains a single file.
    ''' </summary>
    ''' <returns><see langword="True"/> if successful, <see langword="False"/> otherwise.</returns>
    Private Function ValidateZipArchive(file As FileInfo) As Boolean

        Try
            Using archive As ZipArchive = ZipFile.OpenRead(file.FullName)
                Select Case archive.Entries.Count
                    Case 1
                        Return True
                    Case 0
                        Form1.ShowMessageBoxInUIThread(Me, $"Error validating zip archive.", $"The zip archive is empty: {file.FullName}", MessageBoxIcon.Error)
                        Return False
                    Case Else
                        Form1.ShowMessageBoxInUIThread(Me, $"Error validating zip archive.", $"The zip archive contains more than one file: {file.FullName}", MessageBoxIcon.Error)
                        Return False
                End Select
            End Using

        Catch ex As InvalidDataException
            Return False

        Catch ex As Exception
            Form1.ShowMessageBoxInUIThread(Me, $"Error validating zip archive.", ex.Message, MessageBoxIcon.Error)
            Return False

        End Try

    End Function

    Private Sub UpdateStatus(statusText As String, writeToLogFile As Boolean, Optional eventType As TraceEventType = TraceEventType.Information)
        Me.ToolStripStatusLabel1.Text = statusText
        If writeToLogFile Then
            Form1.WriteLogEntry(eventType, statusText)
        End If
    End Sub

    Private Sub InitializeLogger()
        If Not Form1.Settings.LogEnabled Then
            Return
        End If

        Form1.logFileWriter = New StreamWriter(Form1.Settings.LogFile.FullName, append:=Form1.Settings.LogAppendMode, encoding:=Encoding.UTF8, bufferSize:=1024) With {.AutoFlush = True}
        Form1.logFileWriter.WriteLine(String.Format("          Log Date: {0}          ", Date.Now.Date.ToShortDateString()))
        Form1.logFileWriter.WriteLine("=========================================")
        Form1.logFileWriter.WriteLine()
    End Sub

    Private Shared Sub WriteLogEntry(eventType As TraceEventType, message As String)
        If Not Form1.Settings.LogEnabled OrElse Form1.logFileWriter Is Nothing Then
            Return
        End If

        Dim localDate As String = Date.Now.Date.ToShortDateString()
        Dim localTime As String = Date.Now.ToLongTimeString()
        Const entryFormat As String = "[{1}] | {2,-11} | {3}" ' {0}=Date, {1}=Time, {2}=Event, {3}=Message.

        Form1.logFileWriter.WriteLine(String.Format(entryFormat, localDate, localTime, eventType.ToString(), message))
    End Sub

    Private Sub DeinitializeLogger()
        If Not Form1.Settings.LogEnabled OrElse Form1.logFileWriter Is Nothing Then
            Return
        End If

        Try
            Form1.logFileWriter.WriteLine("End of log session.")
            Form1.logFileWriter.WriteLine()
            Form1.logFileWriter.Close()
        Catch ex As Exception
            ' Ignore.
        End Try
    End Sub

    Private Sub ResetProgressBar()
        Me.UpdateProgressBar(1, 0)
        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal)
        TaskbarManager.Instance.SetProgressValue(0, 0)
    End Sub

    Private Sub UpdateProgressBar(maxValue As Integer, currentValue As Integer)
        Me.ProgressBar_Decryption.Invoke(Sub()
                                             Me.ProgressBar_Decryption.Maximum = maxValue
                                             Me.ProgressBar_Decryption.Value = currentValue
                                         End Sub)
        TaskbarManager.Instance.SetProgressValue(currentValue, maxValue)
    End Sub

    Friend Shared Sub ShowMessageBoxInUIThread(f As Form, title As String, message As String, icon As MessageBoxIcon)
        f.Invoke(Sub() MessageBox.Show(f, message, title, MessageBoxButtons.OK, icon))

        Select Case icon
            Case MessageBoxIcon.Error
                Form1.WriteLogEntry(TraceEventType.Critical, message)

            Case MessageBoxIcon.Warning
                Form1.WriteLogEntry(TraceEventType.Warning, message)

            Case Else
                Form1.WriteLogEntry(TraceEventType.Information, message)
        End Select
    End Sub

    Private Sub SaveUserSettings()
        Try
            If Form1.Settings.SaveSettingsOnExit Then
                Me.UpdateStatus($"Saving user settings...", writeToLogFile:=False)
                My.Settings.EncryptedPS3DiscsDir = Form1.Settings.EncryptedPS3DiscsDir.FullName
                My.Settings.DecryptionKeysDir = Form1.Settings.DecryptionKeysDir.FullName
                My.Settings.PS3DecExePath = Form1.Settings.PS3DecExeFile.FullName
                My.Settings.OutputDir = Form1.Settings.OutputDir.FullName
                My.Settings.DeleteDecryptedISOs = Form1.Settings.DeleteDecryptedISOs
                My.Settings.DeleteKeysAfterUse = Form1.Settings.DeleteKeysAfterUse
                My.Settings.CompactMode = Form1.Settings.CompactMode
                My.Settings.RememberSizeAndPosition = Form1.Settings.RememberSizeAndPosition
                If My.Settings.RememberSizeAndPosition AndAlso Me.WindowState <> FormWindowState.Minimized Then
                    My.Settings.WindowPosition = Me.Location
                    My.Settings.WindowSize = Me.Size
                End If
                My.Settings.LogEnabled = Form1.Settings.LogEnabled
                My.Settings.LogAppendMode = Form1.Settings.LogAppendMode
                My.Settings.SaveSettingsOnExit = Form1.Settings.SaveSettingsOnExit
                My.Settings.Save()
            Else
                My.Settings.Reset()
            End If
        Catch ex As Exception
            Form1.ShowMessageBoxInUIThread(Me, My.Application.Info.Title, $"Error saving user settings: {ex.Message}", MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadUserSettings()

        Try
            If My.Settings.SaveSettingsOnExit Then
                Me.UpdateStatus($"Loading user settings...", writeToLogFile:=False)
                Dim dirPath As String = My.Application.Info.DirectoryPath
                Form1.Settings.EncryptedPS3DiscsDir = New DirectoryInfo(My.Settings.EncryptedPS3DiscsDir.Replace(dirPath, "."))
                Form1.Settings.DecryptionKeysDir = New DirectoryInfo(My.Settings.DecryptionKeysDir.Replace(dirPath, "."))
                Form1.Settings.PS3DecExeFile = New FileInfo(My.Settings.PS3DecExePath.Replace(dirPath, "."))
                Form1.Settings.OutputDir = New DirectoryInfo(My.Settings.OutputDir.Replace(dirPath, "."))
                Form1.Settings.DeleteDecryptedISOs = My.Settings.DeleteDecryptedISOs
                Form1.Settings.DeleteKeysAfterUse = My.Settings.DeleteKeysAfterUse
                Form1.Settings.CompactMode = My.Settings.CompactMode
                Form1.Settings.RememberSizeAndPosition = My.Settings.RememberSizeAndPosition
                If Form1.Settings.RememberSizeAndPosition Then
                    Me.Location = My.Settings.WindowPosition
                    Me.Size = My.Settings.WindowSize
                End If
                Form1.Settings.LogEnabled = My.Settings.LogEnabled
                Form1.Settings.LogAppendMode = My.Settings.LogAppendMode
                Form1.Settings.SaveSettingsOnExit = My.Settings.SaveSettingsOnExit
            End If
        Catch ex As Exception
            Form1.ShowMessageBoxInUIThread(Me, My.Application.Info.Title, $"Error loading user settings: {ex.Message}", MessageBoxIcon.Error)
        End Try
    End Sub


    Public Function DriveHasFreeSpace(drive As DriveInfo, requiredSpaceInBytes As Long) As Boolean

        If drive.IsReady Then
            Dim freeSpace As Long = drive.AvailableFreeSpace
            Return freeSpace > requiredSpaceInBytes
        Else
            Return False
        End If

    End Function

    Private Function FormatFileSize(bytes As Long, flags As StrFormatByteSizeFlags) As String

        Dim buffer As New StringBuilder(8, 16)
        Dim result As Integer = NativeMethods.StrFormatByteSizeEx(CULng(bytes), flags, buffer, 16)
        If result <> 0 Then ' HResult.S_OK
            Marshal.ThrowExceptionForHR(result)
        End If

        Return buffer.ToString()

    End Function

#End Region

End Class


Module FileSystemInfoExtensions
    <System.Runtime.CompilerServices.Extension>
    Public Function GetDriveInfo(fsi As IO.FileSystemInfo) As DriveInfo

        If fsi Is Nothing Then
            Throw New ArgumentNullException(paramName:=NameOf(fsi))
        End If

        Dim driveName As String = Path.GetPathRoot(fsi.FullName)
        Return New DriveInfo(driveName)
    End Function
End Module