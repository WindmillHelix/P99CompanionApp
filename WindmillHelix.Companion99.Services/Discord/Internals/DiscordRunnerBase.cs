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

namespace WindmillHelix.Companion99.Services.Discord.Internals
{
    public abstract class DiscordRunnerBase :
        IEventSubscriber<DiscordConfiguration>
    {
        private readonly IEventService _eventService;
        private readonly ILogReaderService _logReaderService;
        private readonly IDiscordConfigService _discordConfigService;

        protected DiscordConfiguration DiscordConfiguration { get; private set; }

        protected ConcurrentDictionary<string, DiscordSocketClient> DiscordClients { get; private set; }

        protected Dictionary<Guid, DiscordAccount> DiscordAccounts { get; private set; }

        public DiscordRunnerBase(
            IEventService eventService,
            IDiscordConfigService discordConfigService)
        {
            DiscordClients = new ConcurrentDictionary<string, DiscordSocketClient>();
            DiscordAccounts = new Dictionary<Guid, DiscordAccount>();

            _eventService = eventService;
            _discordConfigService = discordConfigService;
        }

        public Task Handle(DiscordConfiguration value)
        {
            DiscordConfiguration = value;
            DiscordAccounts = value.Accounts.ToDictionary(x => x.DiscordAccountId, x => x);
            return Task.CompletedTask;
        }

        public virtual async Task StartAsync()
        {
            _eventService.AddSubscriber<DiscordConfiguration>(this);
            var discordConfiguration = _discordConfigService.GetConfiguration();
            await Handle(discordConfiguration);
        }
    }
}
