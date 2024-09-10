Option Strict On
Option Explicit On
Option Infer Off

Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Namespace DevCase.Extensions.StringExtensions

    ''' <summary>
    ''' Provides method extensions for a <see cref="String"/> type.
    ''' </summary>
    <HideModuleName>
    Public Module StringExtensions

        ''' <summary>
        ''' Determines whether a string is a Hexadecimal string.
        ''' </summary>
        '''
        ''' <example> This is a code example.
        ''' <code language="VB.NET">
        ''' MsgBox("0xFFFF".IsHexadecimal())
        ''' MsgBox("FFFF".IsHexadecimal())
        ''' MsgBox("FF FF".IsHexadecimal())
        ''' </code>
        ''' </example>
        '''
        ''' <param name="sender">
        ''' The source character.
        ''' </param>
        '''
        ''' <returns>
        ''' <see langword="True"/> if the string is a Hexadecimal string, <see langword="False"/> otherwise.
        ''' </returns>
        <DebuggerStepThrough>
        <Extension>
        <EditorBrowsable(EditorBrowsableState.Always)>
        Public Function IsHexadecimal(value As String) As Boolean

            If String.IsNullOrWhiteSpace(value) Then
                Return False
            End If

            value = value.Replace(" "c, "") ' eg. "FF FF FF" > "FFFFFF"
            If value.StartsWith("0x", StringComparison.OrdinalIgnoreCase) Then
                value = value.Remove(0, 2)
            End If

            Return value.All(Function(c As Char) "ABCDEF0123456789".Contains(c, StringComparison.OrdinalIgnoreCase)) AndAlso
                   (value.Length Mod 2) = 0 ' is even.
        End Function

    End Module

End Namespace