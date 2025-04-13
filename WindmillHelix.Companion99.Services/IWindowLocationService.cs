using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.Services
{
    public interface IWindowLocationService
    {
        WindowLocation GetWindowLocation(string windowName);

        void SaveLocation(string windowName, Point location, Size? size);
    }
}
