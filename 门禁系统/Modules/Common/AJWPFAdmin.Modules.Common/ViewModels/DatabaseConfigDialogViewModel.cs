using AJWPFAdmin.Core.Components.AJTable.ViewModels;
using AJWPFAdmin.Core.Utils;
using AJWPFAdmin.Core.Validation;
using AJWPFAdmin.Core;
using AJWPFAdmin.Services.EF.Tables;
using AJWPFAdmin.Services.EF;
using AJWPFAdmin.Services;
using MaterialDesignExtensions.Controls;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AJWPFAdmin.Core.Components.AJTable.Views.AJTable;
using static AJWPFAdmin.Core.ExceptionTool;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Masuit.Tools.Models;
using Prism.Services.Dialogs;

namespace AJWPFAdmin.Modules.Common.ViewModels
{
    public class DatabaseConfigDialogViewModel : AnnotationValidationViewModel, IDialogAware
    {
        private bool _loading;
        public bool Loading
        {
            get { return _loading; }
            set
            {
                SetProperty(ref _loading, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private string _dataSource;
        [Display(Name = "服务器地址")]
        [Required(ErrorMessage = "{0}必填")]
        public string DataSource
        {
            get { return _dataSource; }
            set
            {
                SetProperty(ref _dataSource, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private string _databaseName;

        private string _uid;
        [Display(Name = "数据库账号")]
        [Required(ErrorMessage = "{0}必填")]
        public string UId
        {
            get { return _uid; }
            set
            {
                SetProperty(ref _uid, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private string _password;
        [Display(Name = "数据库密码")]
        [Required(ErrorMessage = "{0}必填")]
        public string Password
        {
            get { return _password; }
            set
            {
                SetProperty(ref _password, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private DelegateCommand _saveCmd;
        public DelegateCommand SaveCmd =>
            _saveCmd ?? (_saveCmd = new DelegateCommand(ExecuteSaveCmdAsync, CanExecuteSaveCmd));

        public string Title => throw new NotImplementedException();

        async void ExecuteSaveCmdAsync()
        {
            if (!ValidateModel())
            {
                SaveCmd.RaiseCanExecuteChanged();
                return;
            }

            Loading = true;

            var str = $"server={_dataSource};user={_uid};password={_password};database={_databaseName}";

            var file = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

            await Task.Delay(2000);

            var obj = CommonUtil.TryGetJSONObject<JObject>(await File.ReadAllTextAsync(file));

            obj["ConnectionStrings"]["MYSQL"] = str;

            _cfgSvc.Config["ConnectionStrings:MYSQL"] = str;
            _cfgSvc.Config.Reload();

            await File.WriteAllTextAsync(file, obj.ToString());

            await CommonUtil.ShowAlertDialogAsync(new AlertDialogArguments
            {
                Title = "提示",
                Message = "保存成功!"
            },DialogIds.DialogWindow);

            Loading = false;

            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));

        }

        bool CanExecuteSaveCmd()
        {
            return !HasErrors;
        }

        private AJConfigService _cfgSvc;
        private AJGlobalExceptionResolvedEvent _globalExceptionEvt;

        public event Action<IDialogResult> RequestClose;

        public DatabaseConfigDialogViewModel(AJConfigService cfgSvc, 
            IEventAggregator eventAggregator)
        {
            _cfgSvc = cfgSvc;
            _globalExceptionEvt = eventAggregator.GetEvent<AJGlobalExceptionResolvedEvent>();
            _globalExceptionEvt.Subscribe(() =>
            {
                Loading = false;
            });
            
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var mySQLStrData = CommonUtil.DecryptMySqlConnStr(_cfgSvc.Config.GetConnectionString("MYSQL"));

            DataSource = mySQLStrData.host;
            UId = mySQLStrData.user;
            Password = mySQLStrData.password;
            _databaseName = mySQLStrData.database;
        }
    }
}
