﻿using AJWPFAdmin.Core;
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
    public class CarTypeFormViewModel : AnnotationValidationViewModel, IDialogAware
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

        private CarType _form;

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

        private bool _autoPass;
        public bool AutoPass
        {
            get { return _autoPass; }
            set
            {
                SetProperty(ref _autoPass, value);
            }
        }

        private bool _enablePassagewayStatistic;
        public bool EnablePassagewayStatistic
        {
            get { return _enablePassagewayStatistic; }
            set
            {
                SetProperty(ref _enablePassagewayStatistic, value);
            }
        }

        private bool _sysRequired;
        public bool SysRequired
        {
            get { return _sysRequired; }
            set
            {
                SetProperty(ref _sysRequired, value);
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
            _form.AutoPass = AutoPass;
            _form.SysRequired = SysRequired;
            _form.EnablePassagewayStatistic = EnablePassagewayStatistic;

            if (_form.Id == 0)
            {
                if (db.CarTypes.Any(p => p.Name == name))
                {
                    ExceptionTool.FriendlyError("车辆类型已存在", dialogId: DialogIds.DialogWindow);
                    return;
                }
                _form.Name = Name;
                _form.Id = SnowFlake.GetInstance().GetLongId();
                _form.CreateDate = DateTime.Now;
                db.CarTypes.Add(_form);
            }
            else
            {
                if (_form.Name != name && db.CarTypes.Any(p => p.Name == name))
                {
                    ExceptionTool.FriendlyError("车辆类型已存在", dialogId: DialogIds.DialogWindow);
                    return;
                }
                _form.Name = Name;
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
            parameters.TryGetValue<CarType>("data", out var data);
            DialogTitle = data == null ? $"新增类型" : $"编辑 {data.Name}";

            if (data != null)
            {
                _form = data;
                
                Name = _form.Name;
                AutoPass = _form.AutoPass;
                SysRequired = _form.SysRequired;
                EnablePassagewayStatistic = _form.EnablePassagewayStatistic;
            }

        }

        private DbService db;
        private AJGlobalExceptionResolvedEvent _globalExceptionEvt;

        public CarTypeFormViewModel(DbService dbIns, IEventAggregator eventAggregator)
        {
            _form = new CarType();
            _globalExceptionEvt = eventAggregator.GetEvent<AJGlobalExceptionResolvedEvent>();
            _globalExceptionEvt.Subscribe(() =>
            {
                Loading = false;
            });
            db = dbIns;
        }
    }
}
