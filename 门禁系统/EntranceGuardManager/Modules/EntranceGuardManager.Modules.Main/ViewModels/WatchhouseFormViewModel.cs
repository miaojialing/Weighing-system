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
    public class WatchhouseFormViewModel : AnnotationValidationViewModel, IDialogAware
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

        private Watchhouse _form;

        private string _name;
        [Display(Name = "名称")]
        [Required(ErrorMessage = "{0}必填")]
        [StringLength(50, ErrorMessage = "{0}超长:{1}")]
        public string Name
        {
            get { return _name; }
            set
            {
                SetProperty(ref _name, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private string _ip;
        [Display(Name = "IP地址")]
        [AJFormIPField]
        public string IP
        {
            get { return _ip; }
            set
            {
                SetProperty(ref _ip, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private string _remark;
        [Display(Name = "备注")]
        [StringLength(100, ErrorMessage = "{0}超长:{1}")]
        public string Remark
        {
            get { return _remark; }
            set
            {
                SetProperty(ref _remark, value);
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
            _form.IP = IP;
            _form.Remark = Remark;

            if (_form.Id == 0)
            {
                if (db.Watchhouses.Any(p => p.Name == name))
                {
                    ExceptionTool.FriendlyError("岗亭名称已存在", dialogId: DialogIds.DialogWindow);
                    return;
                }
                _form.Name = name;
                _form.Id = SnowFlake.GetInstance().GetLongId();
                _form.CreateDate = DateTime.Now;
                db.Watchhouses.Add(_form);
            }
            else
            {

                if (_form.Name != name && db.Watchhouses.Any(p => p.Name == name))
                {
                    ExceptionTool.FriendlyError("岗亭名称已存在", dialogId: DialogIds.DialogWindow);
                    return;
                }
                _form.Name = name;
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
            parameters.TryGetValue<Watchhouse>("data", out var data);

            parameters.TryGetValue<MaterialType>("type", out var type);

            if (data != null)
            {
                _form = data;

                Name = _form.Name;
                IP = _form.IP;
                Remark = _form.Remark;
            }

            DialogTitle = data == null ? $"新增岗亭" : $"编辑 {data.Name}";
        }

        private DbService db;
        private AJGlobalExceptionResolvedEvent _globalExceptionEvt;

        public WatchhouseFormViewModel(DbService dbIns, IEventAggregator eventAggregator)
        {
            _form = new Watchhouse();
            _globalExceptionEvt = eventAggregator.GetEvent<AJGlobalExceptionResolvedEvent>();
            _globalExceptionEvt.Subscribe(() =>
            {
                Loading = false;
            });
            db = dbIns;
        }
    }
}
