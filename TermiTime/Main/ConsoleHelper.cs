using System;
using System.Runtime.InteropServices;

// Not written by myself, used AI to generate this code.

public static class ConsoleHelper
{
    private const int MF_BYCOMMAND = 0x00000000;
    private const int SC_MAXIMIZE = 0xF030;
    private const int SC_SIZE = 0xF000;

    [DllImport("user32.dll")]
    private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

    [DllImport("user32.dll")]
    private static extern bool DeleteMenu(IntPtr hMenu, int uPosition, int uFlags);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    public static void DisableResizing()
    {
        IntPtr handle = GetConsoleWindow();
        IntPtr sysMenu = GetSystemMenu(handle, false);

        if (sysMenu != IntPtr.Zero)
        {
            DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND);
            DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);
        }
    }

    public static void DisableMaximizeOnly()
    {
        IntPtr handle = GetConsoleWindow();
        IntPtr sysMenu = GetSystemMenu(handle, false);
        if (sysMenu != IntPtr.Zero)
        {
            DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND);
        }
    }
}