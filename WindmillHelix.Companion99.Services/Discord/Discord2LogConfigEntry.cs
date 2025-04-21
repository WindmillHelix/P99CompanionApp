using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Services.Discord
{
    public class Discord2LogConfigEntry
    {
        public Guid Discord2LogConfigEntryId { get; set; }

        public Guid DiscordAccountId { get; set; }

        public string Name { get; set; }

        public ulong SourceChannelId { get; set; }

        public string TargetFileName { get; set; }

        public string Prefix { get; set; }

        public string FilterRegex { get; set; }

        public string RemoveRegex { get; set; }

        public bool IsEnabled { get; set; }
    }
}
