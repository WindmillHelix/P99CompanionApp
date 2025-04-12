using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.Services
{
    public interface ILastZoneService
    {
        void SetLastZone(string serverName, string characterName, string zoneName, string account);

        void SetBindPoint(string serverName, string characterName, string bindZone);

        void SetSkyCorpseDate(string serverName, string characterName, DateTime dateOfDeath);

        void SetIgnored(string serverName, string characterName, bool isIgnored);

        void RemoveEntry(string serverName, string characterName);

        IReadOnlyCollection<CharacterZone> GetLastZones();
    }
}
