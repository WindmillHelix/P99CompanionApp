using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Services.Models.Maps;

namespace WindmillHelix.Companion99.Services.Maps
{
    public class CurrentLocationService : ICurrentLocationService, ILogListener
    {
        private readonly IEventService _eventService;
        private readonly IZoneLookupService _zoneLookupService;

        public CurrentLocation CurrentLocation { get; private set; }

        public CurrentLocationService(
            IEventService eventService, 
            IZoneLookupService zoneLookupService,
            ILogReaderService logReaderService)
        {
            _eventService = eventService;
            _zoneLookupService = zoneLookupService;
            logReaderService.AddListener(this);
        }

        public void HandleLogLine(string serverName, string characterName, DateTime eventDate, string line)
        {
            const string locationPrefix = "Your Location is ";
            const string zonedPrefix = "You have entered ";
            if (line.StartsWith(zonedPrefix))
            {
                var zone = line.Substring(zonedPrefix.Length).TrimEnd('.');
                var zoneShortName = _zoneLookupService.GetShortName(zone);
                var newLocation = new CurrentLocation
                {
                    ZoneShortName = zoneShortName,
                    Location = null
                };

                CurrentLocation = newLocation;
                _eventService.Raise<CurrentLocation>(newLocation);
            }
            else if(line.StartsWith(locationPrefix))
            {
                var location = GetLocation(line, locationPrefix);
                var newLocation = new CurrentLocation
                {
                    ZoneShortName = CurrentLocation?.ZoneShortName,
                    Location = location
                };

                CurrentLocation = newLocation;
                _eventService.Raise<CurrentLocation>(newLocation);
            }
        }

        private Location GetLocation(string line, string prefix)
        {
            var coordinates = line.Substring(prefix.Length);

            var parts = coordinates.Split(',');
            if (parts.Length == 3)
            {
                var x = double.Parse(parts[1]);
                var y = double.Parse(parts[0]);
                var z = double.Parse(parts[2]);

                return new Location
                {
                    X = x,
                    Y = y,
                    Z = z
                };
            }

            return null;
        }
    }
}
