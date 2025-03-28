using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Common;

namespace WindmillHelix.Companion99.Data.Migrators
{
    public class ConfigJsonMigrator : IMigrator
    {
        private readonly IKeyValueRepository _keyValueRepository;

        public ConfigJsonMigrator(IKeyValueRepository keyValueRepository)
        {
            _keyValueRepository = keyValueRepository;
        }

        public string Key => "ConfigJson";

        public void Execute()
        {
            var configFile = Path.Combine(FileHelper.GetDataFolder(), "config.json");
            if(!File.Exists(configFile))
            {
                return;
            }

            var json = File.ReadAllText(configFile);
            var values = JsonConvert.DeserializeObject<IDictionary<string, string>>(json);
            foreach(var item in values)
            {
                _keyValueRepository.SetValue(item.Key, item.Value);
            }
        }
    }
}
