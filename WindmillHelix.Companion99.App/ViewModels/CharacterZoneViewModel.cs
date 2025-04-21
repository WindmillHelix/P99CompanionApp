using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.App.ViewModels
{
    public class CharacterZoneViewModel : INotifyPropertyChanged
    {
        private readonly CharacterZone _model;

        public CharacterZoneViewModel(CharacterZone model)
        {
            _model = model;
        }

        public string ServerName => _model.ServerName;

        public string CharacterName => _model.CharacterName;

        public string ZoneName => _model.ZoneName;

        public string Account => _model.Account;

        public DateTime? SkyCorpseDate => _model.SkyCorpseDate;

        public string BindZone => _model.BindZone;

        public bool IsIgnored => _model.IsIgnored;

        public TimeSpan? SkyCorpseTimer
        {
            get
            {
                if (!SkyCorpseDate.HasValue)
                {
                    return null;
                }

                var value = SkyCorpseDate.Value;
                var now = DateTime.UtcNow;
                var expiryDate = now.Subtract(TimeSpan.FromDays(7));
                if(value < expiryDate)
                {
                    return null;
                }

                var timer = TimeSpan.FromDays(7) + (value - now);

                return timer;
            }
        }

        public string SkyCorpseTimerString
        {
            get
            {
                var timer = SkyCorpseTimer;
                if(timer == null)
                {
                    return null;
                }

                var value = timer.Value;
                var result = $"{value.Days}d {value.Hours}h {value.Minutes}m";
                return result;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void RaiseSkyCorpseTimerChange()
        {
            NotifyPropertyChanged(nameof(SkyCorpseTimer));
            NotifyPropertyChanged(nameof(SkyCorpseTimerString));
        }
    }
}
