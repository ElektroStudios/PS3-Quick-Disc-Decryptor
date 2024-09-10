'Option Strict On
'Option Explicit On
'Option Infer Off

'Imports System.ComponentModel
'Imports System.IO
'Imports System.Text

'Namespace DevCase.Core.Diagnostics.Logging

'    ''' <summary>
'    ''' A simple logging system assistant that helps to create a log for the current application.
'    ''' </summary>
'    '''
'    ''' <example> This is a code example.
'    ''' <code language="VB.NET">
'    ''' Public Class Form1 : Inherits Form
'    ''' 
'    '''     Private logfile As New LogfileWriter(String.Format("{0}.log", My.Application.Info.AssemblyName)) With
'    '''         {
'    '''             .EntryFormat = "[{1}] | {2,-11} | {3}"
'    '''         } ' {0}=Date, {1}=Time, {2}=Event, {3}=Message.
'    ''' 
'    '''     Private Sub Form1_Load() Handles MyBase.Load
'    ''' 
'    '''         With Me.logfile
'    '''             .Clear()
'    '''             .WriteText("#########################################")
'    '''             .WriteNewLine()
'    '''             .WriteText(String.Format("          Log Date {0}          ", Date.Now.Date.ToShortDateString))
'    '''             .WriteNewLine()
'    '''             .WriteText("#########################################")
'    '''             .WriteNewLine()
'    '''             .WriteEntry(TraceEventType.Information, "Application is being initialized.")
'    '''         End With
'    ''' 
'    '''         Try
'    '''             Dim setting As Integer = Integer.Parse(" Hello World! :D ")
'    ''' 
'    '''         Catch ex As Exception
'    '''             Me.logfile.WriteEntry(TraceEventType.Critical, "Cannot parse 'setting' object in 'Sub Form1_Load()' method.")
'    '''             Me.logfile.WriteEntry(TraceEventType.Information, "Exiting...")
'    '''             Application.Exit()
'    ''' 
'    '''         End Try
'    ''' 
'    '''     End Sub
'    ''' 
'    ''' End Class
'    ''' </code>
'    ''' </example>
'    <ImmutableObject(False)>
'    Public Class LogfileWriter : Implements IDisposable

'#Region " Private Fields "

'        ''' <summary>
'        ''' The <see cref="StreamWriter"/> where is written the logging data
'        ''' </summary>
'        Private sw As StreamWriter

'#End Region

'#Region " Properties "

'        ''' <summary>
'        ''' Gets the log filepath.
'        ''' </summary>
'        '''
'        ''' <value>
'        ''' The log filepath.
'        ''' </value>
'        Public ReadOnly Property Filepath As String

'        ''' <summary>
'        ''' Gets the logfile encoding.
'        ''' </summary>
'        '''
'        ''' <value>
'        ''' The logfile encoding.
'        ''' </value>
'        Public ReadOnly Property Encoding As Encoding

'        ''' <summary>
'        ''' Gets or sets the format of a log entry.
'        ''' {0}=Date, {1}=Time, {2}=Event, {3}=Message.
'        ''' </summary>
'        '''
'        ''' <value>
'        ''' The format of a log entry.
'        ''' </value>
'        Public Property EntryFormat As String = "[{0}] [{1}] | {2,-11} | {3}"

'#End Region

'#Region " Constructors "

'        ''' <summary>
'        ''' Prevents a default instance of the <see cref="LogfileWriter"/> class from being created.
'        ''' </summary>
'        <DebuggerNonUserCode>
'        Private Sub New()
'        End Sub

'        ''' <summary>
'        ''' Initializes a new instance of the <see cref="LogfileWriter"/> class.
'        ''' </summary>
'        '''
'        ''' <param name="filepath">
'        ''' The log filepath.
'        ''' </param>
'        ''' 
'        ''' <param name="enc">
'        ''' The file encoding.
'        ''' </param>
'        <DebuggerStepThrough>
'        Public Sub New(filepath As String, Optional enc As Encoding = Nothing)

'            If enc Is Nothing Then
'                enc = Encoding.Default
'            End If

'            Me.Filepath = filepath
'            Me.Encoding = enc
'            Dim bufferSize As Integer = 1024 ' StreamWriterDefault = BufferSizes.Kb1
'            Me.sw = New StreamWriter(filepath, append:=True, encoding:=enc, bufferSize:=bufferSize) With {.AutoFlush = True}
'        End Sub

'#End Region

'#Region " Public Methods "

'        ''' <summary>
'        ''' Writes a new entry on the logfile.
'        ''' </summary>
'        '''
'        ''' <param name="eventType">
'        ''' The type of event.
'        ''' </param>
'        ''' 
'        ''' <param name="message">
'        ''' The message to log.
'        ''' </param>
'        <DebuggerStepThrough>
'        Public Sub WriteEntry(eventType As TraceEventType,
'                              message As String)

'            Dim localDate As String = Date.Now.Date.ToShortDateString
'            Dim localTime As String = Date.Now.ToLongTimeString

'            Me.sw.WriteLine(String.Format(Me.EntryFormat, localDate, localTime, eventType.ToString(), message))
'        End Sub

'        ''' <summary>
'        ''' Writes any text on the logfile.
'        ''' </summary>
'        '''
'        ''' <param name="text">
'        ''' The text to write.
'        ''' </param>
'        <DebuggerStepThrough>
'        Public Sub WriteText(text As String)

'            Me.sw.WriteLine(text)
'        End Sub

'        ''' <summary>
'        ''' Writes an empty line on the logfile.
'        ''' </summary>
'        <DebuggerStepThrough>
'        Public Sub WriteNewLine()

'            Me.sw.WriteLine()
'        End Sub

'        ''' <summary>
'        ''' Clears the logfile content.
'        ''' </summary>
'        <DebuggerStepThrough>
'        Public Sub Clear()
'            Me.sw.Close()

'            Dim bufferSize As Integer = 1024 ' StreamWriterDefault = BufferSizes.Kb1

'            Me.sw = New StreamWriter(Me.Filepath, append:=False, encoding:=Me.Encoding, bufferSize:=bufferSize) With {.AutoFlush = True}
'            Me.sw.BaseStream.SetLength(0)
'        End Sub

'#End Region

'#Region " IDisposable Implementation "

'        ''' <summary>
'        ''' Flag to detect redundant calls when disposing.
'        ''' </summary>
'        Private isDisposed As Boolean = False

'        ''' <summary>
'        ''' Releases all the resources used by this instance.
'        ''' </summary>
'        <DebuggerStepThrough>
'        Public Sub Dispose() Implements IDisposable.Dispose
'            Me.Dispose(isDisposing:=True)
'            GC.SuppressFinalize(Me)
'        End Sub

'        ''' <summary>
'        ''' Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
'        ''' Releases unmanaged and, optionally, managed resources.
'        ''' </summary>
'        '''
'        ''' <param name="isDisposing">
'        ''' <see langword="True"/>  to release both managed and unmanaged resources; 
'        ''' <see langword="False"/> to release only unmanaged resources.
'        ''' </param>
'        <DebuggerStepThrough>
'        Protected Sub Dispose(isDisposing As Boolean)

'            If (Not Me.isDisposed) AndAlso isDisposing Then
'                Me.sw?.Close()
'            End If

'            Me.isDisposed = True
'        End Sub

'#End Region

'    End Class

'End Namespace
