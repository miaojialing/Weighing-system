using AJWPFAdmin.Core.Logger;
using AJWPFAdmin.Core;
using AJWPFAdmin.Services.EF;
using AJWPFAdmin.Services;
using EntranceGuardManager.Views;
using Microsoft.EntityFrameworkCore;
using Prism.Ioc;
using System.Windows;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using AJWPFAdmin.Core.HardwareSDKS;
using AJWPFAdmin.Services.Jobs;
using Prism.Modularity;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using EntranceGuardManager.Modules.Main;
using EntranceGuardManager.Modules.SideMenu;
using AJWPFAdmin.Modules.Common;
using AJWPFAdmin.Modules.Common.Views;

namespace EntranceGuardManager
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
                     new[] {
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

            var task = Container.Resolve<QuarztTaskService>();
            await task.StartAsync(Container, new List<IAJJob>
            {
                new DatabaseBackupJob(),
                new CarExpiredCheckJob()
            });
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            // 加载硬件SDKdll 路径到环境变量
            //var baseDir = Directory.GetCurrentDirectory();
            //var b1 = AppDomain.CurrentDomain.BaseDirectory;

            //CommonUtil.AddEnvironmentPaths(new string[] { Path.Combine(baseDir, @"HardwareSDKs\VzClientSDK\") });

            //LiveCharts.Configure(cfg =>
            //{

            //});
        }

        protected async override void OnExit(ExitEventArgs e)
        {
            var task = Container.Resolve<QuarztTaskService>();
            await task.StopAsync();
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
