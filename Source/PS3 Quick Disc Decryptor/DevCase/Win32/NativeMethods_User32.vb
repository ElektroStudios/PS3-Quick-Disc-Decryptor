Option Strict On
Option Explicit On
Option Infer Off

Imports System.Runtime.InteropServices
Imports System.Runtime.Versioning
Imports System.Security

Namespace DevCase.Win32.NativeMethods

    ''' <summary>
    ''' Platform Invocation methods (P/Invoke), access unmanaged code.
    ''' <para></para>
    ''' User32.dll.
    ''' </summary>
    <HideModuleName>
    <SuppressUnmanagedCodeSecurity>
    <SupportedOSPlatform("windows")>
    Friend Module User32

#Region " User32.dll "

        ''' <summary>
        ''' Changes the parent window of the specified child window.
        ''' </summary>
        '''
        ''' <remarks>
        ''' <see href="https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-setparent"/>
        ''' </remarks>
        '''
        ''' <param name="hWndChild">
        ''' A handle to the child window.
        ''' </param>
        '''
        ''' <param name="hWndNewParent">
        ''' A handle to the new parent window. If this parameter is <see cref="IntPtr.Zero"/>, 
        ''' the desktop window becomes the new parent window. 
        ''' <para></para>
        ''' If this parameter is HWND_MESSAGE, the child window becomes a message-only window.
        ''' </param>
        '''
        ''' <returns>
        ''' If the function succeeds, the return value is <see langword="True"/>.
        ''' <para></para>
        ''' If the function fails, the return value is <see langword="False"/>.
        ''' <para></para>
        ''' To get extended error information, call <see cref="Marshal.GetLastWin32Error"/>. 
        ''' </returns>
        <DllImport(Win32LibNames.User32, SetLastError:=True, ExactSpelling:=True)>
        Friend Function SetParent(hWndChild As IntPtr,
                                  hWndNewParent As IntPtr
        ) As IntPtr
        End Function

        ''' <summary>
        ''' Changes the position and dimensions of the specified window.
        ''' <para></para>
        ''' For a top-level window, the position and dimensions are relative to the upper-left corner of the screen.
        ''' <para></para>
        ''' For a child window, they are relative to the upper-left corner of the parent window's client area.
        ''' </summary>
        '''
        ''' <remarks>
        ''' <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms633534%28v=vs.85%29.aspx"/>
        ''' </remarks>
        '''
        ''' <param name="hWnd">
        ''' A handle to the window.
        ''' </param>
        ''' 
        ''' <param name="x">
        ''' The new position of the left side of the window.
        ''' </param>
        ''' 
        ''' <param name="y">
        ''' The new position of the top of the window.
        ''' </param>
        ''' 
        ''' <param name="width">
        ''' The new width of the window.
        ''' </param>
        ''' 
        ''' <param name="height">
        ''' The new height of the window.
        ''' </param>
        ''' 
        ''' <param name="repaint">
        ''' Indicates whether the window is to be repainted.
        ''' <para></para>
        ''' If this parameter is <see langword="True"/>, the window receives a message. 
        ''' If the parameter is <see langword="False"/>, no repainting of any kind occurs.
        ''' <para></para>
        ''' This applies to the client area, the nonclient area (including the title bar and scroll bars), 
        ''' and any part of the parent window uncovered as a result of moving a child window.
        ''' </param>
        '''
        ''' <returns>
        ''' <see langword="True"/> if the function succeeds, <see langword="False"/> otherwise.
        ''' </returns>
        <DllImport(Win32LibNames.User32, SetLastError:=True)>
        Friend Function MoveWindow(hWnd As IntPtr,
                                   x As Integer, y As Integer,
                                   width As Integer, height As Integer,
   <MarshalAs(UnmanagedType.Bool)> repaint As Boolean
        ) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        ''' <summary>
        ''' Enables or disables mouse and keyboard input to the specified window or control.
        ''' <para></para>
        ''' When input is disabled, the window does not receive input such as mouse clicks and key presses. 
        ''' <para></para>
        ''' When input is enabled, the window receives all input.
        ''' </summary>
        '''
        ''' <remarks>
        ''' <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms646291%28v=vs.85%29.aspx"/>
        ''' </remarks>
        '''
        ''' <param name="hWnd">
        ''' A handle to the window to be enabled or disabled. 
        ''' </param>
        ''' 
        ''' <param name="enable">
        ''' Indicates whether to enable or disable the window. 
        ''' <para></para>
        ''' If this parameter is <see langword="True"/>, the window is enabled. 
        ''' <para></para>
        ''' If the parameter is <see langword="False"/>, the window is disabled. 
        ''' </param>
        '''
        ''' <returns>
        ''' If the the window is enabled, the return value is <see langword="True"/>.
        ''' <para></para>
        ''' If the window is not enabled, the return value is <see langword="False"/>.
        ''' </returns>
        <DllImport(Win32LibNames.User32, SetLastError:=True)>
        Friend Function EnableWindow(hWnd As IntPtr,
     <MarshalAs(UnmanagedType.Bool)> enable As Boolean
        ) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

#End Region

    End Module

End Namespace
