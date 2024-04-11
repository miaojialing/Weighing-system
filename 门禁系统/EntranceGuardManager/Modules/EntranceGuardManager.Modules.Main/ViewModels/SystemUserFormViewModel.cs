using AJWPFAdmin.Core;
using AJWPFAdmin.Core.Components.AJTable.ViewModels;
using AJWPFAdmin.Core.Enums;
using AJWPFAdmin.Core.ExtensionMethods;
using AJWPFAdmin.Core.Utils;
using AJWPFAdmin.Core.Validation;
using AJWPFAdmin.Services.EF;
using AJWPFAdmin.Services.EF.Tables;
using Masuit.Tools;
using Masuit.Tools.Security;
using Masuit.Tools.Systems;
using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static AJWPFAdmin.Core.ExceptionTool;

namespace EntranceGuardManager.Modules.Main.ViewModels
{
    public class SystemUserFormViewModel : AnnotationValidationViewModel, IDialogAware
    {
        private string _dialogTitle;
        public string DialogTitle
        {
            get { return _dialogTitle; }
            set { SetProperty(ref _dialogTitle, value); }
        }

        private bool _loading;
        public bool Loading
        {
            get { return _loading; }
            set { SetProperty(ref _loading, value); }
        }

        private SystemUser _form;

        private string _accountName;
        [Display(Name = "登录账号")]
        [Required(ErrorMessage = "{0}必填")]
        [StringLength(20, ErrorMessage = "账户名称超长:{1}")]
        public string AccountName
        {
            get { return _accountName; }
            set
            {
                SetProperty(ref _accountName, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private string _password;
        [Display(Name = "登录密码")]
        [AJFormPasswordField]
        public string Password
        {
            get { return _password; }
            set
            {
                SetProperty(ref _password, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private bool _enable;
        public bool Enable
        {
            get { return _enable; }
            set { SetProperty(ref _enable, value); }
        }

        private AJTableFormSchemaItemOptions _roleId;
        [Display(Name = "所属角色")]
        [Required( ErrorMessage = "{0}未设置")]
        public AJTableFormSchemaItemOptions RoleId
        {
            get { return _roleId; }
            set { SetProperty(ref _roleId, value); }
        }

        private ObservableCollection<AJTableFormSchemaItemOptions> _roleOptions;
        public ObservableCollection<AJTableFormSchemaItemOptions> RoleOptions
        {
            get { return _roleOptions; }
            set { SetProperty(ref _roleOptions, value); }
        }

        private DelegateCommand _saveCmd;
        public DelegateCommand SaveCmd =>
            _saveCmd ?? (_saveCmd = new DelegateCommand(ExecuteSaveCmdAsync, CanExecuteSaveCmd));

        async void ExecuteSaveCmdAsync()
        {
            if (!ValidateModel())
            {
                SaveCmd.RaiseCanExecuteChanged();
                //ExceptionTool.FriendlyError("所提交数据有误", dialogId: DialogIds.DialogWindow);
                return;
            }

            Loading = true;

            var name = AccountName;
            _form.Phone = "";
            _form.NickName = "";
            _form.RoleId = (long)RoleId.Value;
            _form.RoleName = RoleId.Label;
            _form.Enable = Enable;
            _form.Password = Password.AESEncrypt(AJWPFAdmin.Core.Properties.Resources.AESKey);

            if (_form.Id == 0)
            {
                if (db.SystemUsers.Any(p => p.AccountName == name))
                {
                    FriendlyError("账号已存在", dialogId: DialogIds.DialogWindow);
                    return;
                }
                _form.AccountName = name;
                _form.Id = SnowFlake.GetInstance().GetLongId();
                _form.UpdateDate = _form.CreateDate = DateTime.Now;
                db.SystemUsers.Add(_form);
            }
            else
            {

                if (_form.AccountName != name && db.SystemUsers.Any(p => p.AccountName == name))
                {
                    FriendlyError("账号已存在", dialogId: DialogIds.DialogWindow);
                    return;
                }
                _form.AccountName = name;
                _form.UpdateDate = DateTime.Now;
                db.Entry(_form).State = EntityState.Modified;
            }

            await db.SaveChangesAsync();

            Loading = false;

            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }

        bool CanExecuteSaveCmd()
        {
            return !HasErrors;
        }

        public string Title => string.Empty;

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public async void OnDialogOpened(IDialogParameters parameters)
        {
            parameters.TryGetValue<SystemUser>("data", out var data);

            DialogTitle = data == null ? $"新增账户" : $"编辑 {data.AccountName}";

            var roles = await db.SystemRoles.AsNoTracking()
                .OrderByDescending(p => p.CreateDate).Select(p => new AJTableFormSchemaItemOptions
                {
                    Label = p.Name,
                    Value = p.Id
                }).ToListAsync();

            RoleOptions = new ObservableCollection<AJTableFormSchemaItemOptions>(roles);

            if (data != null)
            {
                _form = data;

                AccountName = _form.AccountName;
                Password = _form.Password.AESDecrypt(AJWPFAdmin.Core.Properties.Resources.AESKey);
                Enable = _form.Enable;
                RoleId = RoleOptions.FirstOrDefault(p => (long)p.Value == _form.RoleId);
            }
            else
            {
                RoleId = RoleOptions.FirstOrDefault();
                Enable = true;
            }

        }

        private DbService db;
        private AJGlobalExceptionResolvedEvent _globalExceptionEvt;

        public SystemUserFormViewModel(DbService dbIns, IEventAggregator eventAggregator)
        {
            _form = new SystemUser();
            _globalExceptionEvt = eventAggregator.GetEvent<AJGlobalExceptionResolvedEvent>();
            _globalExceptionEvt.Subscribe(() =>
            {
                Loading = false;
            });
            db = dbIns;
        }
    }
}
