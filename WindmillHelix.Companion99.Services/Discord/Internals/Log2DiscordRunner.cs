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
    public class Log2DiscordRunner : DiscordRunnerBase, ILogListener
    {
        private readonly ILogReaderService _logReaderService;

        private ConcurrentDictionary<string, DiscordSocketClient> _discordClients = new ConcurrentDictionary<string, DiscordSocketClient>();

        public Log2DiscordRunner(
            IEventService eventService,
            ILogReaderService logReaderService,
            IDiscordConfigService discordConfigService)
            : base(eventService, discordConfigService)
        {
            _logReaderService = logReaderService;
        }

        public async override Task StartAsync()
        {
            await base.StartAsync();

            _logReaderService.AddListener(this);
        }

        public void HandleLogLine(string serverName, string characterName, DateTime eventDate, string line)
        {
            AsyncHelper.RunSynchronously(() => HandleLogLineAsync(serverName, characterName, eventDate, line));
        }

        private async Task HandleLogLineAsync(string serverName, string characterName, DateTime eventDate, string line)
        {
            // get a reference to current config because it can change
            var config = base.DiscordConfiguration;
            if (config == null)
            {
                return;
            }

            foreach (var entry in config.Log2DiscordConfigEntries)
            {
                if (!entry.IsEnabled)
                {
                    continue;
                }

                if (Regex.IsMatch(line, entry.Regex))
                {
                    var account = base.DiscordAccounts[entry.DiscordAccountId];
                    var client = await GetDiscordClientAsync(account);
                    if (client.LoginState == LoginState.LoggedIn)
                    {
                        var channel = client.GetChannel(entry.TargetChannelId) as IMessageChannel;
                        if (channel != null)
                        {
                            await channel.SendMessageAsync(line);
                        }
                    }
                    else
                    {
                        // todo: log
                    }
                }
            }
        }

        private async Task<DiscordSocketClient> GetDiscordClientAsync(DiscordAccount account)
        {
            if (_discordClients.ContainsKey(account.AccountToken))
            {
                return _discordClients[account.AccountToken];
            }

            var client = new DiscordSocketClient();
            await client.LoginAsync(TokenType.Bot, account.AccountToken, true);
            await client.StartAsync();

            _discordClients.AddOrUpdate(account.AccountToken, client, (a, b) => client);
            return client;
        }
    }
}