using System;
using System.Runtime.InteropServices;
using SFML.Graphics;
using SFML.System;

namespace vimage
{
    /// <summary>
    /// Desktop Window Manager
    /// </summary>
    internal class DWM
    {
        // Make Window Background Transparent
        [DllImport("dwmapi.dll")]
        public static extern void DwmEnableBlurBehindWindow(
            IntPtr hwnd,
            ref DWM_BLURBEHIND blurBehind
        );

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect
        );

        // Show/Hide in Taskbar
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        private const int GWL_EX_STYLE = -20;
        private const uint WS_EX_APPWINDOW = 0x00040000,
            WS_EX_TOOLWINDOW = 0x00000080;
        public static bool TaskbarIconVisible = true;

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_HIDE = 0x00;
        private const int SW_SHOW = 0x05;

        public static void TaskBarIconSetVisible(IntPtr hWnd, bool visible)
        {
            TaskbarIconVisible = visible;
            bool isOlderThanWindows8 = Environment.OSVersion.Version < new Version(6, 2);

            if (TaskbarIconVisible)
            {
                _ = SetWindowLong(
                    hWnd,
                    GWL_EX_STYLE,
                    (GetWindowLong(hWnd, GWL_EX_STYLE)) & ~WS_EX_TOOLWINDOW & ~WS_EX_APPWINDOW
                );
            }
            else
            {
                if (isOlderThanWindows8)
                    _ = ShowWindow(hWnd, SW_HIDE); // Makes hiding possible in Windows Vista/7
                _ = SetWindowLong(
                    hWnd,
                    GWL_EX_STYLE,
                    (GetWindowLong(hWnd, GWL_EX_STYLE) | WS_EX_TOOLWINDOW) & ~WS_EX_APPWINDOW
                ); // Hiding TaskBar-Icon
                if (isOlderThanWindows8)
                    _ = ShowWindow(hWnd, SW_SHOW); // Makes hiding possible in Windows Vista/7
            }
        }

        // Toggle Borderless / Title bar
        private const int GWL_STYLE = -16;
        public const uint WS_CAPTION = 0x00C00000,
            WS_SYSMENU = 0x00080000,
            WS_POPUP = 0x80000000;
        private const uint SWP_FRAMECHANGED = 0x0020;
        private static bool SysMenuVisible = true;

        public static void SysMenuSetVisible(RenderWindow window, bool visible)
        {
            if (SysMenuVisible == visible)
                return;
            SysMenuVisible = visible;
            _ = SysMenuVisible
                ? SetWindowLong(
                    window.SystemHandle,
                    GWL_STYLE,
                    GetWindowLong(window.SystemHandle, GWL_STYLE) | WS_SYSMENU
                )
                : SetWindowLong(
                    window.SystemHandle,
                    GWL_STYLE,
                    GetWindowLong(window.SystemHandle, GWL_STYLE) & ~WS_SYSMENU
                );

            _ = SetWindowPos(
                window.SystemHandle,
                new IntPtr(0),
                window.Position.X,
                window.Position.Y,
                (int)window.Size.X,
                (int)window.Size.Y,
                SWP_FRAMECHANGED
            );
        }

        public static void TitleBarSetVisible(RenderWindow window, bool visible)
        {
            _ = visible
                ? SetWindowLong(
                    window.SystemHandle,
                    GWL_STYLE,
                    GetWindowLong(window.SystemHandle, GWL_STYLE) | WS_CAPTION | WS_SYSMENU
                )
                : SetWindowLong(
                    window.SystemHandle,
                    GWL_STYLE,
                    GetWindowLong(window.SystemHandle, GWL_STYLE) & ~WS_CAPTION
                );

            _ = SetWindowPos(
                window.SystemHandle,
                new IntPtr(0),
                window.Position.X,
                window.Position.Y,
                (int)window.Size.X,
                (int)window.Size.Y,
                SWP_FRAMECHANGED
            );
        }

        public static void PreventExlusiveFullscreen(RenderWindow window)
        {
            _ = SetWindowLong(
                window.SystemHandle,
                GWL_STYLE,
                GetWindowLong(window.SystemHandle, GWL_STYLE) & ~WS_POPUP
            );
        }

        // Window/Client Rect/Pos - used for Title Bar support
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public struct Point
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll")]
        private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        public static RECT GetClientRect(IntPtr hWnd)
        {
            _ = GetClientRect(hWnd, out RECT result);
            return result;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        public static RECT GetWindowRect(IntPtr hWnd)
        {
            _ = GetWindowRect(hWnd, out RECT result);
            return result;
        }

        [DllImport("user32.dll")]
        private static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        public static Vector2i ClientToScreen(IntPtr hWnd, int x, int y)
        {
            Point result = new Point() { x = x, y = y };
            _ = ClientToScreen(hWnd, ref result);
            return new Vector2i(result.x, result.y);
        }

        public static Vector2i GetWindowClientPos(IntPtr hWnd)
        {
            RECT rect = GetClientRect(hWnd);
            return ClientToScreen(hWnd, rect.Left, rect.Top);
        }

        public static Vector2i GetTitleBarDifference(IntPtr hWnd)
        {
            RECT rect = GetWindowRect(hWnd);
            Vector2i cp = GetWindowClientPos(hWnd);
            return new Vector2i(cp.X - rect.Left, cp.Y - rect.Top);
        }

        // Make Window Always On Top
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int X,
            int Y,
            int cx,
            int cy,
            uint uFlags
        );

        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        public static void SetAlwaysOnTop(IntPtr hWnd, bool alwaysOnTop = true)
        {
            if (alwaysOnTop)
            {
                _ = SetWindowPos(hWnd, new IntPtr(-1), 0, 0, 0, 0, TOPMOST_FLAGS);
            }
            else
            {
                _ = SetWindowPos(hWnd, new IntPtr(1), 0, 0, 0, 0, TOPMOST_FLAGS);
                _ = SetWindowPos(hWnd, new IntPtr(0), 0, 0, 0, 0, TOPMOST_FLAGS);
            }
        }

        // Make Window Click-through-able
        private const uint WS_EX_TRANSPARENT = 0x00000020,
            WS_EX_LAYERED = 0x00080000;

        public static void SetClickThroughAble(IntPtr hWnd, bool canClickThrough = true)
        {
            _ = canClickThrough
                ? SetWindowLong(
                    hWnd,
                    GWL_EX_STYLE,
                    (GetWindowLong(hWnd, GWL_EX_STYLE)) | WS_EX_LAYERED | WS_EX_TRANSPARENT
                )
                : SetWindowLong(
                    hWnd,
                    GWL_EX_STYLE,
                    (GetWindowLong(hWnd, GWL_EX_STYLE)) & ~WS_EX_LAYERED & ~WS_EX_TRANSPARENT
                );
        }
    }

    [Flags]
    internal enum DWM_BB
    {
        Enable = 1,
        BlurRegion = 2,
        TransitionOnMaximized = 4,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DWM_BLURBEHIND
    {
        public DWM_BB dwFlags;
        public bool fEnable;
        public IntPtr hRgnBlur;
        public bool fTransitionOnMaximized;
    }
}
