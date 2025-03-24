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
    /// Interaction logic for EditLog2DiscordWindow.xaml
    /// </summary>
    public partial class EditLog2DiscordWindow : Window
    {
        private readonly IDiscordConfigService _configService;
        private Guid _entryId;

        public EditLog2DiscordWindow()
        {
            _configService = DependencyInjector.Resolve<IDiscordConfigService>();
            InitializeComponent();
        }

        public void SetEntry(Log2DiscordConfigEntry entry)
        {
            _entryId = entry.Log2DiscordConfigEntryId;
            NameTextBox.Text = entry.Name;
            RegexTextBox.Text = entry.Regex;
            ChannelIdTextBox.Text = entry.TargetChannelId <= 0 ? string.Empty : entry.TargetChannelId.ToString();
            EnabledCheckBox.IsChecked = entry.IsEnabled;

            var config = _configService.GetConfiguration();
            DiscordAccountsComboBox.ItemsSource = config.Accounts.OrderBy(x => x.Name).ToList();
            if (entry.DiscordAccountId != Guid.Empty)
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

            var entry = new Log2DiscordConfigEntry
            {
                Log2DiscordConfigEntryId = _entryId,
                DiscordAccountId = (DiscordAccountsComboBox.SelectedItem as DiscordAccount)?.DiscordAccountId ?? Guid.Empty,
                Regex = RegexTextBox.Text,
                Name = NameTextBox.Text,
                TargetChannelId = channelId,
                IsEnabled = EnabledCheckBox.IsChecked ?? false
            };

            var saveResult = await _configService.SaveLog2DiscordAsync(entry);
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
