using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Services.Models
{
    public class ZoneInfo
    {
        public ZoneInfo()
        {
            LongNames = new List<string>();
        }

        public string ShortName { get; set; }

        public List<string> LongNames { get; set; }
    }
}
