using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Services.Discord
{
    public class Log2DiscordConfigEntry
    {
        public Guid Log2DiscordConfigEntryId { get; set; }

        public Guid DiscordAccountId { get; set; }

        public string Name { get; set; }

        public string Regex { get; set; }

        public ulong TargetChannelId { get; set; } 

        public bool IsEnabled { get; set; }
    }
}
