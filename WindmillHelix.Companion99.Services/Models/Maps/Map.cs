using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Services.Models.Maps
{
    public class Map
    {
        public IReadOnlyCollection<MapLayer> Layers { get; set; }
    }
}
