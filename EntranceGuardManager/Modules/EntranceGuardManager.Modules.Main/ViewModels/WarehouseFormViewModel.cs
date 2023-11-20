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
using System.Windows.Media;
using static AJWPFAdmin.Core.ExceptionTool;

namespace EntranceGuardManager.Modules.Main.ViewModels
{
    public class WarehouseFormViewModel : AnnotationValidationViewModel, IDialogAware
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

        private bool _editMode;
        public bool EditMode
        {
            get { return _editMode; }
            set { SetProperty(ref _editMode, value); }
        }

        private Warehouse _form;

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

        private string _carLimit;
        [Display(Name = "总车次")]
        [Required(ErrorMessage = "{0}必填")]
        [AJFormNumberField]
        public string CarLimit
        {
            get { return _carLimit; }
            set
            {
                SetProperty(ref _carLimit, value);
                if (!EditMode)
                {
                    ResidualCarLimit = value;
                }
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private string _residualCarLimit;
        [Display(Name = "剩余车次")]
        [Required(ErrorMessage = "{0}必填")]
        [AJFormNumberField]
        public string ResidualCarLimit
        {
            get { return _residualCarLimit; }
            set
            {
                SetProperty(ref _residualCarLimit, value);
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
            _form.CarLimit = CarLimit.TryGetInt();
            _form.ResidualCarLimit = ResidualCarLimit.TryGetInt();

            if (_form.Id == 0)
            {
                if (db.Warehouses.Any(p => p.Name == name))
                {
                    ExceptionTool.FriendlyError("仓库名称已存在", dialogId: DialogIds.DialogWindow);
                    return;
                }
                _form.Name = Name;
                _form.Id = SnowFlake.GetInstance().GetLongId();
                _form.CreateDate = DateTime.Now;
                db.Warehouses.Add(_form);
            }
            else
            {
                if (_form.Name != name && db.Warehouses.Any(p => p.Name == name))
                {
                    ExceptionTool.FriendlyError("仓库名称已存在", dialogId: DialogIds.DialogWindow);
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
            parameters.TryGetValue<Warehouse>("data", out var data);
            DialogTitle = data == null ? $"新增仓库" : $"编辑 {data.Name}";

            if (data != null)
            {
                EditMode = true;

                _form = data;
                
                Name = _form.Name;
                CarLimit = _form.CarLimit.ToString();
                ResidualCarLimit = _form.ResidualCarLimit.ToString();
            }

        }

        private DbService db;
        private AJGlobalExceptionResolvedEvent _globalExceptionEvt;

        public WarehouseFormViewModel(DbService dbIns, IEventAggregator eventAggregator)
        {
            _form = new Warehouse();
            _globalExceptionEvt = eventAggregator.GetEvent<AJGlobalExceptionResolvedEvent>();
            _globalExceptionEvt.Subscribe(() =>
            {
                Loading = false;
            });
            db = dbIns;
        }
    }
}
