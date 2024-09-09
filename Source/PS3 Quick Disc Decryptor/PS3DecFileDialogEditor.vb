Option Strict On
Option Explicit On
Option Infer Off

Imports System.ComponentModel
Imports System.Drawing.Design
Imports System.IO

Friend NotInheritable Class PS3DecFileDialogEditor : Inherits UITypeEditor

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Function GetEditStyle(context As ITypeDescriptorContext) As UITypeEditorEditStyle
        Return UITypeEditorEditStyle.Modal
    End Function

    Public Overrides Function EditValue(context As ITypeDescriptorContext, provider As IServiceProvider, value As Object) As Object

        Using dlg As New OpenFileDialog() With {
            .Title = "Select the 'PS3Dec.exe' executable file...",
            .AutoUpgradeEnabled = True,
            .DereferenceLinks = True,
            .RestoreDirectory = True,
            .ShowHelp = False,
            .Multiselect = False, .Filter = "|PS3Dec.exe"
        }

            If dlg.ShowDialog() = DialogResult.OK Then
                If TypeOf value Is FileInfo Then
                    Return New FileInfo(dlg.FileName)
                Else
                    Return dlg.FileName
                End If
            End If
        End Using

        Return MyBase.EditValue(context, provider, value)

    End Function

End Class
