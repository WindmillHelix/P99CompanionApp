using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WindmillHelix.Companion99.App.Maps
{
    public class MapElement
    {
        public string LayerName { get; set; }

        public MapElementType MapElementType { get; set; }

        public string FilterText { get; set; }
        
        public FrameworkElement[] Elements { get; set; }
    }
}
