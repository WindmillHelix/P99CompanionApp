using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Services.Models
{
    public class WhoResult : INotifyPropertyChanged
    {
        public DateTime EventDate { get; set; }

        public string Class { get; set; }

        public int? Level { get; set; }

        public string Name { get; set; }

        public string Race { get; set; }

        public string Guild { get; set; }

        public string ZoneName { get; set; }

        public string LookingForGroup { get; set; }

        public string AwayFromKeyboard { get; set; }

        public string LinkDead { get; set; }

        public string ServerName { get; set; }

        public string OriginalLine { get; set; }

        private string _note;

        public string Note
        {
            get
            { 
                return _note;
            }

            set
            {
                _note = value;
                NotifyPropertyChanged(nameof(Note));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
