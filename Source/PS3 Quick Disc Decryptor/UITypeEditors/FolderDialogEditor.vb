Option Strict On
Option Explicit On
Option Infer Off

Imports System.ComponentModel
Imports System.Drawing.Design
Imports System.IO

Friend Class FolderDialogEditor : Inherits UITypeEditor

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Function GetEditStyle(context As ITypeDescriptorContext) As UITypeEditorEditStyle
        Return UITypeEditorEditStyle.Modal
    End Function

    Public Overrides Function EditValue(context As ITypeDescriptorContext, provider As IServiceProvider, value As Object) As Object

        Dim dlg As New FolderBrowserDialog With {
            .AutoUpgradeEnabled = True,
            .Description = "Select a directory...",
            .ShowNewFolderButton = True
        }

        If dlg.ShowDialog() = DialogResult.OK Then
#Disable Warning IDE0046 ' Convert to conditional expression
            If TypeOf value Is DirectoryInfo Then
                Return New DirectoryInfo(dlg.SelectedPath)
            Else
                Return dlg.SelectedPath
            End If
#Enable Warning IDE0046 ' Convert to conditional expression

        Else
            Return value

        End If

        Return MyBase.EditValue(context, provider, value)
    End Function

End Class
