using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Services.Discord
{
    public class DiscordAccount
    {
        public Guid DiscordAccountId { get; set; }

        public string AccountToken { get; set; }

        public string Name { get; set; }
    }
}
