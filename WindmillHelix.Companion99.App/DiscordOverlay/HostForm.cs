using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindmillHelix.Companion99.App.DiscordOverlay.Interop;

namespace DiscordOverlay
{
    public class HostForm : RenderForm
    {
        public HostForm()
            : base(Constants.WindowTitle)
        {
            SetDefaults();
            SetupControls();
            SetupEvents();
        }

        private void SetDefaults()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = false;
            this.Opacity = 0;
        }

        private void SetupControls()
        {
        }

        private void SetupEvents()
        {
        }

        protected override void WndProc(ref Message message)
        {
            if (message.Msg == InteropConstants.WM_NCHITTEST)
            {
                message.Result = (IntPtr)InteropConstants.HTTRANSPARENT;
            }
            else
            {
                base.WndProc(ref message);
            }
        }
    }
}
