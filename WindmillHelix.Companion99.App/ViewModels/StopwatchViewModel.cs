using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WindmillHelix.Companion99.App.ViewModels
{
    public class StopwatchViewModel : INotifyPropertyChanged
    {
        public Stopwatch Watch { get; set; }

        public string Elapsed
        {
            get
            {
                var elapsed = Watch.Elapsed;
                var text = string.Format(
                    "{0}:{1}:{2}.{3}",
                    Math.Floor(elapsed.TotalHours),
                    elapsed.Minutes.ToString("D2"),
                    elapsed.Seconds.ToString("D2"),
                    Math.Floor(elapsed.Milliseconds / 100d));

                return text;
            }
        }

        public Visibility StartButtonVisibility
        {
            get
            {
                return Watch.IsRunning ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public Visibility StopButtonVisibility
        {
            get
            {
                return Watch.IsRunning ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void RaiseTimerChange()
        {
            RaisePropertyChanged(nameof(Elapsed));
        }

        public void RaiseStatusChange()
        {
            RaisePropertyChanged(nameof(StartButtonVisibility));
            RaisePropertyChanged(nameof(StopButtonVisibility));
        }

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
