﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
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
        Me.ProgressBar_Decryption = New ProgressBar()
        Me.StatusStrip1 = New StatusStrip()
        Me.ToolStripStatusLabel1 = New ToolStripStatusLabel()
        Me.BackgroundWorker1 = New ComponentModel.BackgroundWorker()
        Me.Button_Abort = New Button()
        Me.TextBox_PS3Dec_Output = New TextBox()
        Me.TableLayoutPanel1 = New TableLayoutPanel()
        Me.Panel_Buttons = New Panel()
        Me.StatusStrip1.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.Panel_Buttons.SuspendLayout()
        Me.SuspendLayout()
        ' 
        ' PropertyGrid_Settings
        ' 
        Me.PropertyGrid_Settings.Dock = DockStyle.Fill
        Me.PropertyGrid_Settings.Location = New Point(3, 3)
        Me.PropertyGrid_Settings.Name = "PropertyGrid_Settings"
        Me.PropertyGrid_Settings.PropertySort = PropertySort.Categorized
        Me.PropertyGrid_Settings.Size = New Size(642, 284)
        Me.PropertyGrid_Settings.TabIndex = 1
        Me.PropertyGrid_Settings.ToolbarVisible = False
        ' 
        ' Button_StartDecryption
        ' 
        Me.Button_StartDecryption.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        Me.Button_StartDecryption.Location = New Point(3, 6)
        Me.Button_StartDecryption.Name = "Button_StartDecryption"
        Me.Button_StartDecryption.Size = New Size(127, 33)
        Me.Button_StartDecryption.TabIndex = 0
        Me.Button_StartDecryption.Text = "Start Decryption"
        Me.Button_StartDecryption.UseVisualStyleBackColor = True
        ' 
        ' ProgressBar_Decryption
        ' 
        Me.ProgressBar_Decryption.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        Me.ProgressBar_Decryption.Location = New Point(270, 6)
        Me.ProgressBar_Decryption.Name = "ProgressBar_Decryption"
        Me.ProgressBar_Decryption.Size = New Size(369, 33)
        Me.ProgressBar_Decryption.TabIndex = 2
        ' 
        ' StatusStrip1
        ' 
        Me.StatusStrip1.Items.AddRange(New ToolStripItem() {Me.ToolStripStatusLabel1})
        Me.StatusStrip1.Location = New Point(0, 470)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Padding = New Padding(2, 0, 13, 0)
        Me.StatusStrip1.Size = New Size(648, 22)
        Me.StatusStrip1.TabIndex = 0
        Me.StatusStrip1.Text = "StatusStrip1"
        ' 
        ' ToolStripStatusLabel1
        ' 
        Me.ToolStripStatusLabel1.DisplayStyle = ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusLabel1.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New Size(633, 17)
        Me.ToolStripStatusLabel1.Spring = True
        Me.ToolStripStatusLabel1.Text = "..."
        Me.ToolStripStatusLabel1.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' BackgroundWorker1
        ' 
        Me.BackgroundWorker1.WorkerSupportsCancellation = True
        ' 
        ' Button_Abort
        ' 
        Me.Button_Abort.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        Me.Button_Abort.Enabled = False
        Me.Button_Abort.Location = New Point(136, 6)
        Me.Button_Abort.Name = "Button_Abort"
        Me.Button_Abort.Size = New Size(128, 33)
        Me.Button_Abort.TabIndex = 1
        Me.Button_Abort.Text = "Abort"
        Me.Button_Abort.UseVisualStyleBackColor = True
        ' 
        ' TextBox_PS3Dec_Output
        ' 
        Me.TextBox_PS3Dec_Output.BackColor = Color.FromArgb(CByte(30), CByte(30), CByte(30))
        Me.TextBox_PS3Dec_Output.BorderStyle = BorderStyle.FixedSingle
        Me.TextBox_PS3Dec_Output.Dock = DockStyle.Fill
        Me.TextBox_PS3Dec_Output.Enabled = False
        Me.TextBox_PS3Dec_Output.ForeColor = Color.IndianRed
        Me.TextBox_PS3Dec_Output.Location = New Point(3, 293)
        Me.TextBox_PS3Dec_Output.Multiline = True
        Me.TextBox_PS3Dec_Output.Name = "TextBox_PS3Dec_Output"
        Me.TextBox_PS3Dec_Output.ReadOnly = True
        Me.TextBox_PS3Dec_Output.Size = New Size(642, 123)
        Me.TextBox_PS3Dec_Output.TabIndex = 2
        ' 
        ' TableLayoutPanel1
        ' 
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100F))
        Me.TableLayoutPanel1.Controls.Add(Me.Panel_Buttons, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.PropertyGrid_Settings, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.TextBox_PS3Dec_Output, 0, 1)
        Me.TableLayoutPanel1.Dock = DockStyle.Fill
        Me.TableLayoutPanel1.Location = New Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 3
        Me.TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Percent, 69.07545F))
        Me.TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Percent, 30.92455F))
        Me.TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Absolute, 50F))
        Me.TableLayoutPanel1.Size = New Size(648, 470)
        Me.TableLayoutPanel1.TabIndex = 1
        ' 
        ' Panel_Buttons
        ' 
        Me.Panel_Buttons.Controls.Add(Me.Button_StartDecryption)
        Me.Panel_Buttons.Controls.Add(Me.Button_Abort)
        Me.Panel_Buttons.Controls.Add(Me.ProgressBar_Decryption)
        Me.Panel_Buttons.Dock = DockStyle.Fill
        Me.Panel_Buttons.Location = New Point(3, 422)
        Me.Panel_Buttons.Name = "Panel_Buttons"
        Me.Panel_Buttons.Size = New Size(642, 45)
        Me.Panel_Buttons.TabIndex = 0
        ' 
        ' Form1
        ' 
        Me.AutoScaleDimensions = New SizeF(8F, 20F)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.ClientSize = New Size(648, 492)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.DoubleBuffered = True
        Me.Font = New Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point)
        Me.Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Me.Name = "Form1"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Text = "PS3 Quick Disc Decryptor"
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.Panel_Buttons.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub
    Friend WithEvents PropertyGrid_Settings As PropertyGrid
    Friend WithEvents Button_StartDecryption As Button
    Friend WithEvents ProgressBar_Decryption As ProgressBar
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents ToolStripStatusLabel1 As ToolStripStatusLabel
    Friend WithEvents BackgroundWorker1 As System.ComponentModel.BackgroundWorker
    Friend WithEvents Button_Abort As Button
    Friend WithEvents TextBox_PS3Dec_Output As TextBox
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents Panel_Buttons As Panel

End Class
