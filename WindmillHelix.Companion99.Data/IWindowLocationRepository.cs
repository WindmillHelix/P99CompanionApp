using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Data.Models;

namespace WindmillHelix.Companion99.Data
{
    public interface IWindowLocationRepository
    {
        WindowLocationModel GetByWindowName(string windowName);

        void SaveWindowLocation(WindowLocationModel model);
    }
}
