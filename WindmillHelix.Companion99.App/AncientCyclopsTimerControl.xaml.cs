using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WindmillHelix.Companion99.App
{
    /// <summary>
    /// Interaction logic for AncientCyclopsTimerControl.xaml
    /// </summary>
    public partial class AncientCyclopsTimerControl : UserControl
    {
        private string _lastValue = string.Empty;

        public AncientCyclopsTimerControl()
        {
            InitializeComponent();
            var start = new ThreadStart(RunThread);
            var thread = new Thread(start);
            thread.Start();
        }

        private void RunThread()
        {
            Thread.CurrentThread.IsBackground = true;

            var startTime = new DateTime(2022, 11, 6, 10, 26, 0, DateTimeKind.Utc);

            while(true)
            {
                var now = DateTime.UtcNow;
                var difference = now - startTime;
                var sinceLast = Math.Floor(difference.TotalSeconds) % (72*60);

                var until = TimeSpan.FromSeconds(72 * 60 - sinceLast);

                var value = until.ToString();
                var colors = GetColors(until);

                if (value != _lastValue)
                {
                    try
                    {
                        Dispatcher.Invoke(() =>
                        {
                            TimerLabel.Content = value;
                            TimerLabel.Background = colors.Item1;
                            TimerLabel.Foreground = colors.Item2;
                        });

                        _lastValue = value;
                    }
                    catch(TaskCanceledException)
                    {
                    }
                }

                Thread.Sleep(100);
            }
        }

        private Tuple<Brush, Brush> GetColors(TimeSpan timeUntilSpawn)
        {
            var totalSeconds = timeUntilSpawn.TotalSeconds;
            if(totalSeconds < 2 * 60 || totalSeconds > 60*71)
            {
                return new Tuple<Brush, Brush>(Brushes.Red, Brushes.White);
            }

            if(totalSeconds < 60 * 10)
            {
                return new Tuple<Brush, Brush>(Brushes.Yellow, Brushes.Black);
            }

            return new Tuple<Brush, Brush>(Brushes.Green, Brushes.White);
        }
    }
}
