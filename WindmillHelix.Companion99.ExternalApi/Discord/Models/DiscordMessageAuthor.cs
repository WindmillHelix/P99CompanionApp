using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.ExternalApi.Discord.Models
{
    public class DiscordMessageAuthor
    {
        [JsonPropertyName("username")]
        public string UserName { get; set; }
    }
}
