using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Services.Models.Maps
{
    public class MapLayer
    {
        public string Name { get; set; }

        public IReadOnlyCollection<Line> Lines { get; set; }

        public IReadOnlyCollection<Point> Points { get; set; }
    }
}
