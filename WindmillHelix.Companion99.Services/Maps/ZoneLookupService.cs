using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Common;
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.Services.Maps
{
    public class ZoneLookupService : IZoneLookupService
    {
        private Dictionary<string, string> _longToShortMap 
            = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private Dictionary<string, ZoneInfo> _shortNameToZoneMap 
            = new Dictionary<string, ZoneInfo>(StringComparer.OrdinalIgnoreCase);

        public ZoneLookupService()
        {
            LoadMappings();
        }

        private void LoadMappings()
        {
            var json = ResourceHelper.GetResourceString("Resources.zones.json");

            var zones = JsonSerializer.Deserialize<ZoneInfo[]>(json);

            foreach(var zone in zones)
            {
                _shortNameToZoneMap.Add(zone.ShortName, zone);

                foreach (var longName in zone.LongNames)
                { 
                    _longToShortMap.Add(longName, zone.ShortName);
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
