using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordOverlay
{
    internal static class ExtensionMethods
    {
        public static SharpDX.Color ToSharpDXColor(this Color color)
        {
            var result = new SharpDX.Color(color.R, color.G, color.B, color.A);
            return result;
        }
    }
}