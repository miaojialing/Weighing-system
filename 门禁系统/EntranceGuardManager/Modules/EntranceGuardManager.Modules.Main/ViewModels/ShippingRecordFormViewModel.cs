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
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static AJWPFAdmin.Core.ExceptionTool;

namespace EntranceGuardManager.Modules.Main.ViewModels
{
    public class ShippingRecordFormViewModel : AnnotationValidationViewModel, IDialogAware
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

        private bool _preparing;
        public bool Preparing
        {
            get { return _preparing; }
            set { SetProperty(ref _preparing, value); }
        }

        private ShippingRecord _form;


        private string _carNo;
        [Display(Name = "车牌号")]
        [Required(ErrorMessage = "{0}必填")]
        [StringLength(50, ErrorMessage = "{0}超长:{1}")]
        [AJFormCodeField]
        public string CarNo
        {
            get { return _carNo; }
            set
            {
                SetProperty(ref _carNo, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private string _idCardNo;
        [Display(Name = "车牌号")]
        [StringLength(120, ErrorMessage = "{0}超长:{1}")]
        [AJFormCodeField]
        public string IDCardNo
        {
            get { return _idCardNo; }
            set
            {
                SetProperty(ref _idCardNo, value);
            }
        }

        private PassagewayDirection _direction;
        [Display(Name = "状态")]
        [Required(ErrorMessage = "{0}必选")]
        public PassagewayDirection Direction
        {
            get { return _direction; }
            set
            {
                SetProperty(ref _direction, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private string _carPrefix;
        [Display(Name = "车牌前缀")]
        [Required(ErrorMessage = "{0}必填")]
        public string CarPrefix
        {
            get { return _carPrefix; }
            set
            {
                SetProperty(ref _carPrefix, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private AJTableFormSchemaItemOptions _typeId;
        [Display(Name = "车辆类型")]
        [Required(ErrorMessage = "{0}必填")]
        public AJTableFormSchemaItemOptions TypeId
        {
            get { return _typeId; }
            set
            {
                SetProperty(ref _typeId, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private AJTableFormSchemaItemOptions _watchhouseId;
        [Display(Name = "进厂岗亭")]
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

        private AJTableFormSchemaItemOptions _passagewayId;
        [Display(Name = "进厂通道")]
        [Required(ErrorMessage = "{0}必填")]
        public AJTableFormSchemaItemOptions PassagewayId
        {
            get { return _passagewayId; }
            set
            {
                SetProperty(ref _passagewayId, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private AJTableFormSchemaItemOptions _exitWatchhouseId;
        public AJTableFormSchemaItemOptions ExitWatchhouseId
        {
            get { return _exitWatchhouseId; }
            set
            {
                SetProperty(ref _exitWatchhouseId, value);
            }
        }

        private AJTableFormSchemaItemOptions _exitPassagewayId;
        public AJTableFormSchemaItemOptions ExitPassagewayId
        {
            get { return _exitPassagewayId; }
            set
            {
                SetProperty(ref _exitPassagewayId, value);
            }
        }


        private string _engineNo;
        [Display(Name = "发动机号")]
        [StringLength(50, ErrorMessage = "{0}超长")]
        public string EngineNo
        {
            get { return _engineNo; }
            set
            {
                SetProperty(ref _engineNo, value);
            }
        }

        private string _vin;
        [Display(Name = "VIN")]
        [StringLength(50, ErrorMessage = "{0}超长")]
        public string VIN
        {
            get { return _vin; }
            set
            {
                SetProperty(ref _vin, value);
            }
        }

        private string _paiFangJieDuan;
        [Display(Name = "排放阶段")]
        [StringLength(50, ErrorMessage = "{0}超长")]
        public string PaiFangJieDuan
        {
            get { return _paiFangJieDuan; }
            set
            {
                SetProperty(ref _paiFangJieDuan, value);
            }
        }

        private DateTime? _regDate;
        public DateTime? RegDate
        {
            get { return _regDate; }
            set
            {
                SetProperty(ref _regDate, value);
            }
        }

        private string _materialName;
        [Display(Name = "货物")]
        [StringLength(50, ErrorMessage = "{0}超长:{1}")]
        public string MaterialName
        {
            get { return _materialName; }
            set { SetProperty(ref _materialName, value); }
        }

        private string _carNetWeight;
        [Display(Name = "重量")]
        [AJFormNumberField]
        public string CarNetWeight
        {
            get { return _carNetWeight; }
            set { SetProperty(ref _carNetWeight, value); }
        }

        private DateTime? _shipStartDate;
        public DateTime? ShipStartDate
        {
            get { return _shipStartDate; }
            set { SetProperty(ref _shipStartDate, value); }
        }

        private DateTime? _shipEndDate;
        public DateTime? ShipEndDate
        {
            get { return _shipEndDate; }
            set { SetProperty(ref _shipEndDate, value); }
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

        private List<Passageway> _passagewaySource;

        private ObservableCollection<AJTableFormSchemaItemOptions> _passagewayOptions;
        public ObservableCollection<AJTableFormSchemaItemOptions> PassagewayOptions
        {
            get { return _passagewayOptions; }
            set
            {
                SetProperty(ref _passagewayOptions, value);
            }
        }

        private ObservableCollection<AJTableFormSchemaItemOptions> _exitPassagewayOptions;
        public ObservableCollection<AJTableFormSchemaItemOptions> ExitPassagewayOptions
        {
            get { return _exitPassagewayOptions; }
            set
            {
                SetProperty(ref _exitPassagewayOptions, value);
            }
        }

        private ObservableCollection<AJTableFormSchemaItemOptions> _typeOptions;
        public ObservableCollection<AJTableFormSchemaItemOptions> TypeOptions
        {
            get { return _typeOptions; }
            set
            {
                SetProperty(ref _typeOptions, value);
            }
        }

        private ObservableCollection<string> _carPreFixOptions;
        public ObservableCollection<string> CarPreFixOptions
        {
            get { return _carPreFixOptions; }
            set
            {
                SetProperty(ref _carPreFixOptions, value);
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



        private DelegateCommand<object> _watchhouseChangedCmd;
        public DelegateCommand<object> WatchhouseChangedCmd =>
            _watchhouseChangedCmd ?? (_watchhouseChangedCmd = new DelegateCommand<object>(ExecuteWatchhouseChangedCmd));

        void ExecuteWatchhouseChangedCmd(object parameter)
        {
            AJTableFormSchemaItemOptions option = null;
            if (parameter is SelectionChangedEventArgs eventArgs)
            {
                if (eventArgs.AddedItems != null && eventArgs.AddedItems.Count > 0)
                {
                    option = (AJTableFormSchemaItemOptions)eventArgs.AddedItems[0];
                }
            }
            else
            {
                option = (AJTableFormSchemaItemOptions)parameter;
            }

            if (option == null)
            {
                PassagewayOptions?.Clear();
                PassagewayId = null;
                return;
            }

            PassagewayOptions = new ObservableCollection<AJTableFormSchemaItemOptions>(_passagewaySource
               .Where(p =>p.Direction  ==  PassagewayDirection.进 && p.WatchhouseId == (long)option.Value)
               .Select(p => new AJTableFormSchemaItemOptions
               {
                   Label = p.Name,
                   Value = p.Id,
                   OtherData = p.Direction
               }));

            PassagewayId = PassagewayOptions.FirstOrDefault();
        }

        private DelegateCommand<object> _exitEatchhouseChangedCmd;
        public DelegateCommand<object> ExitWatchhouseChangedCmd =>
            _exitEatchhouseChangedCmd ?? (_exitEatchhouseChangedCmd = new DelegateCommand<object>(ExecuteExitWatchhouseChangedCmd));

        void ExecuteExitWatchhouseChangedCmd(object parameter)
        {
            AJTableFormSchemaItemOptions option = null;
            if (parameter is SelectionChangedEventArgs eventArgs)
            {
                if (eventArgs.AddedItems != null && eventArgs.AddedItems.Count > 0)
                {
                    option = (AJTableFormSchemaItemOptions)eventArgs.AddedItems[0];
                }
            }
            else
            {
                option = (AJTableFormSchemaItemOptions)parameter;
            }

            if (option == null)
            {
                ExitPassagewayOptions?.Clear();
                ExitPassagewayId = null;
                return;
            }

            ExitPassagewayOptions = new ObservableCollection<AJTableFormSchemaItemOptions>(_passagewaySource
               .Where(p => p.Direction == PassagewayDirection.出 && p.WatchhouseId == (long)option.Value)
               .Select(p => new AJTableFormSchemaItemOptions
               {
                   Label = p.Name,
                   Value = p.Id,
                   OtherData = p.Direction
               }));

            ExitPassagewayId = ExitPassagewayOptions.FirstOrDefault();
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
            
            var carNo = $"{CarPrefix}{CarNo}";
            if (!CommonRegex.CARNO.IsMatch(carNo))
            {
                FriendlyError("输入的车牌号格式有误");
            }

            _form.CarNo = carNo;
            _form.IDCardNo = IDCardNo;
            _form.WatchhouseId = (long)WatchhouseId.Value;
            _form.WatchhouseName = WatchhouseId.Label;
            _form.PassagewayId = (long)PassagewayId.Value;
            _form.PassagewayName = PassagewayId.Label;

            if (ExitWatchhouseId != null)
            {
                _form.ExitWatchhouseId = (long)ExitWatchhouseId.Value;
                _form.ExitWatchhouseName = ExitWatchhouseId.Label;
            }

            if (ExitPassagewayId != null)
            {
                _form.ExitPassagewayId = (long)ExitPassagewayId.Value;
                _form.ExitPassagewayName = ExitPassagewayId.Label;
            }
            
            _form.Direction = (PassagewayDirection)PassagewayId.OtherData;

            if (_form.Direction == PassagewayDirection.出)
            {
                if (_form.ExitPassagewayId == null)
                {
                    FriendlyError("设置了出厂状态必须设置出厂通道 ");
                    return;
                }
                if (ShipEndDate ==  null)
                {
                    FriendlyError("设置了出厂状态必须设置出厂日期");
                    return;
                }
            }

            _form.TypeId = (long)TypeId.Value;
            _form.TypeName = TypeId.Label;
            _form.EngineNo = EngineNo;
            _form.VIN = VIN;
            _form.PaiFangJieDuan = PaiFangJieDuan;
            _form.RegDate = RegDate;
            _form.MaterialName = MaterialName;
            _form.CarNetWeight = CarNetWeight.TryGetDecimal();
            _form.ShipStartDate = ShipStartDate;
            _form.ShipEndDate = ShipEndDate;
            _form.Direction = Direction;

            if (_form.Id == 0)
            {
                _form.Id = SnowFlake.GetInstance().GetLongId();
                _form.MaterialCategory = string.Empty;
                _form.CreateDate = DateTime.Now;
                db.ShippingRecords.Add(_form);
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

        public async void OnDialogOpened(IDialogParameters parameters)
        {
            parameters.TryGetValue<ShippingRecord>("data", out var data);

            DialogTitle = data == null ? $"新增台账记录" : $"编辑 {data.CarNo}";

            Preparing = true;

            var watchhouseOptions = await db.Watchhouses.AsNoTracking().OrderBy(p => p.Id)
                .Select(p => new AJTableFormSchemaItemOptions
                {
                    Value = p.Id,
                    Label = p.Name
                }).ToListAsync();

            WatchHouseOptions = new ObservableCollection<AJTableFormSchemaItemOptions>(watchhouseOptions);

            _passagewaySource = await db.Passageways.AsNoTracking().OrderBy(p => p.Id).ToListAsync();

            var typeOptions = await db.CarTypes.AsNoTracking().OrderBy(p => p.Id)
                .Select(p => new AJTableFormSchemaItemOptions
                {
                    Value = p.Id,
                    Label = p.Name,
                }).ToListAsync();

            TypeOptions = new ObservableCollection<AJTableFormSchemaItemOptions>(typeOptions);

            if (data != null)
            {
                _form = data;

                var prefix = _form.CarNo.ElementAtOrDefault(0).ToString();

                CarPrefix = CarPreFixOptions.Contains(prefix) ? prefix : CarPreFixOptions.FirstOrDefault();
                CarNo = string.Join("", _form.CarNo.Skip(1).ToArray());
                IDCardNo = _form.IDCardNo;
                Direction = _form.Direction;
                WatchhouseId = WatchHouseOptions.FirstOrDefault(p => (long)p.Value == _form.WatchhouseId);
                ExitWatchhouseId = WatchHouseOptions.FirstOrDefault(p => (long)p.Value == _form.ExitWatchhouseId.GetValueOrDefault());

                var selectedPassageway = _passagewaySource
                    .FirstOrDefault(p =>p.Direction == PassagewayDirection.进 && p.Id == _form.PassagewayId);

                if (selectedPassageway != null)
                {
                    PassagewayId = new AJTableFormSchemaItemOptions
                    {
                        Label = selectedPassageway.Name,
                        Value = selectedPassageway.Id,
                        OtherData = selectedPassageway.Direction
                    };
                }

                var selectedExitPassageway = _passagewaySource
                    .FirstOrDefault(p => p.Direction == PassagewayDirection.出 && p.Id == _form.ExitPassagewayId.GetValueOrDefault());

                if(selectedExitPassageway != null)
                {
                    ExitPassagewayId = new AJTableFormSchemaItemOptions
                    {
                        Label = selectedExitPassageway.Name,
                        Value = selectedExitPassageway.Id,
                        OtherData = selectedExitPassageway.Direction
                    };
                }
                
                TypeId = TypeOptions.FirstOrDefault(p => (long)p.Value == _form.TypeId);
                EngineNo = _form.EngineNo;
                VIN = _form.VIN;
                PaiFangJieDuan = _form.PaiFangJieDuan;
                RegDate = _form.RegDate;
                MaterialName = _form.MaterialName;
                CarNetWeight = _form.CarNetWeight.ToString();
                ShipStartDate = _form.ShipStartDate;
                ShipEndDate = _form.ShipEndDate;
            }
            else
            {
                CarPrefix = CarPreFixOptions.FirstOrDefault();
                WatchhouseId = WatchHouseOptions.FirstOrDefault();
                ExitWatchhouseId = WatchHouseOptions.FirstOrDefault();
                Direction = PassagewayDirection.进;

                var selectedPassageway = _passagewaySource
                    .FirstOrDefault(p => p.Direction == PassagewayDirection.进 
                    && p.WatchhouseId == (long)WatchhouseId.Value);
                if (selectedPassageway != null)
                {
                    PassagewayId = new AJTableFormSchemaItemOptions
                    {
                        Label = selectedPassageway.Name,
                        Value = selectedPassageway.Id,
                        OtherData = selectedPassageway.Direction
                    };
                }

                var selectedExitPassageway = _passagewaySource
                    .FirstOrDefault(p => p.Direction == PassagewayDirection.出 
                    && p.WatchhouseId == (long)ExitWatchhouseId.Value);
                if (selectedExitPassageway != null)
                {
                    ExitPassagewayId = new AJTableFormSchemaItemOptions
                    {
                        Label = selectedExitPassageway.Name,
                        Value = selectedExitPassageway.Id,
                        OtherData = selectedExitPassageway.Direction
                    };
                }

                TypeId = TypeOptions.FirstOrDefault();
                ShipStartDate = DateTime.Now;
            }

            ExecuteWatchhouseChangedCmd(WatchhouseId);
            ExecuteExitWatchhouseChangedCmd(WatchhouseId);

            Preparing = false;
        }

        private DbService db;
        private AJGlobalExceptionResolvedEvent _globalExceptionEvt;

        public ShippingRecordFormViewModel(DbService dbIns, IEventAggregator eventAggregator)
        {
            _form = new ShippingRecord();
            _globalExceptionEvt = eventAggregator.GetEvent<AJGlobalExceptionResolvedEvent>();
            _globalExceptionEvt.Subscribe(() =>
            {
                Loading = false;
                Preparing = false;
            });
            db = dbIns;
            CarPreFixOptions = CommonUtil.CARNOPREFIX;
            DirectionOptions = CommonUtil.EnumToDictionary<PassagewayDirection>((type) =>
            {
                switch (type)
                {
                    case PassagewayDirection.进:
                        return "进厂";
                    case PassagewayDirection.出:
                        return "出厂";
                    default:
                        return string.Empty;
                }
            });
        }
    }

    public class DirectionToExitPassagewayVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (PassagewayDirection)value  == PassagewayDirection.进 
                ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
