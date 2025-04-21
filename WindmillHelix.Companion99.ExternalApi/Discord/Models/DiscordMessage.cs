using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.ExternalApi.Discord.Models
{
    public class DiscordMessage
    {
        private string _id;
        private string _channelId;

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

        [JsonPropertyName("channel_id")]
        public string ChannelIdString
        {
            get
            {
                return _channelId;
            }

            set
            {
                _channelId = value;
                ChannelId = ulong.Parse(value);
            }
        }

        public ulong ChannelId
        {
            get;
            private set;
        }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("author")]
        public DiscordMessageAuthor Author { get; set; }
    }
}
