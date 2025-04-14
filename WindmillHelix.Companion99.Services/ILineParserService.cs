using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.Services
{
    public interface ILineParserService
    {
        string GetZoneNameFromResultsEndcap(string line);

        WhoResult ParseWhoResultLine(string line, DateTime eventDate, string serverName, string defaultZone);
    }
}
