Option Strict On
Option Explicit On
Option Infer Off

Imports System.ComponentModel
Imports System.Drawing.Design
Imports System.IO

Friend NotInheritable Class ProgramSettings

    Public Sub New()
    End Sub

    <Category("Program Settings")>
    <Editor(GetType(FolderDialogEditor), GetType(UITypeEditor))>
    <DisplayName("PS3 ISOs directory")>
    <Description("Directory path containing Redump's encrypted PS3 disc images (*.iso files).")>
    Public Property EncryptedISOsDir As New DirectoryInfo(".\Encrypted\")
    Private Function ShouldSerializeEncryptedISOsDir() As Boolean
        Return Me.EncryptedISOsDir.FullName <> $"{My.Application.Info.DirectoryPath}\Encrypted\"
    End Function

    <Category("Program Settings")>
    <Editor(GetType(FolderDialogEditor), GetType(UITypeEditor))>
    <DisplayName("Output directory")>
    <Description("Directory path where to write the decrypted PS3 disc images.")>
    Public Property OutputDir As New DirectoryInfo(".\Decrypted\")
    Private Function ShouldSerializeOutputDir() As Boolean
        Return Me.OutputDir.FullName <> $"{My.Application.Info.DirectoryPath}\Decrypted\"
    End Function

    <Category("Program Settings")>
    <Editor(GetType(FolderDialogEditor), GetType(UITypeEditor))>
    <DisplayName("Decryption keys directory")>
    <Description("Directory path containing decryption keys for encrypted PS3 disc images.
The files can be *.dkey or *.txt and must contain a string of 32 characters long.")>
    Public Property DecryptionKeysDir As New DirectoryInfo(".\DKeys\")
    Private Function ShouldSerializeDecryptionKeysDir() As Boolean
        Return Me.DecryptionKeysDir.FullName <> $"{My.Application.Info.DirectoryPath}\DKeys\"
    End Function

    <Category("Program Settings")>
    <Editor(GetType(PS3DecFileDialogEditor), GetType(UITypeEditor))>
    <DisplayName("PS3Dec.exe")>
    <Description("Path pointing to PS3Dec.exe file, required to decrypt the PS3 disc images.
See: https://www.romhacking.net/utilities/1456/")>
    Public Property PS3DecExeFile As New FileInfo("PS3Dec.exe")
    Private Function ShouldSerializePS3DecExeFile() As Boolean
        Return Me.PS3DecExeFile.FullName <> $"{My.Application.Info.DirectoryPath}\PS3Dec.exe"
    End Function

    <Category("Program Settings")>
    <DisplayName("Delete encrypted ISOs after decryption")>
    <Description("If True, encrypted Redump ISOs will be permanently deleted from disk after successful decryption.")>
    <DefaultValue(False)>
    Public Property DeleteSuccessfullyConvertedISOs As Boolean = False

End Class