using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.Services
{
    public class LineParserService : ILineParserService
    {
        private readonly INoteService _noteService;

        public LineParserService(INoteService noteService)
        {
            _noteService = noteService;
        }

        public string GetZoneNameFromResultsEndcap(string line)
        {
            if(!line.StartsWith("There are no ") && !line.StartsWith("There are ") && !line.StartsWith("There is "))
            {
                return null;
            }

            const string playersIn = " players in ";
            line = line.Replace(" player in ", playersIn);
            var index = line.IndexOf(playersIn);
            var zone = line.Substring(index + playersIn.Length).TrimEnd('.');
            if(zone == "EverQuest")
            {
                return null;
            }

            return zone;
        }

        public WhoResult ParseWhoResultLine(string line, DateTime eventDate, string serverName, string defaultZone)
        {
            var result = new WhoResult();
            result.ServerName = serverName;
            result.EventDate = eventDate;
            result.OriginalLine = line;

            if(line.StartsWith(" <LINKDEAD>"))
            {
                result.LinkDead = "LINKDEAD";
                line = line.Substring(11);
            }

            if(line.StartsWith(" AFK "))
            {
                result.AwayFromKeyboard = "AFK";
                line = line.Substring(5);
            }

            if(line.EndsWith(" LFG"))
            {
                result.LookingForGroup = "LFG";
                line = line.Substring(0, line.Length - 4);
            }

            if(!line.StartsWith("["))
            {
                return null;
            }

            if(line.StartsWith("[ANONYMOUS]"))
            {
                result.Class = "Unknown";
                result.Level = null;
            }
            else
            {
                var parts = line.Substring(1, line.IndexOf(']') - 1).Split(' ');
                result.Level = int.Parse(parts[0]);
                result.Class = GetActualClassName(parts[1]);
            }

            var closeIndex = line.IndexOf(']');
            var nameParts = line.Substring(closeIndex + 2).Split(' '); ;
            result.Name = nameParts[0];

            var raceIndex = line.IndexOf("(");
            if (raceIndex > -1)
            {
                result.Race = line.Substring(raceIndex + 1, line.IndexOf(")") - raceIndex - 1);
            }

            var guildIndex = line.IndexOf("<");
            if (guildIndex > -1)
            {
                result.Guild = line.Substring(guildIndex + 1, line.IndexOf(">") - guildIndex - 1);
            }

            var zoneIndex = line.IndexOf("ZONE: ");
            if (zoneIndex > -1)
            {
                result.ZoneName = line.Substring(zoneIndex + 6);
            }
            else
            {
                result.ZoneName = defaultZone;
            }

            result.Note = _noteService.GetNote(serverName, result.Name);
            return result;
        }

        private string GetActualClassName(string value)
        {
            switch (value)
            {
                case "Minstrel":
                case "Troubadour":
                case "Virtuoso":
                    return "Bard";
                case "Vicar":
                case "Templar":
                case "High Priest":
                case "High":
                    return "Cleric";
                case "Wanderer":
                case "Preserver":
                case "Hierophant":
                    return "Druid";
                case "Illusionist":
                case "Beguiler":
                case "Phantasmist":
                    return "Enchanter";
                case "Elementalist":
                case "Conjurer":
                case "Arch Mage":
                case "Arch": // hack to avoid parsing out correctly
                    return "Magician";
                case "Disciple":
                case "Master":
                case "Grandmaster":
                    return "Monk";
                case "Heretic":
                case "Defiler":
                case "Warlock":
                    return "Necromancer";
                case "Cavalier":
                case "Knight":
                case "Crusader":
                    return "Paladin";
                case "Pathfinder":
                case "Outrider":
                case "Warder":
                    return "Ranger";
                case "Rake":
                case "Blackguard":
                case "Assassin":
                    return "Rogue";
                case "Reaver":
                case "Revenant":
                case "Grave Lord":
                case "Grave":
                    return "Shadow Knight";
                case "Mystic":
                case "Luminary":
                case "Oracle":
                    return "Shaman";
                case "Champion":
                case "Myrmidon":
                case "Warlord":
                    return "Warrior";
                case "Channeler":
                case "Evoker":
                case "Sorcerer":
                    return "Wizard";
                default:
                    return value;
            }
        }
    }
}
