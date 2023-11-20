using AJWPFAdmin.Services.EF;
using Masuit.Tools.Security;
using MaterialDesignExtensions.Controls;
using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using Prism.Ioc;
using System.Threading.Tasks;
using System;
using EntranceGuardManager.Views;
using Prism.Events;
using static AJWPFAdmin.Core.ExceptionTool;
using AJWPFAdmin.Services.EF.Tables;
using Prism.Regions;
using System.ComponentModel;
using MaterialDesignThemes.Wpf;
using AJWPFAdmin.Core.Validation;
using AJWPFAdmin.Services;
using Prism.Services.Dialogs;
using AJWPFAdmin.Modules.Common.Views;

namespace EntranceGuardManager.ViewModels
{
    public class LoginWindowViewModel : AnnotationValidationViewModel
    {
        private string _account = string.Empty;
        [Required(ErrorMessage = "账户名未填写")]
        public string Account
        {
            get { return _account; }
            set
            {
                SetProperty(ref _account, value);
                LoginCmd.RaiseCanExecuteChanged();
            }
        }

        private string _password = string.Empty;
        [Required(ErrorMessage = "账号密码未填写")]
        public string Password
        {
            get { return _password; }
            set
            {
                SetProperty(ref _password, value);
                LoginCmd.RaiseCanExecuteChanged();
            }
        }

        private bool _loading;
        public bool Loading
        {
            get { return _loading; }
            set { SetProperty(ref _loading, value); }
        }

        private DelegateCommand _openDatabaseConfigCmd;
        public DelegateCommand OpenDatabaseConfigCmd =>
            _openDatabaseConfigCmd ?? (_openDatabaseConfigCmd = new DelegateCommand(ExecuteOpenDatabaseConfigCmd));

        void ExecuteOpenDatabaseConfigCmd()
        {
            _dialogService.ShowDialog(nameof(DatabaseConfigDialog), (r) =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    _sysUserSvc.RefreshDb();
                }
            });
        }

        private DelegateCommand _loginCmd;
        public DelegateCommand LoginCmd =>
            _loginCmd ?? (_loginCmd = new DelegateCommand(ExecuteLoginCmdAsync, CanExecuteLoginCmd));

        private DelegateCommand _closedCmd;
        public DelegateCommand ClosedCmd =>
            _closedCmd ?? (_closedCmd = new DelegateCommand(ExecuteClosedCmd));

        void ExecuteClosedCmd()
        {
            _globalExceptionEvt?.Unsubscribe(OnGlobalException);
        }

        void ExecuteLoginCmdAsync()
        {
            Loading = true;

            var worker = new BackgroundWorker
            {
                WorkerReportsProgress = false,
                WorkerSupportsCancellation = false,
            };
            worker.DoWork += (sneder, e) =>
            {
                _sysUserSvc.Login(_account, _password);
            };
            worker.RunWorkerCompleted += async (sender, e) =>
            {
                Loading = false;

                if (e.Error != null)
                {
                    await AlertDialog.ShowDialogAsync("root", new AlertDialogArguments
                    {
                        Title = "登录失败",
                        Message = e.Error.Message
                    });
                    return;
                }

                var mwd = _container.Resolve<MainWindow>();
                RegionManager.SetRegionManager(mwd, _container.Resolve<IRegionManager>());
                mwd.Show();
                Application.Current.MainWindow.Close();
                Application.Current.MainWindow = mwd;
            };

            worker.RunWorkerAsync();


        }

        bool CanExecuteLoginCmd()
        {
            return !string.IsNullOrEmpty(_account) && !string.IsNullOrEmpty(_password);
        }

        private IContainerProvider _container;
        private IDialogService _dialogService;
        private SystemUserService _sysUserSvc;
        private AJGlobalExceptionResolvedEvent _globalExceptionEvt;

        void OnGlobalException()
        {
            Loading = false;
        }

        public LoginWindowViewModel(IContainerProvider container, IEventAggregator eventAggregator,
            IDialogService dialogService,
            SystemUserService sysUserSvc)
        {
            _sysUserSvc = sysUserSvc;
            _container = container;
            _dialogService = dialogService;
            _globalExceptionEvt = eventAggregator.GetEvent<AJGlobalExceptionResolvedEvent>();
            _globalExceptionEvt.Subscribe(OnGlobalException);

        }
    }
}
