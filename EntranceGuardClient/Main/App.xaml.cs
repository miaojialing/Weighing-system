using AJWPFAdmin.Views;
using AJWPFAdmin.Core;
using AJWPFAdmin.Core.Logger;
//using AJWPFAdmin.Modules.Header;
using AJWPFAdmin.Modules.Main;
using AJWPFAdmin.Modules.SideMenu;
using AJWPFAdmin.Services;
using AJWPFAdmin.Services.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;
using System.Globalization;
using System.Threading;
using System.Collections;
using AJWPFAdmin.Services.Jobs;
using System.Collections.Generic;
using AJWPFAdmin.Core.HardwareSDKS;
using AJWPFAdmin.Core.Utils;
using System.IO;
using Prism.Events;
using AJWPFAdmin.Core.GlobalEvents;
using System.Text;
using AJWPFAdmin.Modules.Common;
using AJWPFAdmin.Modules.Common.Views;

namespace AJWPFAdmin
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<LoginWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<AJConfigService>();
            containerRegistry.RegisterSingleton<DbService>(provider =>
            {
                var cfgSvc = provider.Resolve<AJConfigService>();

                var builder = new DbContextOptionsBuilder<DbService>();
                var version = new MySqlServerVersion(ServerVersion.Parse("8.0.34-mysql"));
                builder.UseMySql(cfgSvc.Config.GetConnectionString("MYSQL"), version,
                    bd =>
                    {
                        bd.EnableRetryOnFailure(3);
                    }).EnableDetailedErrors()
                    .LogTo(log => Debug.WriteLine(log),
                     new [] { 
                         DbLoggerCategory.Database.Command.Name, 
                         DbLoggerCategory.Update.Name,
                         DbLoggerCategory.Query.Name 
                     });
                return new DbService(builder.Options);
            });
            containerRegistry.RegisterSingleton<AJLog4NetLogger>();
            containerRegistry.RegisterSingleton<ExceptionTool>();
            containerRegistry.RegisterSingleton<AJDatabaseckupService>();
            containerRegistry.RegisterSingleton<SystemUserService>();
            containerRegistry.RegisterSingleton<QuarztTaskService>();
            containerRegistry.RegisterSingleton<AJMQTTService>();
            containerRegistry.RegisterDialogWindow<DialogWindow>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<MainModule>();
            moduleCatalog.AddModule<SideMenuModule>();
            moduleCatalog.AddModule<CommonModule>();
        }

        protected async override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //  注册 GB2312
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var task = Container.Resolve<QuarztTaskService>();
            await task.StartAsync(Container,new List<IAJJob>
            {
                new DatabaseBackupJob()
            });
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            //LiveCharts.Configure(cfg =>
            //{

            //});
        }

        protected async override void OnExit(ExitEventArgs e)
        {
            var task = Container.Resolve<QuarztTaskService>();
            await task.StopAsync();

            var mqttSvc = Container.Resolve<AJMQTTService>();
            await mqttSvc.CloseAsync();

            var eventAggregator = Container.Resolve<IEventAggregator>();

            eventAggregator.GetEvent<ApplicationExitEvent>().Publish();

            base.OnExit(e);
        }

        private async void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                if (e.IsTerminating)
                {
                    var ext = Container.Resolve<ExceptionTool>();
                    await ext.LogExceptionAsync((e.ExceptionObject as Exception));

                }
            }
            catch
            {

            }
            
        }

        private async void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                e.SetObserved();
                var ext = Container.Resolve<ExceptionTool>();
                await ext.LogExceptionAsync(e.Exception);
            }
            catch
            {
            }

        }

        private async void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                e.Handled = true;
                var ext = Container.Resolve<ExceptionTool>();
                await ext.LogExceptionAsync(e.Exception);
                
            }
            catch
            {

            }
            

        }
    }
}
