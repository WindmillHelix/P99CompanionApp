using System;
using System.Collections.Generic;
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
using WindmillHelix.Companion99.Common;
using WindmillHelix.Companion99.Services;
using WindmillHelix.Companion99.Services.Events;
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.App
{
    /// <summary>
    /// Interaction logic for WhoResultsControl.xaml
    /// </summary>
    public partial class WhoResultsControl : UserControl, ILogListener, IEventSubscriber<NoteUpdateEvent>
    {
        private bool _isInWhoResults = false;
        private bool _isResetting = false;

        private ILineParserService _lineParserService;

        private List<Tuple<string, DateTime>> _resultsLines = new List<Tuple<string, DateTime>>();
        private List<WhoResult> _results = new List<WhoResult>();
        private List<WhoResult> _filtered = new List<WhoResult>();

        public WhoResultsControl()
        {
            InitializeComponent();

            _lineParserService = DependencyInjector.Resolve<ILineParserService>();
            var logReaderService = DependencyInjector.Resolve<ILogReaderService>();
            var eventService = DependencyInjector.Resolve<IEventService>();

            
            LfgComboBox.SelectedIndex = 0;
            ClassComboBox.SelectedIndex = 0;
            ActionComboBox.SelectedIndex = 1;

            logReaderService.AddListener(this);
            eventService.AddSubscriber(this);

            GuildTextBox.TextChanged += GuildTextBox_TextChanged;
            ClassComboBox.SelectionChanged += ClassComboBox_SelectionChanged;
            LfgComboBox.SelectionChanged += LfgComboBox_SelectionChanged;
        }

        private void LfgComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFiltersAndItemSource();
        }

        private void ClassComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFiltersAndItemSource();
        }

        private void GuildTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFiltersAndItemSource();
        }

        protected void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ((ListViewItem)sender).Content as WhoResult;
            var action = ((ComboBoxItem)ActionComboBox.SelectedItem).Content.ToString();

            if (action == "Note")
            {
                EditNote(item);
            }
            else if (action == "Tell")
            {
                PutTellOnClipboard(item);
            }
            else if (action == "Consent")
            {
                PutConsentOnClipboard(item);
            }
            else if (action == "Target")
            {
                PutTargetOnClipboard(item);
            }
        }

        public void HandleLogLine(string serverName, string characterName, DateTime eventDate, string line)
        {
            if(line.StartsWith("----------"))
            {
                _isInWhoResults = true;
                _resultsLines.Clear();
                return;
            }

            if(!_isInWhoResults)
            {
                return;
            }

            if (line.StartsWith("There are ") || line.StartsWith("There is ") || line.StartsWith("Your who request was cut short"))
            {
                _isInWhoResults = false;
                var defaultZoneName = _lineParserService.GetZoneNameFromResultsEndcap(line);

                var results = _resultsLines
                    .Select(x => _lineParserService.ParseWhoResultLine(x.Item1, x.Item2, serverName, defaultZoneName))
                    .Where(x => x != null)
                    .OrderBy(x => string.IsNullOrEmpty(x.Note))
                    .ThenBy(x => x.Name)
                    .ToList();

                _results = results;

                Dispatcher.Invoke(() =>
                {
                    ApplyFiltersAndItemSource();
                });

                return;
            }

            _resultsLines.Add(Tuple.Create(line, eventDate));
        }

        private void ApplyFiltersAndItemSource()
        {
            if(_isResetting)
            {
                return;
            }

            var filtered = _results;
            if(!string.IsNullOrWhiteSpace(GuildTextBox.Text))
            {
                filtered = filtered.Where(x => x.Guild != null && x.Guild.Contains(GuildTextBox.Text, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }

            var classSelection = ((ComboBoxItem)ClassComboBox.SelectedItem).Content.ToString();
            filtered = ApplyClassFilter(filtered, classSelection);

            var lfgSelection = ((ComboBoxItem)LfgComboBox.SelectedItem).Content.ToString();
            if(lfgSelection == "Yes")
            {
                filtered = filtered.Where((x) => x.LookingForGroup != null && x.LookingForGroup == "LFG").ToList();
            }
            else if(lfgSelection == "No")
            {
                filtered = filtered.Where((x) => string.IsNullOrWhiteSpace(x.LookingForGroup)).ToList();
            }

            _filtered = filtered;
            ResultsListView.ItemsSource = filtered;
            UpdateGuildStatistics(filtered);
            UpdateClassStatistics(filtered);
        }

        private void UpdateGuildStatistics(List<WhoResult> filtered)
        {
            const string Unguilded = "Unguilded";

            var guildNames = filtered.Select(x => x.Guild).Distinct().OrderBy(x => x).ToList();

            var summaries = guildNames.Select(x => $"{(string.IsNullOrEmpty(x) ? Unguilded : x)}: {filtered.Count(f => f.Guild == x)}").ToList();
            var labelText = string.Join(" | ", summaries);
            GuildSummaryLabel.Text = labelText;
        }

        private void UpdateClassStatistics(List<WhoResult> filtered)
        {
            var classNames = filtered.Select(x => x.Class).Distinct().OrderBy(x => x).ToList();

            var summaries = classNames.Select(x => $"{x}: {filtered.Count(f => f.Class == x)}").ToList();
            var labelText = string.Join(" | ", summaries);
            ClassSummaryLabel.Text = labelText;
        }

        private List<WhoResult> ApplyClassFilter(List<WhoResult> items, string classSelection)
        {
            if (classSelection == "Any")
            {
                return items;
            }

            if(classSelection == "Unknown/Anonymous")
            {
                return items.Where(x => x.Class == "Unknown").ToList();
            }

            if (classSelection == "+Melee DPS")
            {
                return items.Where(x => x.Class == "Rogue" || x.Class == "Monk" || x.Class == "Shadow Knight" || x.Class == "Paladin").ToList();
            }

            if (classSelection == "+Tank")
            {
                return items.Where(x => x.Class == "Warrior" || x.Class == "Paladin" || x.Class == "Shadow Knight").ToList();
            }

            if (classSelection == "+Priest")
            {
                return items.Where(x => x.Class == "Druid" || x.Class == "Shaman" || x.Class == "Cleric").ToList();
            }

            if (classSelection == "+Porter")
            {
                return items.Where(x => x.Class == "Druid" || x.Class == "Wizard").ToList();
            }

            return items.Where(x => x.Class == classSelection).ToList();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            var selected = ResultsListView.SelectedItem as WhoResult;
            var header = menuItem.Header.ToString();
            if(header == "Tell")
            {
                PutTellOnClipboard(selected);
            }
            else if (header == "Target")
            {
                PutTargetOnClipboard(selected);
            }
            else if (header == "Consent")
            {
                PutConsentOnClipboard(selected);
            }
            else if(header == "Note")
            {
                EditNote(selected);
            }
        }

        private void EditNote(WhoResult whoResult)
        {
            var editNoteWindow = new EditNoteWindow();

            var note = new NoteItem
            {
                Note = whoResult.Note,
                ServerName = whoResult.ServerName,
                CharacterName = whoResult.Name
            };

            editNoteWindow.Note = note;
            editNoteWindow.ShowDialog();
            whoResult.Note = editNoteWindow.Note.Note;
        }

        private void PutTargetOnClipboard(WhoResult whoResult)
        {
            var command = $"/tar {whoResult.Name}";
            Clipboard.SetText(command);
        }

        private void PutTellOnClipboard(WhoResult whoResult)
        {
            var command = $"/tell {whoResult.Name}";
            Clipboard.SetText(command);
        }

        private void PutConsentOnClipboard(WhoResult whoResult)
        {
            var command = $"/consent {whoResult.Name}";
            Clipboard.SetText(command);
        }

        public Task Handle(NoteUpdateEvent value)
        {
            var note = value.Note;
            var item = _results.SingleOrDefault(x => x.Name.EqualsIngoreCase(note.CharacterName) && x.ServerName.EqualsIngoreCase(note.ServerName));

            if(item != null)
            {
                item.Note = note.Note;
            }

            return Task.CompletedTask;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            _isResetting = true;
            try
            {
                GuildTextBox.Clear();
                ClassComboBox.SelectedIndex = 0;
                LfgComboBox.SelectedIndex = 0;
                _isResetting = false;
                ApplyFiltersAndItemSource();

            }
            finally
            {
                _isResetting = false;
            }
        }

        private void EncounterLogsButton_Click(object sender, RoutedEventArgs e)
        {
            var single = SingleWindowManager.GetWindow<EncounterLogsWindow>();
            single.Window.Logs = _filtered;
            single.ShowOrActivate();
        }
    }
}
