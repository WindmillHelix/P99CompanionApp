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
using System.Windows.Shapes;
using WindmillHelix.Companion99.Common.Threading;
using WindmillHelix.Companion99.Services.Discord;

namespace WindmillHelix.Companion99.App.Settings
{
    /// <summary>
    /// Interaction logic for EditDiscord2LogWindow.xaml
    /// </summary>
    public partial class EditDiscord2LogWindow : Window
    {
        private readonly IDiscordConfigService _configService;
        private Guid _entryId;

        public EditDiscord2LogWindow()
        {
            _configService = DependencyInjector.Resolve<IDiscordConfigService>();
            InitializeComponent();
        }

        public void SetEntry(Discord2LogConfigEntry entry)
        {
            _entryId = entry.DiscordAccountId;
            NameTextBox.Text = entry.Name;

            ChannelIdTextBox.Text = entry.SourceChannelId <= 0 ? string.Empty : entry.SourceChannelId.ToString();
            FileNameTextBox.Text = entry.TargetFileName;
            PrefixTextBox.Text = entry.Prefix;
            FilterRegexTextBox.Text = entry.FilterRegex;
            RemoveRegexTextBox.Text = entry.RemoveRegex;
            EnabledCheckBox.IsChecked = entry.IsEnabled;

            var config = _configService.GetConfiguration();
            DiscordAccountsComboBox.ItemsSource = config.Accounts.OrderBy(x => x.Name).ToList();
            if(entry.DiscordAccountId != Guid.Empty)
            {
                DiscordAccountsComboBox.SelectedItem = config.Accounts.Single(x => x.DiscordAccountId == entry.DiscordAccountId);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            AsyncHelper.RunSynchronously(() => SaveAsync());
        }

        private async Task SaveAsync()
        {
            ulong.TryParse(ChannelIdTextBox.Text, out var channelId);

            var entry = new Discord2LogConfigEntry
            {
                DiscordAccountId = (DiscordAccountsComboBox.SelectedItem as DiscordAccount)?.DiscordAccountId ?? Guid.Empty,
                TargetFileName = FileNameTextBox.Text,
                FilterRegex = FilterRegexTextBox.Text,
                RemoveRegex = RemoveRegexTextBox.Text,
                IsEnabled = EnabledCheckBox.IsChecked ?? false,
                Prefix = PrefixTextBox.Text,
                Name = NameTextBox.Text,
                SourceChannelId = channelId
            };

            var saveResult = await _configService.SaveDiscord2LogAsync(entry);
            if (!saveResult.WasSuccessful)
            {
                ErrorLabel.Content = saveResult.FailureReason;
            }
            else
            {
                this.Close();
            }
        }
    }
}
