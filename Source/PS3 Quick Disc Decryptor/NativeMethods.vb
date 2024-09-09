Option Strict On
Option Explicit On
Option Infer Off

Imports System.Runtime.InteropServices

Friend NotInheritable Class NativeMethods

    Private Sub New()
    End Sub

    <DllImport("user32.dll", SetLastError:=True)>
    Friend Shared Function SetParent(hWndChild As IntPtr, hWndNewParent As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Friend Shared Function MoveWindow(hWnd As IntPtr, x As Integer, y As Integer, width As Integer, height As Integer, redraw As Boolean) As Boolean
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Friend Shared Function EnableWindow(hWnd As IntPtr, enabled As Boolean) As Boolean
    End Function

End Class