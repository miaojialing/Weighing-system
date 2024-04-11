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

namespace AJWPFAdmin.Modules.Main.ViewModels
{
    public class MaterialDictionaryFormViewModel : AnnotationValidationViewModel, IDialogAware
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

        private MaterialDictionary _form;

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

        private string _sortNo;
        [Display(Name = "序号")]
        [AJFormIntegerField]
        public string SortNo
        {
            get { return _sortNo; }
            set
            {
                SetProperty(ref _sortNo, value);
                SaveCmd.RaiseCanExecuteChanged();
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

            _form.Name = Name;
            _form.SortNo = SortNo.TryGetInt();

            if (_form.Id == 0)
            {
                _form.Id = SnowFlake.GetInstance().GetLongId();
                _form.CreateDate = DateTime.Now;
                db.MaterialDictionaries.Add(_form);
            }
            else
            {
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
            parameters.TryGetValue<MaterialDictionary>("data", out var data);

            parameters.TryGetValue<MaterialType>("type", out var type);

            if (data != null)
            {
                _form = data.DeepClone();
                // 重要,把传入的 Detach, 把副本 Attach, 否则后续更改会报错
                db.Entry(data).State = EntityState.Detached;
                db.MaterialDictionaries.Attach(_form);

                Name = _form.Name;
                SortNo = _form.SortNo.ToString();
            }
            else
            {
                _form.Type = type;
            }

            DialogTitle = data == null ? $"新增{_form.Type}" : $"编辑 {data.Name}";
        }

        private DbService db;
        private AJGlobalExceptionResolvedEvent _globalExceptionEvt;

        public MaterialDictionaryFormViewModel(DbService dbIns, IEventAggregator eventAggregator)
        {
            _form = new MaterialDictionary();
            _globalExceptionEvt = eventAggregator.GetEvent<AJGlobalExceptionResolvedEvent>();
            _globalExceptionEvt.Subscribe(() =>
            {
                Loading = false;
            });
            db = dbIns;
        }
    }
}
