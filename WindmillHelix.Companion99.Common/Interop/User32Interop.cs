using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Common.Interop
{
    public static class User32Interop
    {
        [DllImport("User32")]
        public static extern int SetForegroundWindow(IntPtr hwnd);

        [DllImport("User32")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}
