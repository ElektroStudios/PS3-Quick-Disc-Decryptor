Option Strict On
Option Explicit On
Option Infer Off

Imports System.Runtime.InteropServices
Imports System.Runtime.Versioning

Namespace DevCase.Win32

    ''' <summary>
    ''' Contains the filenames to specify in <see cref="DllImportAttribute.Value"/> for all used Win32 API libraries.
    ''' </summary>
    <SupportedOSPlatform("windows")>
    Friend Module Win32LibNames

        ''' <summary>
        ''' User32.dll
        ''' </summary>
        Friend Const User32 As String = "User32.dll"

    End Module

End Namespace
