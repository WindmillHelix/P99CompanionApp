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
using WindmillHelix.Companion99.Services;

namespace WindmillHelix.Companion99.App.DiscordOverlay
{
	public class DiscordOverlayManager
	{
		private readonly ILogService _logService;

		private bool _isStarted = false;

		private RenderForm _hostForm;
		private OverlayForm _overlayForm;

		private Bitmap _bitmapScreenshot;
		private GraphicsD3D11 _graphics;

		private PictureBox OverlayTarget => _overlayForm.OverlayTarget;
		private Thread _thread;

        public DiscordOverlayManager(ILogService logService)
        {
			_logService = logService;
        }

		Bitmap BitmapScreenshot
		{
			get => _bitmapScreenshot;
			set
			{
				_bitmapScreenshot = value;
				OverlayTarget.Image = value;
			}
		}

		public void Enable(Mode mode)
		{
			var start = new ThreadStart(() => DoEnable(mode));
			_thread = new Thread(start);
			_thread.Start();
		}

		private void DoEnable(Mode mode)
		{
			Thread.CurrentThread.IsBackground = true;
			while (true)
			{
				try
				{
					if (_hostForm == null || _hostForm.IsDisposed)
					{
						_hostForm = new HostForm();
					}

					_hostForm.ShowInTaskbar = false;
					_hostForm.FormBorderStyle = FormBorderStyle.None;
					_hostForm.BackColor = Constants.DefaultTransparencyKey;

					_hostForm.Show();
					_hostForm.Size = new System.Drawing.Size(790, 640);

					if (_overlayForm == null || _overlayForm.IsDisposed)
					{
						_overlayForm = new OverlayForm();
					}

					_overlayForm.Show();

					_graphics = new GraphicsD3D11();
					_graphics.Initialize(_hostForm, true);
					_overlayForm.Show();

					BitmapScreenshot = new Bitmap(_hostForm.Width, _hostForm.Height, PixelFormat.Format32bppArgb);
					_isStarted = true;

					SetMode(mode);
					RenderLoop.Run(_hostForm, RenderCallback, true);
				}
				catch (ThreadAbortException)
				{
					_logService.Log("Caught ThreadAbortException");
					return;
				}
				catch(ThreadInterruptedException)
                {
					_logService.Log("Caught ThreadInterruptedException");
					return;
                }
				catch (Exception thrown)
				{
					_logService.LogException(thrown);
					Thread.Sleep(6000);
				}
			}
		}

		private void SetMode(Mode mode)
        {
            switch (mode)
            {
                case Mode.Run:
					SetRunMode();
                    break;
                case Mode.Resize:
					SetResizeMode();
                    break;
                default:
					throw new ArgumentOutOfRangeException(nameof(mode));
            }
        }

        public void SetRunMode()
		{
			_logService.Log("Invoked");
			if (_overlayForm != null)
			{
				_logService.Log("Setting run mode on overlay form");
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
			_thread?.Interrupt();
			_overlayForm?.Invoke(() => _overlayForm.Close());
			_hostForm?.Invoke(() => _hostForm.Close());
		}

		public void Start()
		{
			if (!_isStarted)
			{
				Enable(Mode.Run);
			}
			else
            {
				SetRunMode();
            }
		}

		public void Disable()
		{
			throw new NotImplementedException();
		}

		private void RenderCallback()
		{
			try
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
			catch(ThreadAbortException)
            {
				return;
            }
			catch (Exception thrown)
            {
				_logService.LogException(thrown);
				Thread.Sleep(2000);
            }
		}

		private void Draw()
		{
			_graphics.ClearRenderTargetView();
			_graphics.PresentSwapChain();
		}
    }
}