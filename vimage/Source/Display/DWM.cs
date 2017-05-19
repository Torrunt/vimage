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
