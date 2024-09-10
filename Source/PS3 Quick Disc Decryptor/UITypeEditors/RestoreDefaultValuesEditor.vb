Option Strict On
Option Explicit On
Option Infer Off

Imports System.ComponentModel
Imports System.Drawing.Design

Friend NotInheritable Class RestoreDefaultValuesEditor : Inherits UITypeEditor

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Function GetEditStyle(context As ITypeDescriptorContext) As UITypeEditorEditStyle
        Return UITypeEditorEditStyle.Modal
    End Function

    Public Overrides Function EditValue(context As ITypeDescriptorContext, provider As IServiceProvider, value As Object) As Object
        Form1.Settings.SetDefaultValues()
        Form1.PropertyGrid_Settings.Refresh()
        Form1.ShowMessageBoxInUIThread(Form1, My.Application.Info.Title, "Default values has been restored for program settings.", MessageBoxIcon.Information)
        Return MyBase.EditValue(context, provider, value)
    End Function

End Class