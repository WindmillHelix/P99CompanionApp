using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Data;
using WindmillHelix.Companion99.Data.Models;
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.Services
{
    public class WindowLocationService : IWindowLocationService
    {
        private readonly IWindowLocationRepository _windowLocationRepository;

        public WindowLocationService(IWindowLocationRepository windowLocationRepository)
        {
            _windowLocationRepository = windowLocationRepository;
        }

        public WindowLocation GetWindowLocation(string windowName)
        {
            var item = _windowLocationRepository.GetByWindowName(windowName);
            if(item == null)
            {
                return null;
            }

            var result = new WindowLocation
            {
                Location = item.Location,
                Size = item.Size
            };

            return result;
        }

        public void SaveLocation(string windowName, Point location, Size? size)
        {
            var item = new WindowLocationModel
            {
                Location = location,
                Size = size,
                WindowName = windowName
            };

            _windowLocationRepository.SaveWindowLocation(item);
        }
    }
}
