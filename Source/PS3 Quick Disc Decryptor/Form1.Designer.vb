<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.PropertyGrid_Settings = New PropertyGrid()
        Me.Button_StartDecryption = New Button()
        Me.ProgressBar1 = New ProgressBar()
        Me.StatusStrip1 = New StatusStrip()
        Me.ToolStripStatusLabel1 = New ToolStripStatusLabel()
        Me.BackgroundWorker1 = New ComponentModel.BackgroundWorker()
        Me.Button_Abort = New Button()
        Me.TextBox_PS3Dec_Output = New TextBox()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        ' 
        ' PropertyGrid_Settings
        ' 
        Me.PropertyGrid_Settings.Location = New Point(12, 12)
        Me.PropertyGrid_Settings.Name = "PropertyGrid_Settings"
        Me.PropertyGrid_Settings.PropertySort = PropertySort.Categorized
        Me.PropertyGrid_Settings.Size = New Size(706, 247)
        Me.PropertyGrid_Settings.TabIndex = 9
        Me.PropertyGrid_Settings.ToolbarVisible = False
        ' 
        ' Button_StartDecryption
        ' 
        Me.Button_StartDecryption.Location = New Point(12, 451)
        Me.Button_StartDecryption.Name = "Button_StartDecryption"
        Me.Button_StartDecryption.Size = New Size(144, 35)
        Me.Button_StartDecryption.TabIndex = 10
        Me.Button_StartDecryption.Text = "Start Decryption"
        Me.Button_StartDecryption.UseVisualStyleBackColor = True
        ' 
        ' ProgressBar1
        ' 
        Me.ProgressBar1.Location = New Point(312, 451)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New Size(405, 35)
        Me.ProgressBar1.TabIndex = 12
        ' 
        ' StatusStrip1
        ' 
        Me.StatusStrip1.Items.AddRange(New ToolStripItem() {Me.ToolStripStatusLabel1})
        Me.StatusStrip1.Location = New Point(0, 494)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New Size(729, 22)
        Me.StatusStrip1.TabIndex = 13
        Me.StatusStrip1.Text = "StatusStrip1"
        ' 
        ' ToolStripStatusLabel1
        ' 
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New Size(538, 17)
        Me.ToolStripStatusLabel1.Text = "The program is ready. Press the ""Start Decryption"" button and you will see the decryption status here."
        ' 
        ' BackgroundWorker1
        ' 
        Me.BackgroundWorker1.WorkerSupportsCancellation = True
        ' 
        ' Button_Abort
        ' 
        Me.Button_Abort.Enabled = False
        Me.Button_Abort.Location = New Point(162, 451)
        Me.Button_Abort.Name = "Button_Abort"
        Me.Button_Abort.Size = New Size(144, 35)
        Me.Button_Abort.TabIndex = 14
        Me.Button_Abort.Text = "Abort"
        Me.Button_Abort.UseVisualStyleBackColor = True
        ' 
        ' TextBox_PS3Dec_Output
        ' 
        Me.TextBox_PS3Dec_Output.BackColor = Color.FromArgb(CByte(30), CByte(30), CByte(30))
        Me.TextBox_PS3Dec_Output.BorderStyle = BorderStyle.FixedSingle
        Me.TextBox_PS3Dec_Output.Enabled = False
        Me.TextBox_PS3Dec_Output.ForeColor = Color.IndianRed
        Me.TextBox_PS3Dec_Output.Location = New Point(12, 265)
        Me.TextBox_PS3Dec_Output.Multiline = True
        Me.TextBox_PS3Dec_Output.Name = "TextBox_PS3Dec_Output"
        Me.TextBox_PS3Dec_Output.ReadOnly = True
        Me.TextBox_PS3Dec_Output.Size = New Size(706, 180)
        Me.TextBox_PS3Dec_Output.TabIndex = 0
        ' 
        ' Form1
        ' 
        Me.AutoScaleDimensions = New SizeF(9F, 21F)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.ClientSize = New Size(729, 516)
        Me.Controls.Add(Me.TextBox_PS3Dec_Output)
        Me.Controls.Add(Me.Button_Abort)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.ProgressBar1)
        Me.Controls.Add(Me.Button_StartDecryption)
        Me.Controls.Add(Me.PropertyGrid_Settings)
        Me.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point)
        Me.FormBorderStyle = FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Me.Margin = New Padding(4)
        Me.MaximizeBox = False
        Me.Name = "Form1"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Text = "PS3 Quick Disc Decryptor"
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub
    Friend WithEvents PropertyGrid_Settings As PropertyGrid
    Friend WithEvents Button_StartDecryption As Button
    Friend WithEvents ProgressBar1 As ProgressBar
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents ToolStripStatusLabel1 As ToolStripStatusLabel
    Friend WithEvents BackgroundWorker1 As System.ComponentModel.BackgroundWorker
    Friend WithEvents Button_Abort As Button
    Friend WithEvents TextBox_PS3Dec_Output As TextBox

End Class
