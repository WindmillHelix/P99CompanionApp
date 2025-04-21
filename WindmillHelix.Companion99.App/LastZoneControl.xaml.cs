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
using WindmillHelix.Companion99.App.Commands;
using WindmillHelix.Companion99.App.ViewModels;
using WindmillHelix.Companion99.Common;
using WindmillHelix.Companion99.Services;
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.App
{
    /// <summary>
    /// Interaction logic for LastZoneControl.xaml
    /// </summary>
    public partial class LastZoneControl : UserControl, ILogListener
    {
        private readonly ILastZoneService _lastZoneService;
        private readonly ILastLoginService _lastLoginService;

        private bool _isResetting = false;

        private IReadOnlyCollection<CharacterZoneViewModel> _items;

        private const string ZonedPrefix = "You have entered ";
        private const string CurrentBindPrefix = "You are currently bound in: ";
        private const string NewBindLine = "You feel yourself bind to the area.";

        public LastZoneControl()
        {
            InitializeComponent();

            DataContext = this;

            var logReaderService = DependencyInjector.Resolve<ILogReaderService>();
            _lastZoneService = DependencyInjector.Resolve<ILastZoneService>();
            _lastLoginService = DependencyInjector.Resolve<ILastLoginService>();
            logReaderService.AddListener(this);

            LoadZones();
            SearchTextBox.TextChanged += SearchTextBox_TextChanged;

            var start = new ThreadStart(RunTimerThread);
            var thread = new Thread(start);
            thread.Start();

            DeleteCommand = new GenericCommand(DeleteSelectedItem);
            DeleteCommand = new GenericCommand(HandleContextMenuCommand);
        }

        public ICommand DeleteCommand { get; private set; }

        public ICommand ContextMenuCommand { get; private set; }

        private void HandleContextMenuCommand()
        {

        }

        private void DeleteSelectedItem()
        {
            var selected = ResultsListView.SelectedItem as CharacterZoneViewModel;
            if (selected == null)
            {
                return;
            }

            _lastZoneService.RemoveEntry(selected.ServerName, selected.CharacterName);
            LoadZones();
        }

        private void RunTimerThread()
        {
            Thread.CurrentThread.IsBackground = true;

            while (true)
            {
                if (_items != null)
                {
                    foreach (var item in _items)
                    {
                        item.RaiseSkyCorpseTimerChange();
                    }
                }

                Thread.Sleep(TimeSpan.FromMinutes(1));
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        public void HandleLogLine(string serverName, string characterName, DateTime eventDate, string line)
        {
            if (line.StartsWith(ZonedPrefix))
            {
                HandleZoneLine(serverName, characterName, eventDate, line);
            }
            else if (line.StartsWith(CurrentBindPrefix))
            {
                HandleCurrentBindLine(serverName, characterName, eventDate, line);
            }
            else if (line == NewBindLine)
            {
                HandleNewBindLine(serverName, characterName, eventDate, line);
            }
        }

        private void HandleZoneLine(string serverName, string characterName, DateTime eventDate, string line)
        {
            var zone = line.Substring(ZonedPrefix.Length).TrimEnd('.');
            var account = _lastLoginService.GetLastLoginName();
            _lastZoneService.SetLastZone(serverName, characterName, zone, account);
            LoadZones();
        }

        private void HandleCurrentBindLine(string serverName, string characterName, DateTime eventDate, string line)
        {
            var zone = line.Substring(CurrentBindPrefix.Length).TrimEnd('.');
            _lastZoneService.SetBindPoint(serverName, characterName, zone);
            LoadZones();
        }

        private void HandleNewBindLine(string serverName, string characterName, DateTime eventDate, string line)
        {
            var current = _items.SingleOrDefault(x => x.ServerName.EqualsIngoreCase(serverName)
                && x.CharacterName.Equals(characterName));

            if (!string.IsNullOrWhiteSpace(current?.ZoneName))
            {
                _lastZoneService.SetBindPoint(serverName, characterName, current.ZoneName);
                LoadZones();
            }
        }

        private void LoadZones()
        {
            var items = _lastZoneService.GetLastZones()
                .OrderBy(x => x.ServerName).ThenBy(x => x.CharacterName).ToList();

            Dispatcher.Invoke(() =>
            {
                _items = items.Select(x => new CharacterZoneViewModel(x)).ToList();
            });

            ApplyFilters();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            _isResetting = true;
            try
            {
                IncludeIgnoredCheckBox.IsChecked = false;
                SearchTextBox.Clear();
                _isResetting = false;
                ApplyFilters();

            }
            finally
            {
                _isResetting = false;
            }
        }

        private void ApplyFilters()
        {
            Dispatcher.Invoke(() =>
            {
                var items = _items;
                var filter = SearchTextBox.Text?.Trim();
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    items = items.Where(
                        x => x.Account.ContainsIngoreCase(filter)
                        || x.CharacterName.ContainsIngoreCase(filter)
                        || x.ZoneName.ContainsIngoreCase(filter)
                        || x.BindZone.ContainsIngoreCase(filter)
                        || (filter.EqualsIngoreCase("sky") && x.SkyCorpseTimer.HasValue)).ToList();
                }

                var shouldIncludeIngored = IncludeIgnoredCheckBox.IsChecked ?? false;
                items = items.Where(x => !x.IsIgnored || shouldIncludeIngored).ToList();

                ResultsListView.ItemsSource = items;
            });
        }

        private void Row_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContextMenu menu = new ContextMenu();

            MenuItem deleteMenuItem = new MenuItem();
            deleteMenuItem.Tag = sender;
            deleteMenuItem.Header = "Delete";
            menu.Items.Add(deleteMenuItem);

            MenuItem ignoreMenuIte = new MenuItem();
            deleteMenuItem.Tag = sender;
            deleteMenuItem.Header = "Ignore";
            menu.Items.Add(deleteMenuItem);

            menu.IsOpen = true;
        }

        private void GridView_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        protected void HandleRightClick(object sender, MouseButtonEventArgs e)
        {
            var selected = ResultsListView.SelectedItem as CharacterZoneViewModel;
            if (selected == null)
            {
                return;
            }

            ContextMenu menu = new ContextMenu();

            MenuItem deleteMenuItem = new MenuItem();
            deleteMenuItem.Tag = selected;
            deleteMenuItem.Header = "Delete";
            deleteMenuItem.Click += DeleteMenuItem_Click;
            menu.Items.Add(deleteMenuItem);

            if (selected.IsIgnored)
            {
                MenuItem ignoreMenuItem = new MenuItem();
                ignoreMenuItem.Tag = selected;
                ignoreMenuItem.Header = "Unignore";
                ignoreMenuItem.Click += UnignoreMenuItem_Click;
                menu.Items.Add(ignoreMenuItem);
            }
            else
            {
                MenuItem ignoreMenuItem = new MenuItem();
                ignoreMenuItem.Tag = selected;
                ignoreMenuItem.Header = "Ignore";
                ignoreMenuItem.Click += IgnoreMenuItem_Click;
                menu.Items.Add(ignoreMenuItem);
            }

            menu.IsOpen = true;
        }

        private void IgnoreMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            var item = menuItem?.Tag as CharacterZoneViewModel;
            if(item == null)
            {
                return;
            }

            _lastZoneService.SetIgnored(item.ServerName, item.CharacterName, true);
            LoadZones();
        }

        private void UnignoreMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            var item = menuItem?.Tag as CharacterZoneViewModel;
            if (item == null)
            {
                return;
            }

            _lastZoneService.SetIgnored(item.ServerName, item.CharacterName, false);
            LoadZones();
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            var item = menuItem?.Tag as CharacterZoneViewModel;
            if (item == null)
            {
                return;
            }

            _lastZoneService.RemoveEntry(item.ServerName, item.CharacterName);
            LoadZones();
        }

        private void IncludeIgnoredCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if(_isResetting)
            {
                return;
            }

            LoadZones();
        }
    }
}
