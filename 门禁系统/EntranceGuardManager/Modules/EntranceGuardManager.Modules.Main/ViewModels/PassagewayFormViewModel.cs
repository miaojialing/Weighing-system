using AJWPFAdmin.Core;
using AJWPFAdmin.Core.Components.AJTable.ViewModels;
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
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static AJWPFAdmin.Core.ExceptionTool;

namespace EntranceGuardManager.Modules.Main.ViewModels
{
    public class PassagewayFormViewModel : AnnotationValidationViewModel, IDialogAware
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

        private Passageway _form;


        private AJTableFormSchemaItemOptions _watchhouseId;
        [Display(Name = "所属岗亭")]
        [Required(ErrorMessage = "{0}必填")]
        public AJTableFormSchemaItemOptions WatchhouseId
        {
            get { return _watchhouseId; }
            set
            {
                SetProperty(ref _watchhouseId, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private AJTableFormSchemaItemOptions _warehouseId;
        public AJTableFormSchemaItemOptions WarehouseId
        {
            get { return _warehouseId; }
            set
            {
                SetProperty(ref _warehouseId, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

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

        private string _code;
        [Display(Name = "编号")]
        [Required(ErrorMessage = "{0}必填")]
        [StringLength(50, ErrorMessage = "{0}超长:{1}")]
        public string Code
        {
            get { return _code; }
            set
            {
                SetProperty(ref _code, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private PassagewayDirection _direction;
        public PassagewayDirection Direction
        {
            get { return _direction; }
            set
            {
                SetProperty(ref _direction, value);
            }
        }

        private bool _countCarEnable;
        public bool CountCarEnable
        {
            get { return _countCarEnable; }
            set
            {
                SetProperty(ref _countCarEnable, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private string _deviceGateway;
        public string DeviceGateway
        {
            get { return _deviceGateway; }
            set
            {
                SetProperty(ref _deviceGateway, value);
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

        private ObservableCollection<AJTableFormSchemaItemOptions> _watchHouseOptions;
        public ObservableCollection<AJTableFormSchemaItemOptions> WatchHouseOptions
        {
            get { return _watchHouseOptions; }
            set
            {
                SetProperty(ref _watchHouseOptions, value);
            }
        }

        private ObservableCollection<AJTableFormSchemaItemOptions> _warehouseOptions;
        public ObservableCollection<AJTableFormSchemaItemOptions> WarehouseOptions
        {
            get { return _warehouseOptions; }
            set
            {
                SetProperty(ref _warehouseOptions, value);
            }
        }

        private Dictionary<string, PassagewayDirection> _directionOptions;
        public Dictionary<string, PassagewayDirection> DirectionOptions
        {
            get { return _directionOptions; }
            set
            {
                SetProperty(ref _directionOptions, value);
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

            if (CountCarEnable && WarehouseId == null)
            {
                ExceptionTool.FriendlyError("关联仓库必选", dialogId: DialogIds.DialogWindow);
                return;
            }

            Loading = true;

            _form.Name = Name;
            var code =Code;
            _form.Remark = Remark;
            var wid = (long)WatchhouseId.Value;
            _form.DeviceGateway = DeviceGateway;
            _form.CountCarEnable = CountCarEnable;
            _form.Direction = Direction;
            if (CountCarEnable)
            {
                _form.WarehouseId = (long)WarehouseId.Value;
                _form.WarehouseName = WarehouseId.Label;
            }
            else
            {
                _form.WarehouseId = null;
                _form.WarehouseName = string.Empty;
            }
            
            if (_form.Id == 0)
            {
                _form.Code = code;
                _form.WatchhouseId = wid;
                if (db.Passageways.Any(p => p.Code == code && p.WatchhouseId == wid))
                {
                    FriendlyError("通道编号已存在");
                }
                _form.Id = SnowFlake.GetInstance().GetLongId();
                _form.CreateDate = DateTime.Now;
                db.Passageways.Add(_form);
            }
            else
            {
                if ((_form.Code != code || _form.WatchhouseId != wid) 
                    && db.Passageways.Any(p => p.Code == code && p.WatchhouseId == wid))
                {
                    FriendlyError("通道编号已存在");
                }
                _form.Code = code;
                _form.WatchhouseId = wid;
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
            parameters.TryGetValue<Passageway>("data", out var data);

            parameters.TryGetValue<ObservableCollection<AJTableFormSchemaItemOptions>>("watchhouseOptions", out var watchhouseOptions);

            DialogTitle = data == null ? $"新增通道" : $"编辑 {data.Name}";

            WatchHouseOptions = watchhouseOptions;

            var warehouseList = await db.Warehouses.AsNoTracking()
                .OrderByDescending(p => p.Id).Select(p => new AJTableFormSchemaItemOptions
                {
                    Label = p.Name,
                    Value = p.Id
                }).ToListAsync();

            WarehouseOptions = new ObservableCollection<AJTableFormSchemaItemOptions>(warehouseList);

            if (data != null)
            {
                _form = data;

                Name = _form.Name;
                Code = _form.Code;
                Remark = _form.Remark;
                WatchhouseId = WatchHouseOptions.FirstOrDefault(p => (long)p.Value == _form.WatchhouseId);
                DeviceGateway = _form.DeviceGateway;
                CountCarEnable = _form.CountCarEnable;
                Direction = _form.Direction;
                if (_form.WarehouseId.HasValue)
                {
                    WarehouseId = WarehouseOptions
                        .FirstOrDefault(p => (long)p.Value == _form.WarehouseId.GetValueOrDefault());
                }
            }
            else
            {
                WatchhouseId = WatchHouseOptions.FirstOrDefault();
                Direction = PassagewayDirection.进;
                Code = new Random().StrictNext().ToString();
            }

            
        }

        private DbService db;
        private AJGlobalExceptionResolvedEvent _globalExceptionEvt;

        public PassagewayFormViewModel(DbService dbIns, IEventAggregator eventAggregator)
        {
            _form = new Passageway();
            _globalExceptionEvt = eventAggregator.GetEvent<AJGlobalExceptionResolvedEvent>();
            _globalExceptionEvt.Subscribe(() =>
            {
                Loading = false;
            });
            db = dbIns;
            DirectionOptions = CommonUtil.EnumToDictionary<PassagewayDirection>(null);
        }
    }
}
