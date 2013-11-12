using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace vimage
{
    class DWM
    {
        [DllImport("dwmapi.dll")]
        public static extern void DwmEnableBlurBehindWindow(IntPtr hwnd, ref DWM_BLURBEHIND blurBehind);
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

        public DWM_BLURBEHIND(bool enabled)
        {
            fEnable = enabled ? true : false;
            hRgnBlur = IntPtr.Zero;
            fTransitionOnMaximized = false;
            dwFlags = DWM_BB.Enable;
        }

        public System.Drawing.Region Region
        {
            get { return System.Drawing.Region.FromHrgn(hRgnBlur); }
        }

        public bool TransitionOnMaximized
        {
            get { return fTransitionOnMaximized; }
            set
            {
                fTransitionOnMaximized = value ? true : false;
                dwFlags |= DWM_BB.TransitionOnMaximized;
            }
        }

        public void SetRegion(System.Drawing.Graphics graphics, System.Drawing.Region region)
        {
            hRgnBlur = region.GetHrgn(graphics);
            dwFlags |= DWM_BB.BlurRegion;
        }
    }
}
