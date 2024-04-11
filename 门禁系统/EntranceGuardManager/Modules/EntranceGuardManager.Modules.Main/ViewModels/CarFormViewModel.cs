using AJWPFAdmin.Core;
using AJWPFAdmin.Core.Components.AJFilePicker.ViewModels;
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
using Newtonsoft.Json;
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
using System.Windows.Data;
using static AJWPFAdmin.Core.ExceptionTool;

namespace EntranceGuardManager.Modules.Main.ViewModels
{
    public class CarTypeVisibilityConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }

            var type = (AJTableFormSchemaItemOptions)value;

            return type.Label == "台账车" ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class CarFormViewModel : AnnotationValidationViewModel, IDialogAware
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

        private Car _form;


        private AJTableFormSchemaItemOptions _typeId;
        [Display(Name = "类型")]
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
        [Display(Name = "卡号")]
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

        private DateTime? _expireDate;
        public DateTime? ExpireDate
        {
            get { return _expireDate; }
            set
            {
                SetProperty(ref _expireDate, value);
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

        private ObservableCollection<UploadFileItem> _vehicleLicense;
        public ObservableCollection<UploadFileItem> VehicleLicense
        {
            get { return _vehicleLicense; }
            set
            {
                SetProperty(ref _vehicleLicense, value);
            }
        }

        private ObservableCollection<UploadFileItem> _attachments;
        public ObservableCollection<UploadFileItem> Attachments
        {
            get { return _attachments; }
            set
            {
                SetProperty(ref _attachments, value);
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
            var idCardNo = IDCardNo;

            if (!CommonRegex.CARNO.IsMatch(carNo))
            {
                await CommonUtil.ShowAlertDialogAsync(new MaterialDesignExtensions.Controls.AlertDialogArguments
                {
                    Title = "提示",
                    Message = "输入的车牌号格式有误"
                });
                return;
            }
            var tid = (long)TypeId.Value;
            _form.ExpireDate = ExpireDate;
            _form.TypeName = TypeId.Label;
            _form.EngineNo = EngineNo;
            _form.VIN = VIN;
            _form.PaiFangJieDuan = PaiFangJieDuan;
            _form.RegDate = RegDate;
            _form.MaterialName = MaterialName;
            _form.CarNetWeight = CarNetWeight.TryGetDecimal();
            _form.VehicleLicense = CommonUtil.AJSerializeObject(VehicleLicense.Select(p => p.Url).ToArray());
            _form.Attachments = CommonUtil.AJSerializeObject(Attachments.Select(p => p.Url).ToArray());

            if (_form.Id == 0)
            {
                _form.CarNo = carNo;
                _form.TypeId = tid;
                _form.IDCardNo = idCardNo;
                if (db.Cars.Any(p => p.CarNo == carNo))
                {
                    FriendlyError("车牌号已存在");
                }
                if (!string.IsNullOrWhiteSpace(idCardNo))
                {
                    if (db.Cars.Any(p => p.IDCardNo == idCardNo))
                    {
                        FriendlyError("卡号已存在");
                    }
                }
                _form.Id = SnowFlake.GetInstance().GetLongId();
                _form.CreateDate = DateTime.Now;
                db.Cars.Add(_form);
            }
            else
            {
                if (_form.CarNo != carNo && db.Cars.Any(p => p.CarNo == carNo))
                {
                    FriendlyError("车牌号已存在");
                }

                if (_form.IDCardNo != idCardNo && db.Cars.Any(p => p.IDCardNo == idCardNo))
                {
                    FriendlyError("卡号已存在");
                }

                _form.CarNo = carNo;
                _form.IDCardNo = idCardNo;
                _form.TypeId = tid;
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
            parameters.TryGetValue<Car>("data", out var data);

            parameters.TryGetValue<ObservableCollection<AJTableFormSchemaItemOptions>>("typeOptions", out var typeOptions);

            TypeOptions = typeOptions;

            if (data != null)
            {
                _form = data;

                var prefix = _form.CarNo.ElementAtOrDefault(0).ToString();

                CarPrefix = CarPreFixOptions.Contains(prefix) ? prefix : CarPreFixOptions.FirstOrDefault();
                CarNo = string.Join("", _form.CarNo.Skip(1).ToArray());
                IDCardNo = _form.IDCardNo;
                ExpireDate = _form.ExpireDate;
                TypeId = TypeOptions.FirstOrDefault(p => (long)p.Value == _form.TypeId);

                EngineNo = _form.EngineNo;
                VIN = _form.VIN;
                PaiFangJieDuan = _form.PaiFangJieDuan;
                RegDate = _form.RegDate;
                MaterialName = _form.MaterialName;
                CarNetWeight = _form.CarNetWeight.ToString();

                var vehicleImages = CommonUtil.TryGetJSONObject<string[]>(_form.VehicleLicense) ?? Array.Empty<string>();
                VehicleLicense = new ObservableCollection<UploadFileItem>(vehicleImages.Select(p => new UploadFileItem(p)));

                var attachmentsImages = CommonUtil.TryGetJSONObject<string[]>(_form.Attachments) ?? Array.Empty<string>();
                Attachments = new ObservableCollection<UploadFileItem>(attachmentsImages.Select(p => new UploadFileItem(p)));
            }
            else
            {
                CarPrefix = CarPreFixOptions.FirstOrDefault();
                TypeId = TypeOptions.FirstOrDefault();
            }

            DialogTitle = data == null ? $"新增车辆" : $"编辑 {data.CarNo}";
        }

        private DbService db;
        private AJGlobalExceptionResolvedEvent _globalExceptionEvt;

        public CarFormViewModel(DbService dbIns, IEventAggregator eventAggregator)
        {
            _form = new Car();
            VehicleLicense = new ObservableCollection<UploadFileItem>();
            Attachments = new ObservableCollection<UploadFileItem>();
            _globalExceptionEvt = eventAggregator.GetEvent<AJGlobalExceptionResolvedEvent>();
            _globalExceptionEvt.Subscribe(() =>
            {
                Loading = false;
            });
            db = dbIns;

            CarPreFixOptions = CommonUtil.CARNOPREFIX;
        }
    }
}
