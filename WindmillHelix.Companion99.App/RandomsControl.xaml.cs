using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
using System.Windows.Threading;
using WindmillHelix.Companion99.App.Models;
using WindmillHelix.Companion99.App.ViewModels;
using WindmillHelix.Companion99.Services;

namespace WindmillHelix.Companion99.App
{
    /// <summary>
    /// Interaction logic for RandomsControl.xaml
    /// </summary>
    public partial class RandomsControl : UserControl, ILogListener
    {
        private bool _isNextLineRollValue = false;
        private string _roller = null;

        private bool _shouldAutoExpand = false;

        private Dictionary<string, RollSetViewModel> _rollLookup = new Dictionary<string, RollSetViewModel>();

        private DispatcherTimer _updateTimer = new DispatcherTimer();

        public RandomsControl()
        {
            InitializeComponent();

            this.DataContext = this;

            RollSets = new ObservableCollection<RollSetViewModel>();
            var logReaderService = DependencyInjector.Resolve<ILogReaderService>();
            logReaderService.AddListener(this);
            _updateTimer.Tick += new EventHandler(AutoRemove);
            _updateTimer.Interval = TimeSpan.FromSeconds(1);
            _updateTimer.Start();
        }

        private void AutoRemove(object sender, EventArgs e)
        {
            var item = AutoRemoveComboBox.SelectedItem as ComboBoxItem;

            var value = int.Parse(item.Tag.ToString());
            if(value < 0)
            {
                return;
            }

            var cutoff = DateTime.Now.AddSeconds(-1 * value);

            var toRemove = RollSets.Where(x => x.LastRoll < cutoff).ToList();

            foreach(var rollSet in toRemove)
            {
                RemoveRollSet(rollSet);
            }
        }

        public ObservableCollection<RollSetViewModel> RollSets { get; private set; }

        public void HandleLogLine(string serverName, string characterName, DateTime eventDate, string line)
        {
            if (_isNextLineRollValue)
            {
                var parts = line.Split(' ');
                var min = parts[7];
                var max = parts[9].Replace(",", string.Empty);
                var rollValueString = parts.Last().Replace(".", string.Empty);

                var key = $"{min}_{max}";

                if(!_rollLookup.ContainsKey(key))
                {
                    var newRollSet = new RollSetViewModel()
                    {
                        FirstRoll = eventDate,
                        Min = int.Parse(min),
                        Max = int.Parse(max),
                        IsExpanded = _shouldAutoExpand
                    };

                    _rollLookup.Add(key, newRollSet);

                    this.Dispatcher.Invoke(() =>
                    {
                        RollSets.Insert(0, newRollSet);
                    });
                }

                var rollSet = _rollLookup[key];

                var roll = new RollViewModel
                {
                    RollSet = rollSet,
                    CharacterName = _roller,
                    RollTime = eventDate,
                    Value = int.Parse(rollValueString)
                };

                rollSet.Rolls.Add(roll);

                _isNextLineRollValue = false;
                _roller = null;

                return;
            }

            const string rollPrefix = "**A Magic Die is rolled by ";
            if (line.StartsWith(rollPrefix))
            {
                var roller = line.Substring(rollPrefix.Length).TrimEnd('.').Trim();
                _roller = roller;
                _isNextLineRollValue = true;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            var rollSet = (RollSetViewModel)button.Tag;
            RemoveRollSet(rollSet);
        }

        private void RemoveRollSet(RollSetViewModel rollSet)
        {
            var key = $"{rollSet.Min}_{rollSet.Max}";

            this.Dispatcher.Invoke(() =>
            {
                RollSets.Remove(rollSet);
            });

            _rollLookup.Remove(key);
        }

        private void ExpandRollsButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button.Tag as RollSetViewModel;
            item.IsExpanded = !item.IsExpanded;
        }

        private void AutoExpandCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            _shouldAutoExpand = AutoExpandCheckbox.IsChecked ?? false;
        }
    }
}
