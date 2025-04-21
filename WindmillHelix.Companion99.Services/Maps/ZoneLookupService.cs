using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Common;

namespace WindmillHelix.Companion99.Services.Maps
{
    public class ZoneLookupService : IZoneLookupService
    {
        private Dictionary<string, string> _longToShortMap = new Dictionary<string, string>();

        public ZoneLookupService()
        {
            LoadMappings();
        }

        private void LoadMappings()
        {
            var zoneData = ResourceHelper.GetResourceString("Resources.zones.txt");
            zoneData = zoneData.Trim().Replace("\r\n", "\n");
            var lines = zoneData.Split('\n');

            foreach (var line in lines)
            {
                var firstIndex = line.IndexOf(" ");
                var secondIndex = line.IndexOf(" ", firstIndex + 1);
                var shortName = line.Substring(firstIndex + 1, secondIndex - firstIndex).Trim();
                var longName = line.Substring(secondIndex + 1).Trim().ToLowerInvariant();
                if (!_longToShortMap.ContainsKey(longName))
                {
                    _longToShortMap.Add(longName, shortName);
                }
            }
        }

        public string GetShortName(string longName)
        {
            if (!_longToShortMap.ContainsKey(longName.ToLowerInvariant()))
            {
                return null;
            }

            var result = _longToShortMap[longName.ToLowerInvariant()];
            return result;
        }
    }
}
