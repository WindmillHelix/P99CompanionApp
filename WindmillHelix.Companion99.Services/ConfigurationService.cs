using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Data;

namespace WindmillHelix.Companion99.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfigurationFileService _configurationFileService;
        private object _loadLock = new object();
        private bool _isLoaded = false;
        private IDictionary<string, string> _configurationCache = null;

        public ConfigurationService(IConfigurationFileService configurationFileService)
        {
            _configurationFileService = configurationFileService;
        }

        public string EverQuestFolder 
        {
            get => GetValue(nameof(EverQuestFolder));
            set => SetValue(nameof(EverQuestFolder), value);
        }

        public bool IsAncientCyclopsTimerEnabled
        {
            get => GetBoolValue(nameof(IsAncientCyclopsTimerEnabled), false);
            set => SetValue(nameof(IsAncientCyclopsTimerEnabled), value.ToString());
        }

        public bool ShouldAutoStartMiddleman
        {
            get => GetBoolValue(nameof(ShouldAutoStartMiddleman), false);
            set => SetValue(nameof(ShouldAutoStartMiddleman), value.ToString());
        }

        public string MapsFolder
        {
            get => GetValue(nameof(MapsFolder));
            set => SetValue(nameof(MapsFolder), value);
        }

        private bool GetBoolValue(string key, bool defaultValue)
        {
            var value = GetValue(key);
            if(string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            return bool.Parse(value);
        }

        public string GetValue(string key)
        {
            if(!_isLoaded)
            {
                lock(_loadLock)
                {
                    if(!_isLoaded)
                    {
                        _configurationCache = _configurationFileService.GetAllValues();
                    }
                }
            }

            var result = _configurationCache.ContainsKey(key) ? _configurationCache[key] : null;
            return result;
        }

        private void SetValue(string key, string value)
        {
            var dictionary = new Dictionary<string, string>();
            dictionary.Add(key, value);
            _configurationFileService.SetValues(dictionary);
            _configurationCache[key] = value;
        }

        public bool IsValidEverQuestFolder(string folderLocation)
        {
            if(string.IsNullOrWhiteSpace(folderLocation))
            {
                return false;
            }

            if (!Directory.Exists(folderLocation))
            {
                return false;
            }

            if (!File.Exists(Path.Combine(folderLocation, "eqgame.exe")))
            {
                return false;
            }

            return true;
        }
    }
}
