using AJWPFAdmin.Core;
using AJWPFAdmin.Core.Enums;
using AJWPFAdmin.Core.ExtensionMethods;
using AJWPFAdmin.Core.Utils;
using AJWPFAdmin.Core.Validation;
using AJWPFAdmin.Services.EF;
using AJWPFAdmin.Services.EF.Tables;
using Masuit.Tools;
using Masuit.Tools.Systems;
using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static AJWPFAdmin.Core.ExceptionTool;

namespace EntranceGuardManager.Modules.Main.ViewModels
{
    public class SystemRoleFormViewModel : AnnotationValidationViewModel, IDialogAware
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

        private SystemRole _form;

        private string _name;
        [Display(Name = "角色名称")]
        [Required(ErrorMessage = "{0}必填")]
        [StringLength(20, ErrorMessage = "{0}超长:{1}")]
        public string Name
        {
            get { return _name; }
            set
            {
                SetProperty(ref _name, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private string _description;
        [Display(Name = "备注")]
        [StringLength(200, ErrorMessage = "{0}超长:{1}")]
        public string Description
        {
            get { return _description; }
            set
            {
                SetProperty(ref _description, value);
            }
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

            var name = Name;
            _form.Description = Description;
            _form.Permission = "all";

            if (_form.Id == 0)
            {
                if (db.SystemRoles.Any(p => p.Name == name))
                {
                    FriendlyError("角色名称已存在", dialogId: DialogIds.DialogWindow);
                    return;
                }
                _form.Name = name;
                _form.Id = SnowFlake.GetInstance().GetLongId();
                _form.UpdateDate = _form.CreateDate = DateTime.Now;
                db.SystemRoles.Add(_form);
            }
            else
            {

                if (_form.Name != name && db.SystemRoles.Any(p => p.Name == name))
                {
                    FriendlyError("角色名称已存在", dialogId: DialogIds.DialogWindow);
                    return;
                }
                _form.Name = name;
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

        public void OnDialogOpened(IDialogParameters parameters)
        {
            parameters.TryGetValue<SystemRole>("data", out var data);

            if (data != null)
            {
                _form = data;

                Name = _form.Name;
                // = _form.IP;
                Description = _form.Description;
            }

            DialogTitle = data == null ? $"新增角色" : $"编辑 {data.Name}";
        }

        private DbService db;
        private AJGlobalExceptionResolvedEvent _globalExceptionEvt;

        public SystemRoleFormViewModel(DbService dbIns, IEventAggregator eventAggregator)
        {
            _form = new SystemRole();
            _globalExceptionEvt = eventAggregator.GetEvent<AJGlobalExceptionResolvedEvent>();
            _globalExceptionEvt.Subscribe(() =>
            {
                Loading = false;
            });
            db = dbIns;
        }
    }
}
