using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Services.Discord
{
    public interface IDiscordConfigService
    {
        DiscordConfiguration GetConfiguration();

        SaveResult SaveAccount(DiscordAccount account);

        SaveResult DeleteAccount(Guid accountId);

        Task<SaveResult> SaveLog2DiscordAsync(Log2DiscordConfigEntry entry);

        Task<SaveResult> DeleteLog2DiscordAsync(Guid log2DiscordConfigEntryId);

        Task<SaveResult> SaveDiscord2LogAsync(Discord2LogConfigEntry entry);

        Task<SaveResult> DeleteDiscord2LogAsync(Guid discord2LogConfigEntryId);
    }
}
