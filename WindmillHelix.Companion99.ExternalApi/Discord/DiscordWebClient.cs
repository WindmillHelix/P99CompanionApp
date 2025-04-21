using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WindmillHelix.Companion99.ExternalApi.Discord.Models;

namespace WindmillHelix.Companion99.ExternalApi.Discord
{
    public  class DiscordWebClient
    {
        private readonly string _token;

        public DiscordWebClient(string token)
        {
            _token = token;
        }

        public async Task<IReadOnlyCollection<DiscordMessage>> GetMessages(ulong channelId)
        {
            var url = $"https://discord.com/api/v9/channels/{channelId}/messages?limit=50";
            var messages = await PerformGet<DiscordMessage[]>(url);

            return messages;
        }

        public async Task<DiscordChannel> GetChannel(ulong channelId)
        {
            var url = $"https://discord.com/api/v9/channels/{channelId}";
            var channel = await PerformGet<DiscordChannel>(url);

            return channel;
        }

        private async Task<T> PerformGet<T>(string url)
        {
            var userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/105.0.0.0 Safari/537.36";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("user-agent", userAgent);
            client.DefaultRequestHeaders.Add("authorization", _token);

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<T>(body);
            return result;
        }
    }
}
