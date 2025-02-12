using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using WindmillHelix.Companion99.App.Models;
using WindmillHelix.Companion99.Services;
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.App
{
    /// <summary>
    /// Interaction logic for InventoryControl.xaml
    /// </summary>
    public partial class InventoryControl : UserControl
    {
        private IReadOnlyCollection<InventoryItem> _items;
        private readonly IInventoryService _inventoryService;
        private readonly FileSystemWatcher _watcher;

        private readonly Regex _multiIdsRegex = new Regex(@"^(\d+,)+\d+$");

        private IReadOnlyCollection<InventoryItem> _filteredItems;

        public InventoryControl()
        {
            InitializeComponent();

            _inventoryService = DependencyInjector.Resolve<IInventoryService>();
            _watcher = _inventoryService.CreateInventoryChangedWatcher();

            var items = _inventoryService.GetInventoryItems();

            _items = items;

            var characterNames = _items.Select(x => x.CharacterName).Distinct().OrderBy(x => x).Select(x => new ComboItem<string, string> { Value = x, Display = x });

            var characterItems = new List<ComboItem<string, string>>();
            characterItems.Add(new ComboItem<string, string> { Value = string.Empty, Display = "-- All --" });
            characterItems.AddRange(characterNames);

            CharacterComboBox.ItemsSource = characterItems;
            CharacterComboBox.SelectedIndex = 0;
            KeysButton.IsEnabled = false;

            SearchTextBox.TextChanged += SearchTextBox_TextChanged;
            CharacterComboBox.SelectionChanged += CharacterComboBox_SelectionChanged;

            _watcher = _inventoryService.CreateInventoryChangedWatcher();
            _watcher.Changed += HandleInventoryFilesChanged;
            _watcher.Created += HandleInventoryFilesChanged;
            _watcher.EnableRaisingEvents = true;

            SetItemSource();
        }

        private void HandleInventoryFilesChanged(object sender, FileSystemEventArgs e)
        {
            var items = _inventoryService.GetInventoryItems();

            _items = items;

            var characterNames = _items.Select(x => x.CharacterName).Distinct().OrderBy(x => x).Select(x => new ComboItem<string, string> { Value = x, Display = x });

            var characterItems = new List<ComboItem<string, string>>();
            characterItems.Add(new ComboItem<string, string> { Value = string.Empty, Display = "-- All --" });
            characterItems.AddRange(characterNames);

            Dispatcher.Invoke(() =>
            {
                var currentCharacterValue = (string)CharacterComboBox.SelectedValue;

                CharacterComboBox.ItemsSource = characterItems;
                if (currentCharacterValue != null)
                {
                    CharacterComboBox.SelectedItem = characterItems.Single(x => x.Value == currentCharacterValue);
                }

                SetItemSource();
            });
        }

        private void CharacterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string characterName = (string)CharacterComboBox.SelectedValue;
            KeysButton.IsEnabled = !string.IsNullOrWhiteSpace(characterName) && !characterName.StartsWith("--");
            SetItemSource();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetItemSource();
        }

        private void SetItemSource()
        {
            var searchText = SearchTextBox.Text;

            
            var filtered = _items;

            if(!string.IsNullOrWhiteSpace((string)CharacterComboBox.SelectedValue))
            {
                filtered = filtered.Where(x => x.CharacterName == (string)CharacterComboBox.SelectedValue).ToList();
            }

            if(string.IsNullOrWhiteSpace(searchText))
            {
                _filteredItems = filtered;
                ItemsListView.ItemsSource = filtered;
                BuildResultSummary(filtered);
                return;
            }

            int itemId = -1;
            if (int.TryParse(searchText, out itemId))
            {
                filtered = filtered.Where(x => x.ItemId == itemId).ToList();
            }
            else if(_multiIdsRegex.IsMatch(searchText))
            {
                var ids = searchText.Split(',').Select(x => int.Parse(x)).ToList();
                filtered = filtered.Where(x => ids.Contains(x.ItemId)).ToList();
            }
            else
            {
                filtered = filtered.Where(x => x.ItemName.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }

            _filteredItems = filtered;
            BuildResultSummary(filtered);
            ItemsListView.ItemsSource = filtered;
        }

        private void BuildResultSummary(IReadOnlyCollection<InventoryItem> filtered)
        {
            var totalCount = filtered.Sum(x => x.Count);
            var distinctCount = filtered.Select(x => x.ItemId).Distinct().Count();

            var resultSummary = $"{totalCount} item";
            if (totalCount != 1)
            {
                resultSummary += "s";
            }

            resultSummary += $" in {filtered.Count} slot";
            if (filtered.Count != 1)
            {
                resultSummary += "s";
            }

            resultSummary += $", {distinctCount} distinct item";
            if (distinctCount != 1)
            {
                resultSummary += "s";
            }

            ResultsSummaryLabel.Content = resultSummary;
        }

        private void KeysButton_Click(object sender, RoutedEventArgs e)
        {
            string characterName = (string)CharacterComboBox.SelectedValue;
            if (string.IsNullOrWhiteSpace(characterName))
            {
                throw new Exception("Character is required");
            }

            var filtered = _items.Where(x => x.CharacterName == characterName).ToList();

            var window = new KeysWindow();
            window.Items = filtered;
            window.CharacterName = characterName;
            window.Show();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
