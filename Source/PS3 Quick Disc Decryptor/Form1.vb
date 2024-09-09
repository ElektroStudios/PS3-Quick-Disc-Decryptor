Option Strict On
Option Explicit On
Option Infer Off

Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading

Friend NotInheritable Class Form1

#Region " Private Fields "

    Private programSettings As New ProgramSettings()

    Private isos As IEnumerable(Of FileInfo)
    Private dkeys As IEnumerable(Of FileInfo)
    Private isoAndDkeyPairs As IDictionary(Of FileInfo, FileInfo)

    Private cmdProcess As Process
    Private ps3DecOpenHandle As FileStream

    Private cancelRequested As Boolean

#End Region

#Region " Event-Handlers "

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Me.PropertyGrid_Settings.SelectedObject = Me.programSettings
        Me.Text = $"{My.Application.Info.Title} v{My.Application.Info.Version}"
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        If e.CloseReason = CloseReason.UserClosing Then
            If Not Me.cmdProcess?.HasExited Then
                Dim question As DialogResult =
                    MessageBox.Show(Me, $"PS3Dec.exe is currently running and writing a decrypted disc in the output directory, if you exit this program PS3Dec.exe process will be killed.{Environment.NewLine & Environment.NewLine}Do you really want to exit?.", My.Application.Info.Title,
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation)

                If question = DialogResult.Yes Then
                    Try
                        Me.cmdProcess?.Kill(entireProcessTree:=True)
                    Catch ex As Exception
                    End Try
                Else
                    e.Cancel = True
                End If
            End If
        End If
    End Sub

    Private Sub Button_StartDecryption_Click(sender As Object, e As EventArgs) Handles Button_StartDecryption.Click

        If Not Me.BackgroundWorker1.IsBusy Then
            Me.PropertyGrid_Settings.Enabled = False
            Me.Button_StartDecryption.Enabled = False
            Me.Button_Abort.Enabled = True

            Me.BackgroundWorker1.RunWorkerAsync()
        End If
    End Sub

    Private Sub Button_Abort_Click(sender As Object, e As EventArgs) Handles Button_Abort.Click

        If Me.BackgroundWorker1.IsBusy Then
            Dim btn As Button = DirectCast(sender, Button)
            btn.Enabled = False
            Me.cancelRequested = True

            Me.ToolStripStatusLabel1.Text = "Aborting, please wait until the current disc gets decrypted..."
        End If
    End Sub

    Private Sub BackgroundWorker1_DoWork(sender As Object, e As DoWorkEventArgs) Handles BackgroundWorker1.DoWork

        Me.UpdateProgressBar(1, 0)

        If Not Me.FetchISOs() OrElse
           Not Me.FetchDecryptionKeys() OrElse
           Not Me.BuildIsoAndDKeyPairs() OrElse
           Not Me.VerifyPS3DecExe() Then

            e.Cancel = True
            Exit Sub
        End If

        Dim totalIsoCount As Integer = Me.isoAndDkeyPairs.Count
        Dim currentIsoIndex As Integer = 0
        Me.UpdateProgressBar(totalIsoCount, 0)

        If Me.isoAndDkeyPairs?.Any() Then

            For Each pair As KeyValuePair(Of FileInfo, FileInfo) In Me.isoAndDkeyPairs
                If Me.cancelRequested Then
                    Me.BackgroundWorker1.CancelAsync()
                    e.Cancel = True
                    Exit For
                End If

                If Not Me.BackgroundWorker1.CancellationPending Then
                    Me.ProcessDecryption(pair, currentIsoIndex, totalIsoCount)
                    Me.UpdateProgressBar(totalIsoCount, Interlocked.Increment(currentIsoIndex))
                End If
            Next
        End If
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted

        If e.Error IsNot Nothing Then
            MessageBox.Show(Me, e.Error.Message, "Unknown error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

        If e.Cancelled AndAlso Me.cancelRequested Then
            Me.ToolStripStatusLabel1.Text = "Operation aborted on demand."
            MessageBox.Show(Me, "Operation aborted on demand.", My.Application.Info.Title, MessageBoxButtons.OK, MessageBoxIcon.Information)
        ElseIf e.Cancelled Then
            Me.ToolStripStatusLabel1.Text = "Operation cancelled due an error."
        Else
            Me.ToolStripStatusLabel1.Text = "Operation completed."
            MessageBox.Show(Me, "Operation completed.", My.Application.Info.Title, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

        Try
            Me.ps3DecOpenHandle?.Close()
            Me.ps3DecOpenHandle = Nothing
        Catch ex As Exception
            MessageBox.Show(Me, ex.Message, "Error releasing PS3Dec.exe file handle", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try

        Me.isos = Nothing
        Me.dkeys = Nothing
        Me.isoAndDkeyPairs = Nothing
        Me.cmdProcess = Nothing

        Me.cancelRequested = False

        Me.PropertyGrid_Settings.Enabled = True
        Me.Button_StartDecryption.Enabled = True
        Me.Button_Abort.Enabled = False
    End Sub

#End Region

#Region " Private Methods "

    Private Function FetchISOs() As Boolean

        Me.SetStatusLabelText("Fetching PS3 ISOs...")
        Try
            Me.isos = Me.programSettings.EncryptedISOsDir?.GetFiles("*.iso", SearchOption.TopDirectoryOnly)
        Catch ex As Exception
            Me.ShowErrorMessageBoxInUIThread("Error fetching PS3 ISOs", ex.Message)
            Return False
        End Try

        If Not Me.isos.Any() Then
            Me.ShowErrorMessageBoxInUIThread("Error fetching PS3 ISOs", $"Can't find any ISO files in the specified directory: '{Me.programSettings.EncryptedISOsDir}'")
            Return False
        End If

        Return True
    End Function

    Private Function FetchDecryptionKeys() As Boolean

        Me.SetStatusLabelText("Fetching decryption keys...")
        Try
            Me.dkeys = Me.programSettings.DecryptionKeysDir?.
                                     GetFiles("*.*", SearchOption.TopDirectoryOnly).
                                     Where(Function(x) x.Extension.ToLowerInvariant() = ".dkey" OrElse
                                                       x.Extension.ToLowerInvariant() = ".txt")
        Catch ex As Exception
            Me.ShowErrorMessageBoxInUIThread("Error fetching decryption keys", ex.Message)
            Return False
        End Try

        If Not Me.dkeys.Any() Then
            Me.ShowErrorMessageBoxInUIThread("Error fetching decryption keys", $"Can't find any decryption key files in the specified directory: {Me.programSettings.DecryptionKeysDir}")
            Return False
        End If

        Return True
    End Function

    Private Function BuildIsoAndDKeyPairs() As Boolean

        Me.SetStatusLabelText("Matching PS3 ISOs with decryption keys...")
        Try
            Me.isoAndDkeyPairs = New Dictionary(Of FileInfo, FileInfo)
            For Each iso As FileInfo In Me.isos
                Dim match As FileInfo =
                    (From dkey As FileInfo In Me.dkeys
                     Where Path.GetFileNameWithoutExtension(dkey.Name).Equals(Path.GetFileNameWithoutExtension(iso.Name), StringComparison.OrdinalIgnoreCase)
                    ).SingleOrDefault()

                If match IsNot Nothing Then
                    Me.isoAndDkeyPairs.Add(iso, match)
                End If
            Next iso

        Catch ex As Exception
            Me.ShowErrorMessageBoxInUIThread("Error matching PS3 ISOs with decryption keys", ex.Message)
            Return False
        End Try

        Dim isosCount As Integer = Me.isos.Count
        Dim diffCount As Integer = isosCount - Me.isoAndDkeyPairs.Count
        If diffCount = isosCount Then
            Me.ShowErrorMessageBoxInUIThread("Missing decryption key matches", $"The program could not find any matchign decryption keys for the current ISOs.")
            Return False
        ElseIf diffCount <> 0 Then
            Me.ShowWarnMessageBoxInUIThread("Missing decryption key matches", $"The program could not find matchign decryption keys for {diffCount} out of {isosCount} ISOs.{Environment.NewLine & Environment.NewLine}The program will proceed now decrypting the remaining ISOs.")
        End If

        Return True
    End Function

    Private Function VerifyPS3DecExe() As Boolean

        Me.SetStatusLabelText("Acquiring PS3Dec.exe file handle...")
        If Not Me.programSettings.PS3DecExeFile.Exists Then
            Me.ShowErrorMessageBoxInUIThread("Error acquiring PS3Dec.exe file handle", $"PS3Dec.exe was not found at: {Me.programSettings.PS3DecExeFile.FullName}")
            Return False
        End If

        Try
            Me.ps3DecOpenHandle = Me.programSettings.PS3DecExeFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read)
        Catch ex As Exception
            Me.ShowErrorMessageBoxInUIThread("Error acquiring PS3Dec.exe file handle", ex.Message)
            Return False
        End Try

        Return True
    End Function

    Private Sub ProcessDecryption(pair As KeyValuePair(Of FileInfo, FileInfo), currentIsoIndex As Integer, totalIsoCount As Integer)

        Dim percentage As Integer = CInt(currentIsoIndex / totalIsoCount * 100)

        Me.SetStatusLabelText($"{percentage}% ({currentIsoIndex}/{totalIsoCount}) | {pair.Key.Name} | Parsing decryption key file content...")
        Dim dkeyString As String = Nothing
        If Not Me.ReadDecryptionKey(pair.Value, dkeyString) Then
            Exit Sub
        End If

        Me.SetStatusLabelText($"{percentage}% ({currentIsoIndex}/{totalIsoCount}) | {pair.Key.Name} | Writing decrypted ISO to output directory...")
        If Not Me.EnsureOutputDirectoryExists() Then
            Exit Sub
        End If
        Me.ExecutePS3Dec(pair.Key, dkeyString, currentIsoIndex, totalIsoCount)

    End Sub

    Private Function ReadDecryptionKey(dkeyFile As FileInfo, ByRef refKeyString As String) As Boolean

        Try
            Dim dkeyString As String = File.ReadAllText(dkeyFile.FullName, Encoding.Default).Trim()
            If dkeyString.Length <> 32 Then
                Me.ShowErrorMessageBoxInUIThread("Error parsing decryption key file content", $"File: {dkeyFile.FullName}{Environment.NewLine & Environment.NewLine}Error message: Decryption key has an invalid length of {dkeyString.Length} (it must be 32 character length).")
                Return False
            End If
            refKeyString = dkeyString

        Catch ex As Exception
            Me.ShowErrorMessageBoxInUIThread("Error reading decryption key file", $"File: {dkeyFile.FullName}{Environment.NewLine & Environment.NewLine}Error message: {ex.Message}")
            Return False

        End Try

        Return True
    End Function

    Private Function EnsureOutputDirectoryExists() As Boolean

        If Not Me.programSettings.OutputDir.Exists Then
            Try
                Me.programSettings.OutputDir.Create()
            Catch ex As Exception
                Me.ShowErrorMessageBoxInUIThread("Error creating output directory", ex.Message)
                Return False
            End Try
        End If
        Return True
    End Function

    Private Sub ExecutePS3Dec(isoFile As FileInfo, dkeyString As String, currentIsoIndex As Integer, totalIsoCount As Integer)

        Me.cmdProcess = New Process()
        With Me.cmdProcess
            .StartInfo.FileName = Environment.GetEnvironmentVariable("COMSPEC")
            .StartInfo.Arguments = $"/C ""(TIMEOUT /T 1 /NOBREAK)>NUL & ""{Me.programSettings.PS3DecExeFile.FullName}"" d key ""{dkeyString}"" ""{isoFile.FullName}"" ""{Me.programSettings.OutputDir.FullName}\{isoFile.Name}"""""
            .StartInfo.UseShellExecute = True
            .StartInfo.CreateNoWindow = False
            .StartInfo.WindowStyle = ProcessWindowStyle.Minimized
        End With
        Try
            Me.cmdProcess.Start()
        Catch ex As Exception
            Me.ShowErrorMessageBoxInUIThread("Error executing PS3Dec.exe", ex.Message)
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

        Me.TextBox_PS3Dec_Output.Invoke(
        Sub()
            Me.TextBox_PS3Dec_Output.Enabled = False
            Me.TextBox_PS3Dec_Output.ResetText()

            Try
                If NativeMethods.SetParent(hWnd, Me.TextBox_PS3Dec_Output.Handle) = IntPtr.Zero Then
                    Throw New Win32Exception(Marshal.GetLastWin32Error())
                End If

                If Not NativeMethods.MoveWindow(hWnd, 0, 0, Me.TextBox_PS3Dec_Output.Width, Me.TextBox_PS3Dec_Output.Height, True) Then
                    Throw New Win32Exception(Marshal.GetLastWin32Error())
                End If

                NativeMethods.EnableWindow(hWnd, False)
            Catch ex As Win32Exception
                Me.TextBox_PS3Dec_Output.Enabled = True
                Me.TextBox_PS3Dec_Output.Text = $"Can't embed CMD window. Don't panic, this is an aesthetic error that does not affect the behavior.{Environment.NewLine & Environment.NewLine}Process Id.: {cmdProcess?.Id}, Main Window Handle: {hWnd}{Environment.NewLine & Environment.NewLine}Error message: {ex.Message}"
                If hWnd <> IntPtr.Zero Then
                    ' Ensure the CMD window gets visible but also disabled
                    ' to avoid user interaction and window closure by mistake.
                    NativeMethods.EnableWindow(hWnd, False)
                End If
            End Try
        End Sub)

        Me.cmdProcess.WaitForExit()
        If Me.cmdProcess.ExitCode = 0 Then
            Dim percentage As Integer = If(currentIsoIndex = 1, 0, CInt((currentIsoIndex) / totalIsoCount) * 100)
            Me.SetStatusLabelText($"{percentage}% ({currentIsoIndex}/{totalIsoCount}) | {isoFile.Name} | Decryption completed.")
            Me.CanDeleteSuccessfullyConvertedISO(isoFile)
        End If

        ' ----------------------------------------------------------------------------------
        ' ALTERNATIVE METHODOLOGY: Run PS3Dec.exe directly (without embedding a CMD window).
        ' ----------------------------------------------------------------------------------
        '
        'Me.ps3DecProcess = New Process()
        'With Me.ps3DecProcess
        '    .StartInfo.FileName = Me.programSettings.PS3DecExeFile.FullName
        '    .StartInfo.Arguments = $"d key ""{dkeyString}"" ""{isoFile.FullName}"" ""{Me.programSettings.OutputDir.FullName}\{isoFile.Name}"""
        '    .StartInfo.CreateNoWindow = True
        'End With
        '
        'Try
        '    Me.ps3DecProcess.Start()
        '    Me.ps3DecProcess.WaitForExit()
        'Catch ex As Exception
        '    Me.ShowErrorMessageBoxInUIThread("Error executing PS3Dec.exe", ex.Message)
        '    Exit Sub
        'End Try
        '
        'If Me.ps3DecProcess.ExitCode = 0 Then
        '    Dim percentage As Integer = If(currentIsoIndex = 1, 0, CInt((currentIsoIndex) / totalIsoCount) * 100)
        '    Me.SetStatusLabelText($"{percentage}% ({currentIsoIndex}/{totalIsoCount}) | {isoFile.Name} | Decryption completed.")
        '    Me.CanDeleteSuccessfullyConvertedISO(isoFile)
        'End If

    End Sub

    Private Sub CanDeleteSuccessfullyConvertedISO(isoFile As FileInfo)
        If Me.programSettings.DeleteSuccessfullyConvertedISOs Then
            Me.SetStatusLabelText($"Deleting encrypted PS3 ISO: {isoFile.Name}...")
            Try
                isoFile.Delete()
            Catch ex As Exception
                Me.ShowWarnMessageBoxInUIThread("Error deleting encrypted PS3 ISO", ex.Message)
            End Try
        End If
    End Sub

    Private Sub SetStatusLabelText(statusText As String)
        Me.ToolStripStatusLabel1.Text = statusText
    End Sub

    Private Sub UpdateProgressBar(maxValue As Integer, currentValue As Integer)
        Me.ProgressBar1.Invoke(Sub()
                                   Me.ProgressBar1.Maximum = maxValue
                                   Me.ProgressBar1.Value = currentValue
                               End Sub)
    End Sub

    Private Sub ShowInfoMessageBoxInUIThread(title As String, message As String)
        Me.Invoke(Sub() MessageBox.Show(Me, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information))
    End Sub

    Private Sub ShowErrorMessageBoxInUIThread(title As String, message As String)
        Me.Invoke(Sub() MessageBox.Show(Me, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error))
    End Sub

    Private Sub ShowWarnMessageBoxInUIThread(title As String, message As String)
        Me.Invoke(Sub() MessageBox.Show(Me, message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning))
    End Sub

#End Region

End Class
