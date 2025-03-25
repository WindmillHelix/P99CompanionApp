using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Services.Maps
{
    public interface IZoneLookupService
    {
        string GetShortName(string longName);
    }
}
