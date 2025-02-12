using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Common;
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.Services
{
    public class InventoryService : IInventoryService
    {
        private const string InventoryFileNameFilter = "*-Inventory.txt";
        private readonly IConfigurationService _configurationService;

        public InventoryService(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public FileSystemWatcher CreateInventoryChangedWatcher()
        {
            var watcher = new FileSystemWatcher(_configurationService.EverQuestFolder);

            watcher.Filter = InventoryFileNameFilter;
            watcher.IncludeSubdirectories = false;
            watcher.NotifyFilter = NotifyFilters.LastWrite;

            return watcher;
        }

        public IReadOnlyCollection<InventoryItem> GetInventoryItems()
        {
            var inventoryFiles = Directory.GetFiles(_configurationService.EverQuestFolder, InventoryFileNameFilter);

            var results = new List<InventoryItem>();
            foreach (var inventoryFile in inventoryFiles)
            {
                var items = GetInventoryItems(inventoryFile);
                results.AddRange(items);
            }

            return results;
        }

        private string[] GetFileLines(string fileName, int maxRetries)
        {
            int tryCount = 0;
            while(tryCount < maxRetries)
            {
                try
                {
                    var lines = File.ReadAllLines(fileName);
                    return lines;
                }
                catch (Exception thrown)
                {
                    Thread.Sleep(200);
                    tryCount++;
                }
            }

            return new string[0];
        }

        private IReadOnlyCollection<InventoryItem> GetInventoryItems(string fileName)
        {
            var items = new List<InventoryItem>();

            var lines = GetFileLines(fileName, 5);

            if(lines.Length == 0)
            {
                return items;
            }

            if(lines[0] != "Location\tName\tID\tCount\tSlots")
            {
                // bad format
                return items;
            }

            // todo: implement
            var fileInfo = new FileInfo(fileName);
            var characterName = fileInfo.Name.Replace("-Inventory.txt", string.Empty, StringComparison.InvariantCultureIgnoreCase);

            foreach (var line in lines.Skip(1))
            {
                var columns = line.Split('\t');
                var itemId = int.Parse(columns[2]);
                var itemUrl = "https://wiki.project1999.com/" + System.Net.WebUtility.UrlEncode(columns[1].Replace(" ", "_"));

                if (itemId > 0)
                {
                    var item = new InventoryItem
                    {
                        CharacterName = characterName,
                        Location = columns[0],
                        ItemId = itemId,
                        ItemName = columns[1],
                        ItemUrl = itemUrl,
                        Count = int.Parse(columns[3])
                    };

                    items.Add(item);
                }
            }

            return items;
        }
    }
}
