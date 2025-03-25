using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Services.Models.Maps;

namespace WindmillHelix.Companion99.Services.Maps
{
    public interface IMapService
    {
        Map GetMap(string zoneShortName);
    }
}
