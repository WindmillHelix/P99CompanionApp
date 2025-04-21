using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Common;
using WindmillHelix.Companion99.Common.Threading;

namespace WindmillHelix.Companion99.Services.Discord
{
    public class DiscordConfigService : IDiscordConfigService
    {
        private readonly string _configFile;
        private readonly IEventService _eventService;

        public DiscordConfigService(IEventService eventService)
        {
            _configFile = Path.Combine(FileHelper.GetDataFolder(), "DiscordConfig.json");
            _eventService = eventService;
        }

        public DiscordConfiguration GetConfiguration()
        {
            if (!File.Exists(_configFile))
            {
                var blank = new DiscordConfiguration()
                {
                    Accounts = new List<DiscordAccount>(),
                    Log2DiscordConfigEntries = new List<Log2DiscordConfigEntry>(),
                    Discord2LogConfigEntries = new List<Discord2LogConfigEntry>()
                };

                return blank;
            }

            var json = File.ReadAllText(_configFile);
            var result = JsonConvert.DeserializeObject<DiscordConfiguration>(json);

            return result;
        }

        public SaveResult DeleteAccount(Guid accountId)
        {
            var configuration = GetConfiguration();

            if (configuration.Log2DiscordConfigEntries.Any(x => x.DiscordAccountId == accountId))
            {
                return Failure("This account is currently in use for Log2Discord");
            }

            if (configuration.Discord2LogConfigEntries.Any(x => x.DiscordAccountId == accountId))
            {
                return Failure("This account is currently in use for Discord2Log");
            }

            var temp = configuration.Accounts.ToList();
            temp.RemoveAll(x => x.DiscordAccountId == accountId);
            configuration.Accounts = temp;

            SaveConfiguration(configuration);

            // todo: switch method to async
            AsyncHelper.RunSynchronously(() => _eventService.Raise(configuration));

            return Success();
        }

        private SaveResult Failure(string failureReason)
        {
            return new SaveResult
            {
                WasSuccessful = false,
                FailureReason = failureReason
            };
        }

        private SaveResult Success()
        {
            return new SaveResult {WasSuccessful = true};
        }

        private Task<SaveResult> FailureAsync(string failureReason)
        {
            var result = Failure(failureReason);
            return Task.FromResult(result);
        }

        private Task<SaveResult> SuccessAsync()
        {
            var result = Success();
            return Task.FromResult(result);
        }

        public SaveResult SaveAccount(DiscordAccount account)
        {
            var config = GetConfiguration();

            if(string.IsNullOrWhiteSpace(account.Name))
            {
                return Failure("Name is required");
            }

            if (string.IsNullOrWhiteSpace(account.AccountToken))
            {
                return Failure("Account token is required");
            }

            account.Name = account.Name.Trim();

            var accounts = config.Accounts.ToList();
            var existing = accounts.SingleOrDefault(x => x.DiscordAccountId == account.DiscordAccountId);
            if(existing != null)
            {
                accounts.Remove(existing);
            }

            if(accounts.Any(x => x.Name.Equals(account.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return Failure("Name is already in use");
            }

            accounts.Add(account);
            config.Accounts = accounts;
            SaveConfiguration(config);

            return Success();
        }

        private void SaveConfiguration(DiscordConfiguration configuration)
        {
            var json = JsonConvert.SerializeObject(configuration);
            File.WriteAllText(_configFile, json);

            _eventService.Raise<DiscordConfiguration>(configuration);
        }

        public Task<SaveResult> SaveLog2DiscordAsync(Log2DiscordConfigEntry entry)
        {
            var config = GetConfiguration();

            if (string.IsNullOrWhiteSpace(entry.Name))
            {
                return FailureAsync("Name is required");
            }

            if (string.IsNullOrWhiteSpace(entry.Regex))
            {
                return FailureAsync("Regex is required");
            }

            if (entry.TargetChannelId <= 0)
            {
                return FailureAsync("Channel ID is required");
            }

            if (entry.DiscordAccountId == Guid.Empty)
            {
                return FailureAsync("Discord Account is required");
            }

            entry.Name = entry.Name.Trim();

            var entries = config.Log2DiscordConfigEntries.ToList();
            var existing = entries.SingleOrDefault(x => x.Log2DiscordConfigEntryId == entry.Log2DiscordConfigEntryId);
            if (existing != null)
            {
                entries.Remove(existing);
            }

            if (entries.Any(x => x.Name.Equals(entry.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return FailureAsync("Name is already in use");
            }

            entries.Add(entry);
            config.Log2DiscordConfigEntries = entries;
            SaveConfiguration(config);

            return SuccessAsync();
        }

        public Task<SaveResult> DeleteLog2DiscordAsync(Guid log2DiscordConfigEntryId)
        {
            var configuration = GetConfiguration();

            var temp = configuration.Log2DiscordConfigEntries.ToList();
            temp.RemoveAll(x => x.Log2DiscordConfigEntryId == log2DiscordConfigEntryId);
            configuration.Log2DiscordConfigEntries = temp;

            SaveConfiguration(configuration);

            AsyncHelper.RunSynchronously(() => _eventService.Raise(configuration));

            return SuccessAsync();
        }

        public Task<SaveResult> SaveDiscord2LogAsync(Discord2LogConfigEntry entry)
        {
            var config = GetConfiguration();

            if (string.IsNullOrWhiteSpace(entry.Name))
            {
                return FailureAsync("Name is required");
            }

            if (entry.DiscordAccountId == Guid.Empty)
            {
                return FailureAsync("Discord Account is required");
            }

            if (entry.SourceChannelId <= 0)
            {
                return FailureAsync("Channel ID is required");
            }

            if (string.IsNullOrWhiteSpace(entry.TargetFileName))
            {
                return FailureAsync("File name is required");
            }

            entry.Name = entry.Name.Trim();

            var entries = config.Discord2LogConfigEntries.ToList();
            var existing = entries.SingleOrDefault(x => x.Discord2LogConfigEntryId == entry.Discord2LogConfigEntryId);
            if (existing != null)
            {
                entries.Remove(existing);
            }

            if (entries.Any(x => x.Name.Equals(entry.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return FailureAsync("Name is already in use");
            }

            entries.Add(entry);
            config.Discord2LogConfigEntries = entries;
            SaveConfiguration(config);

            return SuccessAsync();
        }

        public Task<SaveResult> DeleteDiscord2LogAsync(Guid discord2LogConfigEntryId)
        {
            var configuration = GetConfiguration();

            var temp = configuration.Discord2LogConfigEntries.ToList();
            temp.RemoveAll(x => x.Discord2LogConfigEntryId == discord2LogConfigEntryId);
            configuration.Discord2LogConfigEntries = temp;

            SaveConfiguration(configuration);

            AsyncHelper.RunSynchronously(() => _eventService.Raise(configuration));

            return SuccessAsync();
        }
    }
}
