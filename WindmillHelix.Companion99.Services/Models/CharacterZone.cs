using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WindmillHelix.Companion99.Services.Models
{
    public class CharacterZone
    {
        public string ServerName { get; set; }

        public string CharacterName { get; set; }

        public string ZoneName { get; set; }

        public string Account { get; set; }

        public DateTime? SkyCorpseDate { get; set; }

        public string BindZone { get; set; }

        public bool IsIgnored { get; set; }
    }
}
