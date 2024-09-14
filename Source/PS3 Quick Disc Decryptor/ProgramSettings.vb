Option Strict On
Option Explicit On
Option Infer Off

Imports System.ComponentModel
Imports System.Drawing.Design
Imports System.Globalization
Imports System.IO

Friend NotInheritable Class ProgramSettings

    <Category("1) Resources")>
    <DisplayName("Encrypted PS3 disc images")>
    <Description("Directory path that contains encrypted PS3 disc images from Redump.
The files can be either *.iso or *.zip.")>
    <Editor(GetType(FolderDialogEditor), GetType(UITypeEditor))>
    Public Property EncryptedPS3DiscsDir As DirectoryInfo
    Private Function ShouldSerializeEncryptedPS3DiscsDir() As Boolean
        Return Not String.Equals(Me.EncryptedPS3DiscsDir.FullName,
                                 $"{My.Application.Info.DirectoryPath}\Encrypted",
                                 StringComparison.OrdinalIgnoreCase)
    End Function

    <Category("1) Resources")>
    <DisplayName("Decryption keys")>
    <Description("Directory path containing decryption keys for the encrypted PS3 disc images from Redump.
The files can be either *.dkey or *.txt containing a string of 32 characters long.")>
    <Editor(GetType(FolderDialogEditor), GetType(UITypeEditor))>
    Public Property DecryptionKeysDir As DirectoryInfo
    Private Function ShouldSerializeDecryptionKeysDir() As Boolean
        Return Not String.Equals(Me.DecryptionKeysDir.FullName,
                                 $"{My.Application.Info.DirectoryPath}\Keys",
                                 StringComparison.OrdinalIgnoreCase)
    End Function

    <Category("1) Resources")>
    <DisplayName("Output directory")>
    <Description("Directory path where the decrypted PS3 disc images will be saved.")>
    <Editor(GetType(FolderDialogEditor), GetType(UITypeEditor))>
    Public Property OutputDir As DirectoryInfo
    Private Function ShouldSerializeOutputDir() As Boolean
        Return Not String.Equals(Me.OutputDir.FullName,
                                 $"{My.Application.Info.DirectoryPath}\Decrypted",
                                 StringComparison.OrdinalIgnoreCase)
    End Function

    <Category("1) Resources")>
    <DisplayName("PS3Dec.exe")>
    <Description("Path to the PS3Dec.exe file, which is required for decrypting PS3 disc images from Redump.
For more information, visit: https://www.romhacking.net/utilities/1456/")>
    <Editor(GetType(PS3DecFileDialogEditor), GetType(UITypeEditor))>
    Public Property PS3DecExeFile As FileInfo
    Private Function ShouldSerializePS3DecExeFile() As Boolean
        Return Me.PS3DecExeFile.FullName <> $"{My.Application.Info.DirectoryPath}\PS3Dec.exe"
    End Function

    <Category("2) Clean up")>
    <DisplayName("Delete PS3 disc images after decryption")>
    <Description("If set to True, encrypted PS3 disc images from Redump will be permanently deleted
after they are used for successful decryption.")>
    <DefaultValue(False)>
    Public Property DeleteDecryptedISOs As Boolean

    <Category("2) Clean up")>
    <DisplayName("Delete decryption keys after use")>
    <Description("If set to True, decryption keys will be permanently deleted
after they are used for successful decryption.")>
    <DefaultValue(False)>
    Public Property DeleteKeysAfterUse As Boolean

    <Category("3) User-Interface")>
    <DisplayName("Compact Mode")>
    <Description("Enable or disable compact view mode.")>
    <DefaultValue(False)>
    Public Property CompactMode As Boolean
        Get
            Return Me._compactMode
        End Get
        Set(value As Boolean)
            If value <> Me._compactMode Then
                Me._compactMode = value
                My.Forms.Form1.TextBox_PS3Dec_Output.Visible = Not value
            End If
        End Set
    End Property
    Private _compactMode As Boolean

    <Category("3) User-Interface")>
    <DisplayName("Remember size and position")>
    <Description("Remember last window size and position when the program starts.")>
    <DefaultValue(False)>
    Public Property RememberSizeAndPosition As Boolean

    <Category("3) User-Interface")>
    <DisplayName("Remember current settings")>
    <Description("Save current settings when the program is closed, and loads them when the program starts.")>
    <DefaultValue(False)>
    Public Property SaveSettingsOnExit As Boolean

    <Category("3) User-Interface")>
    <DisplayName("Restore default settings")>
    <Description("Restore the default values for the program settings.")>
    <Editor(GetType(RestoreDefaultValuesEditor), GetType(UITypeEditor))>
    Public Property RestoreDefaultValues As String = "Click the button to restore default settings ->"
    Private Shared Function ShouldSerializeRestoreDefaultValues() As Boolean
        Return False
    End Function

    <Category("4) Logging")>
    <DisplayName("Log file")>
    <Description("Path to the log file that will register information and errors during the decryption procedure.")>
    Public ReadOnly Property LogFile As New FileInfo($".\{My.Application.Info.AssemblyName}.log")

    <Category("4) Logging")>
    <DisplayName("Enabled")>
    <Description("True to enable logging feature, False otherwise.")>
    <DefaultValue(True)>
    Public Property LogEnabled As Boolean = True

    <Category("4) Logging")>
    <DisplayName("Append mode")>
    <Description("If set to False, the log file will be overwritten at the next program start 
if 'Remember current settings' option is set to True.")>
    <DefaultValue(True)>
    Public Property LogAppendMode As Boolean = True

    Public Sub New()
        Me.SetDefaultValues()
    End Sub

    Public Sub SetDefaultValues()
        ' 1) Resources group
        Me.EncryptedPS3DiscsDir = New DirectoryInfo(".\Encrypted")
        Me.DecryptionKeysDir = New DirectoryInfo(".\Keys")
        Me.OutputDir = New DirectoryInfo(".\Decrypted")
        Me.PS3DecExeFile = New FileInfo(".\PS3Dec.exe")

        ' 2) Clean up group
        Me.DeleteDecryptedISOs = False
        Me.DeleteKeysAfterUse = False

        ' 3) User-Interface group
        Me.CompactMode = False
        Me.RememberSizeAndPosition = False
        Me.SaveSettingsOnExit = False

        ' 4) Logging group
        Me.LogEnabled = True
        Me.LogAppendMode = True
    End Sub

End Class
