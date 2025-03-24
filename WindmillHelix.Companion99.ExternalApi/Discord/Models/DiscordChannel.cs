using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.ExternalApi.Discord.Models
{
    public class DiscordChannel
    {
        private string _id;
        private string _guildId;

        [JsonPropertyName("id")]
        public string IdString
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
                Id = ulong.Parse(value);
            }
        }

        public ulong Id
        {
            get;
            private set;
        }

        [JsonPropertyName("guild_id")]
        public string GuildIdString
        {
            get
            {
                return _guildId;
            }

            set
            {
                _guildId = value;
                GuildId = ulong.Parse(value);
            }
        }

        public ulong GuildId
        {
            get;
            private set;
        }
    }
}
