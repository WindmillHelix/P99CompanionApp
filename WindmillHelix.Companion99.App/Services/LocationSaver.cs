using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WindmillHelix.Companion99.Services;

namespace WindmillHelix.Companion99.App.Services
{
    public class LocationSaver
    {
        private readonly IWindowLocationService _windowLocationService;

        private Window _window;
        private string _windowName;

        public LocationSaver(IWindowLocationService windowLocationService)
        {
            _windowLocationService = windowLocationService;
        }

        public void Attach(Window window)
        {
            if(_window != null)
            {
                throw new Exception("This LocationSaver has already been attached to another window");
            }

            _window = window;
            _windowName = window.GetType().Name;

            var savedLocation = _windowLocationService.GetWindowLocation(_windowName);
            if (savedLocation != null)
            {
                _window.WindowStartupLocation = WindowStartupLocation.Manual;
                if (savedLocation.Size != null)
                {
                    _window.Width = savedLocation.Size.Value.Width;
                    _window.Height = savedLocation.Size.Value.Height;
                }

                _window.Left = savedLocation.Location.X;
                _window.Top = savedLocation.Location.Y;
            }
            else
            {
                _window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            _window.SizeChanged += HandleSizeChanged;
            _window.LocationChanged += HandleLocationChanged;
        }

        private void HandleLocationChanged(object? sender, EventArgs e)
        {
            SaveLocation();
        }

        private void HandleSizeChanged(object sender, SizeChangedEventArgs e)
        {
            SaveLocation();
        }

        private void SaveLocation()
        {
            var canBeResized = _window.ResizeMode == ResizeMode.CanResize || _window.ResizeMode == ResizeMode.CanResizeWithGrip;

            var location = new System.Drawing.Point((int)_window.Left, (int)_window.Top);
            var size = new System.Drawing.Size((int)_window.Width, (int)_window.Height);
            _windowLocationService.SaveLocation(_windowName, location, canBeResized ? size : null);
        }
    }
}
