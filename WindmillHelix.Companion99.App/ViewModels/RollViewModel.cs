using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.App.ViewModels;

namespace WindmillHelix.Companion99.App.ViewModels
{
    public class RollViewModel
    {
        public RollSetViewModel RollSet { get; set; }

        public string CharacterName { get; set; }

        public DateTime RollTime { get; set; }

        public int Value { get; set; }

        public string TimeAfterFirstRoll
        {
            get
            {
                var span = RollTime - RollSet.FirstRoll;
                var seconds = Math.Round(span.TotalSeconds, 0);
                return $"{seconds}s";
            }
        }
    }
}
