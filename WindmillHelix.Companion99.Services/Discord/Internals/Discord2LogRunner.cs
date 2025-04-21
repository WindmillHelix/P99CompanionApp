using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Common;
using WindmillHelix.Companion99.ExternalApi.Discord;

namespace WindmillHelix.Companion99.Services.Discord.Internals
{
    public class Discord2LogRunner : DiscordRunnerBase
    {
        private Dictionary<string, DiscordWebClient> _discordClients = new Dictionary<string,DiscordWebClient>();
        private readonly Dictionary<string, ulong> _lastProcessedMessages = new Dictionary<string, ulong>();

        public Discord2LogRunner(
            IEventService eventService,
            ILogReaderService logReaderService,
            IDiscordConfigService discordConfigService)
            : base(eventService, discordConfigService)
        {
        }


        public async override Task StartAsync()
        {
            await base.StartAsync();

            // start poller, purposely not awaited
            var notAwaited = RunDiscordPoller();
        }

        private async Task RunDiscordPoller()
        {
            while (true)
            {
                try
                {
                    var config = base.DiscordConfiguration;
                    var accountMap = base.DiscordAccounts;
                    await PerformPollerRunAsync(config, accountMap);
                }
                catch (Exception thrown)
                {
                    Console.WriteLine(thrown.Message);
                    Console.WriteLine(thrown.StackTrace);
                }

                await Task.Delay(3000);
            }
        }


        private async Task PerformPollerRunAsync(DiscordConfiguration configuration, IDictionary<Guid, DiscordAccount> accountMap)
        {
            var entries = configuration.Discord2LogConfigEntries.Where(x => x.IsEnabled);
            var accountIds = entries.Select(x => x.DiscordAccountId).Distinct().ToList();

            foreach(var accountId in accountIds)
            {
                var account = accountMap[accountId];
                var channelIds = entries.Where(x => x.DiscordAccountId == accountId).Select(x => x.SourceChannelId).Distinct().ToList();
                foreach(var channelId in channelIds)
                {
                    try
                    {
                        var markerKey = $"{accountId}_{channelId}";
                        if (!_lastProcessedMessages.ContainsKey(markerKey))
                        {
                            _lastProcessedMessages.Add(markerKey, 0);
                        }

                        var client = await GetDiscordClientAsync(account);

                        var filters = entries.Where(x => x.DiscordAccountId == accountId && x.SourceChannelId == channelId).ToList();
                        var lastProcessedMessageId = _lastProcessedMessages[markerKey];
                        var messages = (await client.GetMessages(channelId)).OrderBy(x => x.Id).ToList();

                        if (lastProcessedMessageId != 0)
                        {
                            var toDisplay = messages.Where(x => x.Id > lastProcessedMessageId).ToList();

                            foreach (var message in toDisplay)
                            {
                                foreach (var filter in filters)
                                {
                                    var messageContent = message.Content.Replace("\r\n", "\n").Replace("\n", " ").Trim();
                                    if (string.IsNullOrWhiteSpace(filter.FilterRegex) || Regex.IsMatch(messageContent, filter.FilterRegex))
                                    {
                                        DispatchMessage(filter, messageContent);
                                    }
                                }
                            }
                        }

                        var lastMessageId = messages.Last().Id;

                        _lastProcessedMessages[markerKey] = lastMessageId;
                    }
                    catch (Exception ex)
                    {
                        // todo: log
                    }
                }
            }
        }

        private void DispatchMessage(Discord2LogConfigEntry filter, string content)
        {
            if (!string.IsNullOrEmpty(filter.RemoveRegex))
            {
                content = Regex.Replace(content, filter.RemoveRegex, string.Empty);
            }

            if(string.IsNullOrWhiteSpace(content))
            {
                return;
            }

            var logLine = string.Format(
                "[{0}] {2}{1}\r\n",
                DateTime.Now.FormatForEverquestLog(),
                content.Trim(),
                filter.Prefix);

            File.AppendAllLines(filter.TargetFileName, new[] { logLine.Trim() });
        }

        private Task<DiscordWebClient> GetDiscordClientAsync(DiscordAccount account)
        {
            if (_discordClients.ContainsKey(account.AccountToken))
            {
                return Task.FromResult(_discordClients[account.AccountToken]);
            }

            var client = new DiscordWebClient(account.AccountToken);

            _discordClients.Add(account.AccountToken, client);
            return Task.FromResult(client);
        }
    }
}
