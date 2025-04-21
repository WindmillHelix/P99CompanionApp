using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Services.Models.Maps
{
    public class Location
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public override bool Equals(object obj)
        {
            var toCompare = obj as Location;
            if (toCompare == null)
            {
                return false;
            }

            return this.X == toCompare.X && this.Y == toCompare.Y && this.Z == toCompare.Z;
        }
    }
}
