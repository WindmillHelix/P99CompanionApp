using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WindmillHelix.Companion99.App.Events;
using WindmillHelix.Companion99.Services;

namespace WindmillHelix.Companion99.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IEventSubscriber<EverQuestFolderConfiguredEvent>
    {
        public App()
        {
            var startupService = DependencyInjector.Resolve<IStartupService>();
            var eventService = DependencyInjector.Resolve<IEventService>();

            startupService.EnsureDataDirectoryExists();
            startupService.InitializeDatabase();

            if(startupService.IsEverQuestDirectoryValid())
            {
                //  && startupService.IsEverQuestDirectoryValid()
                this.StartupUri = new Uri("MainWindow.xaml", UriKind.RelativeOrAbsolute);
            }
            else
            {
                eventService.AddSubscriber<EverQuestFolderConfiguredEvent>(this);
                this.StartupUri = new Uri("SetupWindow.xaml", UriKind.RelativeOrAbsolute);
            }
        }

        public Task Handle(EverQuestFolderConfiguredEvent value)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.MainWindow = new MainWindow();
                this.MainWindow.Show();
            });

            return Task.CompletedTask;
        }
    }
}
