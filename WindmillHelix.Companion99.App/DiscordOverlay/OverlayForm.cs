using DiscordOverlay.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindmillHelix.Companion99.App;
using WindmillHelix.Companion99.App.DiscordOverlay.Interop;
using WindmillHelix.Companion99.Services;

namespace DiscordOverlay
{
	public class OverlayForm : Form
	{
		private IConfigurationService _configurationService;

		private PictureBox _overlayTarget;

		public bool IsInResizeMode { get; private set; }

		public OverlayForm() : base()
		{
			_configurationService = DependencyInjector.Resolve<IConfigurationService>();
			DoubleBuffered = true;
			SetDefaults();

			SetupControls();
			SetRunMode(Constants.DefaultTransparencyKey);

			SetupEvents();
		}

		public PictureBox OverlayTarget
        {
			get { return _overlayTarget; }
        }

		private void SetDefaults()
        {
			this.Text = string.Empty;
			this.Name = "Discord Overlay";

			this.MinimumSize = new Size(100, 50);
			this.AllowTransparency = true;
			this.BackColor = Color.MediumBlue;

			this.ShowIcon = false;
			this.MinimizeBox = false;
			this.MaximizeBox = false;
			this.ControlBox = false;
			this.BackgroundImageLayout = ImageLayout.None;
			this.FormBorderStyle = FormBorderStyle.SizableToolWindow;

			this.ShowInTaskbar = false;
			this.ControlBox = false;
			this.TopMost = true;
		}

		private void SetupControls()
        {
			_overlayTarget = new PictureBox();
			_overlayTarget.SizeMode = PictureBoxSizeMode.Normal;
			_overlayTarget.BackColor = Constants.DefaultTransparencyKey;
			_overlayTarget.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			_overlayTarget.Dock = DockStyle.Fill;
		}

		private void SetupEvents()
        {
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

			this.Size = new Size(_configurationService.DiscordOverlaySize);
			this.Location = _configurationService.DiscordOverlayLocation;

			this.SizeChanged += OverlayForm_SizeChanged;
			this.LocationChanged += OverlayForm_LocationChanged;
		}

		private void OverlayForm_LocationChanged(object? sender, EventArgs e)
        {
            _configurationService.DiscordOverlayLocation = this.Location;
        }

        private void OverlayForm_SizeChanged(object? sender, EventArgs e)
        {
			_configurationService.DiscordOverlaySize = new Point(this.Size);
        }

        public void SetRunMode(Color transparencyColor)
		{
			IsInResizeMode = false;
			this.AllowTransparency = true;
			this.FormBorderStyle = FormBorderStyle.None;
			this.BackColor = transparencyColor;
			this.TransparencyKey = transparencyColor;
			this.ControlBox = false;

			this.Controls.Clear();
			this.Controls.Add(_overlayTarget);

			if (!this.Controls.Contains(_overlayTarget))
            {
				this.Controls.Add(_overlayTarget);
            }
		}

		public void SetResizeMode()
		{
			IsInResizeMode = true;
			this.AllowTransparency = false;
			this.Opacity = 1;
			this.BackColor = Color.DarkBlue;
			this.FormBorderStyle = FormBorderStyle.Sizable;
			this.ControlBox = true;

			this.Controls.Clear();

			var label = new Label();
			label.Dock = DockStyle.Fill;
			label.ForeColor = Color.White;
			label.Text = "Move and resize this window to where the overlay should appear";
			this.Controls.Add(label);
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams createParams = base.CreateParams;
				createParams.ClassStyle = createParams.ClassStyle | InteropConstants.CP_NOCLOSE_BUTTON;
				return createParams;
			}
		}
	}
}
