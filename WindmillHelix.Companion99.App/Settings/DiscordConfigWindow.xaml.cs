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
using WindmillHelix.Companion99.Services;
using WindmillHelix.Companion99.Services.Discord;

namespace WindmillHelix.Companion99.App.Settings
{
    /// <summary>
    /// Interaction logic for DiscordConfigWindow.xaml
    /// </summary>
    public partial class DiscordConfigWindow : Window, IEventSubscriber<DiscordConfiguration>
    {
        private readonly IDiscordConfigService _discordConfigService;

        public DiscordConfigWindow()
        {
            _discordConfigService = DependencyInjector.Resolve<IDiscordConfigService>();

            InitializeComponent();
            this.SetupDefaults();

            var eventService = DependencyInjector.Resolve<IEventService>();
            eventService.AddSubscriber<DiscordConfiguration>(this);

            var config = _discordConfigService.GetConfiguration();
            LoadValues(config);
        }

        private void NewAccountButton_Click(object sender, RoutedEventArgs e)
        {
            var account = new DiscordAccount();
            account.DiscordAccountId = Guid.NewGuid();

            var window = DependencyInjector.Resolve<EditDiscordAccountWindow>();
            window.SetAccount(account);
            window.ShowDialog();
        }

        private void LoadValues(DiscordConfiguration config)
        {
            AccountsListView.ItemsSource = config.Accounts.OrderBy(x => x.Name).ToList();
            Log2DiscordListView.ItemsSource = config.Log2DiscordConfigEntries.OrderBy(x => x.Name).ToList();
            Discord2LogListView.ItemsSource = config.Discord2LogConfigEntries.OrderBy(x => x.Name).ToList();

            NewLog2DiscordButton.IsEnabled = config.Accounts.Count > 0;
        }

        private void DeleteAccountButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            var account = button?.Tag as DiscordAccount;
            if(account == null)
            {
                return;
            }

            var result = _discordConfigService.DeleteAccount(account.DiscordAccountId);
            if(!result.WasSuccessful)
            {
                MessageBox.Show(result.FailureReason, "Delete Account Failed");
            }
        }

        public Task Handle(DiscordConfiguration value)
        {
            LoadValues(value);
            return Task.CompletedTask;
        }

        private void NewLog2DiscordButton_Click(object sender, RoutedEventArgs e)
        {
            var entry = new Log2DiscordConfigEntry();
            entry.Log2DiscordConfigEntryId = Guid.NewGuid();
            entry.IsEnabled = true;

            var window = DependencyInjector.Resolve<EditLog2DiscordWindow>();
            window.SetEntry(entry);
            window.ShowDialog();
        }

        private void DeleteLog2DiscordButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            var entry = button?.Tag as Log2DiscordConfigEntry;
            if (entry == null)
            {
                return;
            }

            var result = AsyncHelper.RunSynchronously(() => _discordConfigService.DeleteLog2DiscordAsync(entry.Log2DiscordConfigEntryId));
            if (!result.WasSuccessful)
            {
                MessageBox.Show(result.FailureReason, "Delete Log2Discord Failed");
            }
        }

        protected void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ((ListViewItem)sender).Content;
            if(item is DiscordAccount account)
            {
                var window = DependencyInjector.Resolve<EditDiscordAccountWindow>();
                window.SetAccount(account);
                window.ShowDialog();
            }
            else if(item is Log2DiscordConfigEntry log2Discord)
            {
                var window = DependencyInjector.Resolve<EditLog2DiscordWindow>();
                window.SetEntry(log2Discord);
                window.ShowDialog();
            }
            else if(item is Discord2LogConfigEntry discord2Log)
            {
                var window = DependencyInjector.Resolve<EditDiscord2LogWindow>();
                window.SetEntry(discord2Log);
                window.ShowDialog();
            }
        }

        private void NewDiscord2LogButton_Click(object sender, RoutedEventArgs e)
        {
            var entry = new Discord2LogConfigEntry();
            entry.Discord2LogConfigEntryId = Guid.NewGuid();
            entry.IsEnabled = true;

            var window = DependencyInjector.Resolve<EditDiscord2LogWindow>();
            window.SetEntry(entry);
            window.ShowDialog();
        }

        private void DeleteDiscord2LogButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            var entry = button?.Tag as Discord2LogConfigEntry;
            if (entry == null)
            {
                return;
            }

            var result = AsyncHelper.RunSynchronously(() => _discordConfigService.DeleteDiscord2LogAsync(entry.Discord2LogConfigEntryId));
            if (!result.WasSuccessful)
            {
                MessageBox.Show(result.FailureReason, "Delete Discord2Log Failed");
            }
        }
    }
}
