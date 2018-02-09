using System;
using System.Runtime.InteropServices;

namespace vimage
{
    /// <summary>
    /// Desktop Window Manager
    /// </summary>
    class DWM
    {
        // Make Window Background Transparent
        [DllImport("dwmapi.dll")]
        public static extern void DwmEnableBlurBehindWindow(IntPtr hwnd, ref DWM_BLURBEHIND blurBehind);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        // Show/Hide in Taskbar
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private const int GWL_EX_STYLE = -20;
        private const int WS_EX_APPWINDOW = 0x00040000, WS_EX_TOOLWINDOW = 0x00000080;
        private static bool TaskbarIconVisible = true;

        public static void TaskBarIconSetVisible(IntPtr hWnd, bool visible)
        {
            TaskbarIconVisible = visible;
            if (TaskbarIconVisible)
                SetWindowLong(hWnd, GWL_EX_STYLE, (GetWindowLong(hWnd, GWL_EX_STYLE)) & ~WS_EX_TOOLWINDOW & ~WS_EX_APPWINDOW);
            else
                SetWindowLong(hWnd, GWL_EX_STYLE, (GetWindowLong(hWnd, GWL_EX_STYLE) | WS_EX_TOOLWINDOW) & ~WS_EX_APPWINDOW);
        }
        public static void TaskBarIconToggle(IntPtr hWnd) { TaskBarIconSetVisible(hWnd, !TaskbarIconVisible); }

        // Make Window Always On Top
        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        public static void SetAlwaysOnTop(IntPtr hWnd, bool alwaysOnTop = true)
        {
            if (alwaysOnTop)
            {
                SetWindowPos(hWnd, new IntPtr(-1), 0, 0, 0, 0, TOPMOST_FLAGS);
            }
            else
            {
                SetWindowPos(hWnd, new IntPtr(1), 0, 0, 0, 0, TOPMOST_FLAGS);
                SetWindowPos(hWnd, new IntPtr(0), 0, 0, 0, 0, TOPMOST_FLAGS);
            }
        }

    }

    [Flags]
    enum DWM_BB
    {
        Enable = 1,
        BlurRegion = 2,
        TransitionOnMaximized = 4
    }

    [StructLayout(LayoutKind.Sequential)]
    struct DWM_BLURBEHIND
    {
        public DWM_BB dwFlags;
        public bool fEnable;
        public IntPtr hRgnBlur;
        public bool fTransitionOnMaximized;
    }
}
