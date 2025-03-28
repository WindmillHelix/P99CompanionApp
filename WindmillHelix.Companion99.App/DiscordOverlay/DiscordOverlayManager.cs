using DiscordOverlay;
using DiscordOverlay.Interop;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiscordOverlay;
using System.Drawing.Imaging;

namespace WindmillHelix.Companion99.App.DiscordOverlay
{
	public class DiscordOverlayManager
	{
		private bool _isStarted = false;

		private RenderForm _hostForm;
		private OverlayForm _overlayForm;

		private Bitmap _bitmapScreenshot;
		private GraphicsD3D11 _graphics;

		private PictureBox OverlayTarget => _overlayForm.OverlayTarget;

		Bitmap BitmapScreenshot
		{
			get => _bitmapScreenshot;
			set
			{
				_bitmapScreenshot = value;
				OverlayTarget.Image = value;
			}
		}

		public void Enable()
		{
			var start = new ThreadStart(() => DoEnable());
			var thread = new Thread(start);
			thread.Start();
		}

		private void DoEnable()
		{
			_hostForm = new HostForm();
			_hostForm.ShowInTaskbar = false;
			_hostForm.FormBorderStyle = FormBorderStyle.None;
			_hostForm.BackColor = Constants.DefaultTransparencyKey;

			_hostForm.Show();
			_hostForm.Size = new System.Drawing.Size(790, 640);

			_overlayForm = new OverlayForm();
			_overlayForm.Show();

			_graphics = new GraphicsD3D11();
			_graphics.Initialize(_hostForm, true);
			_overlayForm.Show();

			BitmapScreenshot = new Bitmap(_hostForm.Width, _hostForm.Height, PixelFormat.Format32bppArgb);
			_isStarted = true;

			RenderLoop.Run(_hostForm, RenderCallback, true);
		}

		public void SetRunMode()
		{
			if (_overlayForm != null)
			{
				_overlayForm.BeginInvoke(new Action(() => _overlayForm.SetRunMode(Constants.DefaultTransparencyKey)));
			}
		}

		public void SetResizeMode()
		{
			if (_overlayForm != null)
			{
				_overlayForm.BeginInvoke(new Action(() => _overlayForm.SetResizeMode()));
			}
		}

		public void Close()
		{
			_overlayForm?.Close();
			_hostForm?.Close();
		}

		public void Start()
		{
			if (!_isStarted)
			{
				Enable();
				SetRunMode();
			}
		}

		public void Disable()
		{
			throw new NotImplementedException();
		}

		private void RenderCallback()
		{
			var stopWatch = Stopwatch.StartNew();
			if (!_overlayForm.IsInResizeMode)
			{
				Draw();
				var gfxScreenshot = Graphics.FromImage(BitmapScreenshot);
				IntPtr dc = gfxScreenshot.GetHdc();
				User32Methods.PrintWindow(_hostForm.Handle, dc, 0);
				gfxScreenshot.ReleaseHdc();
				gfxScreenshot.Dispose();
				OverlayTarget.Invalidate();
			}

			// Calculate Frame Limiting
			stopWatch.Stop();
			long drawingTime = stopWatch.ElapsedTicks;

			// todo: framerate
			int frameRate = 25;
			if (frameRate > 0)
			{
				int framerateTicks = 10000000 / frameRate;
				long duration = framerateTicks - drawingTime;
				if (duration > 0)
				{
					Thread.Sleep(new TimeSpan(duration));
				}
			}
		}

		private void Draw()
		{
			_graphics.ClearRenderTargetView();
			_graphics.PresentSwapChain();
		}
	}
}