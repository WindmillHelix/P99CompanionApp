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
                SetNewZone(zoneShortName);
            }
            else if (line.StartsWith(locationPrefix))
            {
                var location = GetLocation(line, locationPrefix);
                SetNewZone(CurrentLocation?.ZoneShortName, location);
            }
            else if (line.StartsWith("There are ") 
                && line.Contains(" players in ") 
                && !line.EndsWith(" EverQuest.") 
                && !line.StartsWith("There are no players "))
            {
                var zone = line.Substring(line.IndexOf(" players in ") +  12).TrimEnd('.');
                var zoneShortName = _zoneLookupService.GetShortName(zone);
                if(CurrentLocation?.ZoneShortName != zoneShortName)
                {
                    SetNewZone(zoneShortName);
                }
            }
            else if(
                line.StartsWith("There is") 
                && line.Contains(" player in ") 
                && !line.EndsWith(" EverQuest.")
                && !line.StartsWith("There are no players "))
            {
                var zone = line.Substring(line.IndexOf(" player in ") + 11).TrimEnd('.');
                var zoneShortName = _zoneLookupService.GetShortName(zone);
                if (CurrentLocation?.ZoneShortName != zoneShortName)
                {
                    SetNewZone(zoneShortName);
                }
            }
        }

        private void SetNewZone(string zoneShortName, Location location = null)
        {
            var newLocation = new CurrentLocation
            {
                ZoneShortName = zoneShortName,
                Location = location
            };

            CurrentLocation = newLocation;
            _eventService.Raise<CurrentLocation>(newLocation);
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
