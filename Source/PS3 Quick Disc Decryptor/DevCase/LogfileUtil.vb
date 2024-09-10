'Option Strict On
'Option Explicit On
'Option Infer Off

'Imports System.ComponentModel
'Imports System.IO
'Imports System.Text

'Namespace DevCase.Core.Diagnostics.Logging

'    Public Class LogfileUtil

'        Public Property EntryFormat As String = "[{0}] [{1}] | {2,-11} | {3}"


'        Public Sub WriteEntry(eventType As TraceEventType,
'                              message As String)

'            Dim localDate As String = Date.Now.Date.ToShortDateString()
'            Dim localTime As String = Date.Now.ToLongTimeString()
'            Me.sw.WriteLine(String.Format(Me.EntryFormat, localDate, localTime, eventType.ToString(), message))
'        End Sub

'        <DebuggerStepThrough>
'        Public Sub WriteText(text As String)
'            Me.sw.WriteLine(text)
'        End Sub

'        Public Sub WriteNewLine()
'            Me.sw.WriteLine()
'        End Sub

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
