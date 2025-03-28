using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Data;

namespace WindmillHelix.Companion99.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IKeyValueRepository _keyValueRepository;
        private object _loadLock = new object();
        private bool _isLoaded = false;
        private IDictionary<string, string> _configurationCache = null;

        public ConfigurationService(IKeyValueRepository keyValueRepository)
        {
            _keyValueRepository = keyValueRepository;
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

        public bool IsDiscordOverlayEnabled
        {
            get => GetBoolValue(nameof(IsDiscordOverlayEnabled), false);
            set => SetValue(nameof(IsDiscordOverlayEnabled), value.ToString());
        }

        public string MapsFolder
        {
            get => GetValue(nameof(MapsFolder));
            set => SetValue(nameof(MapsFolder), value);
        }

        public Point DiscordOverlayLocation
        {
            get => GetPointValue(nameof(DiscordOverlayLocation), new Point(50, 50));
            set => SetPointValue(nameof(DiscordOverlayLocation), value);
        }

        public Point DiscordOverlaySize
        {
            get => GetPointValue(nameof(DiscordOverlaySize), new Point(150, 250));
            set => SetPointValue(nameof(DiscordOverlaySize), value);
        }

        private Point GetPointValue(string key, Point defaultValue)
        {
            var value = GetValue(key);
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            var items = value.Split(',');
            var result = new Point(int.Parse(items[0]), int.Parse(items[1]));
            return result;
        }

        private void SetPointValue(string key, Point pointValue)
        {
            var value = $"{pointValue.X},{pointValue.Y}";
            SetValue(key, value);
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
                        _configurationCache = _keyValueRepository.GetAllValues().ToDictionary(x => x.Key, x => x.Value);
                    }
                }
            }

            var result = _configurationCache.ContainsKey(key) ? _configurationCache[key] : null;
            return result;
        }

        private void SetValue(string key, string value)
        {
            _keyValueRepository.SetValue(key, value);
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
