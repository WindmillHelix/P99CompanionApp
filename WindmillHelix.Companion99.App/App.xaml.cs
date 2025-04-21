using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WindmillHelix.Companion99.App.Events;
using WindmillHelix.Companion99.Common;
using WindmillHelix.Companion99.Services;

namespace WindmillHelix.Companion99.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IEventSubscriber<EverQuestFolderConfiguredEvent>
    {
        private readonly ILogService _logService;

        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            var startupService = DependencyInjector.Resolve<IStartupService>();
            var eventService = DependencyInjector.Resolve<IEventService>();
            _logService = DependencyInjector.Resolve<ILogService>();

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

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            _logService.LogException(e.Exception);
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            _logService.LogException(e.Exception);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logService.LogException(e.ExceptionObject as Exception);
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
