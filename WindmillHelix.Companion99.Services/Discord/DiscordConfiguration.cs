using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Services.Discord
{
    public class DiscordConfiguration
    {
        public IReadOnlyCollection<DiscordAccount> Accounts { get; set; }

        public IReadOnlyCollection<Discord2LogConfigEntry> Discord2LogConfigEntries { get; set; }

        public IReadOnlyCollection<Log2DiscordConfigEntry> Log2DiscordConfigEntries { get; set; }
    }
}
