using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DiscordOverlay.Interop
{
    internal static class User32Methods
    {
		[DllImport("user32")]
		public static extern int PrintWindow(IntPtr hwnd, IntPtr hdcBlt, UInt32 nFlags);
	}
}
