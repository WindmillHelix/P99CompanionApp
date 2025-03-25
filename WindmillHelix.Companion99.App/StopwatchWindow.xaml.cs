using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using WindmillHelix.Companion99.App.Models;
using WindmillHelix.Companion99.App.ViewModels;

namespace WindmillHelix.Companion99.App
{
    /// <summary>
    /// Interaction logic for StopwatchWindow.xaml
    /// </summary>
    public partial class StopwatchWindow : Window
    {
        private static ObservableCollection<StopwatchViewModel> _watches = new ObservableCollection<StopwatchViewModel>();

        private DispatcherTimer _updateTimer = new DispatcherTimer();

        public StopwatchWindow()
        {
            InitializeComponent();
            WatchesListView.DataContext = _watches;

            _updateTimer.Tick += new EventHandler(DoUpdates);
            _updateTimer.Interval = TimeSpan.FromMilliseconds(20);
            _updateTimer.Start();
        }

        private void DoUpdates(object sender, EventArgs e)
        {
            foreach (var watch in _watches)
            {
                watch.RaiseTimerChange();
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            AddStopwatch();
        }

        private void ClearAllButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(var watch in _watches)
            {
                watch.Watch.Stop();
            }

            _watches.Clear();
            AddStopwatch();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if(_watches.Count == 0)
            {
                AddStopwatch();
            }
        }

        private void AddStopwatch()
        {
            var model = new StopwatchViewModel
            {
                Watch = new Stopwatch()
            };

            _watches.Add(model);
            AutoSetHeight();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            var model = (sender as Button)?.Tag as StopwatchViewModel;
            model.Watch.Stop();
            model.RaiseStatusChange();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var model = (sender as Button)?.Tag as StopwatchViewModel;
            model.Watch.Stop();
            _watches.Remove(model);
            AutoSetHeight();
        }

        private void AutoSetHeight()
        {
            var count = _watches.Count;
            var height = 200;
            if (count > 5)
            {
                height += 28 * (count - 5);
            }

            this.Height = height;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            var model = (sender as Button)?.Tag as StopwatchViewModel;
            model.Watch.Start();
            model.RaiseStatusChange();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            var model = (sender as Button)?.Tag as StopwatchViewModel;
            model.Watch?.Reset();
            model.RaiseTimerChange();
        }
    }
}
