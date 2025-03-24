using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Common.Threading;
using WindmillHelix.Companion99.Services.Discord.Internals;

namespace WindmillHelix.Companion99.Services.Discord
{
    public class DiscordWorkerService : IDiscordWorkerService
    {
        private readonly Discord2LogRunner _discord2LogRunner;
        private readonly Log2DiscordRunner _log2DiscordRunner;

        public DiscordWorkerService(
           Discord2LogRunner discord2LogRunner,
           Log2DiscordRunner log2DiscordRunner)
        {
            _discord2LogRunner = discord2LogRunner;
            _log2DiscordRunner = log2DiscordRunner;
        }

        public async Task StartAsync()
        {
            await _log2DiscordRunner.StartAsync();
            await _discord2LogRunner.StartAsync();
        }
    }
}
