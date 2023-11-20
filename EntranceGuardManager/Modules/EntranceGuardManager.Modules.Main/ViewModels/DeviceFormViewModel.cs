using AJWPFAdmin.Core;
using AJWPFAdmin.Core.Components.AJTable.ViewModels;
using AJWPFAdmin.Core.Enums;
using AJWPFAdmin.Core.ExtensionMethods;
using AJWPFAdmin.Core.Utils;
using AJWPFAdmin.Core.Validation;
using AJWPFAdmin.Services.EF;
using AJWPFAdmin.Services.EF.Tables;
using Masuit.Tools;
using Masuit.Tools.Core.Validator;
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
    public class DeviceFormViewModel : AnnotationValidationViewModel, IDialogAware
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

        private Device _form;


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

        private AJTableFormSchemaItemOptions _passagewayId;
        [Display(Name = "所属通道")]
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

        private string _code;
        [Display(Name = "编号")]
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

        private string _ip;
        [Display(Name = "IP地址")]
        [StringLength(50, ErrorMessage = "{0}超长:{1}")]
        public string IP
        {
            get { return _ip; }
            set
            {
                SetProperty(ref _ip, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private DeviceType _type;
        public DeviceType Type
        {
            get { return _type; }
            set
            {
                SetProperty(ref _type, value);
            }
        }

        private SerialPortType _serialPort;
        public SerialPortType SerialPort
        {
            get { return _serialPort; }
            set
            {
                SetProperty(ref _serialPort, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private string _port;
        [Display(Name = "端口号")]
        [AJFormIntegerField(ErrorMessage = "端口号有误")]
        public string Port
        {
            get { return _port; }
            set
            {
                SetProperty(ref _port, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private bool _onlyMonitor;
        public bool OnlyMonitor
        {
            get { return _onlyMonitor; }
            set
            {
                SetProperty(ref _onlyMonitor, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private string _loginName;
        [Display(Name = "登录账号")]
        [StringLength(50, ErrorMessage = "{0}超长:{1}")]
        public string LoginName
        {
            get { return _loginName; }
            set
            {
                SetProperty(ref _loginName, value);
            }
        }

        private string _loginPassword;
        [Display(Name = "登录密码")]
        [StringLength(100, ErrorMessage = "{0}超长:{1}")]
        public string LoginPassword
        {
            get { return _loginPassword; }
            set
            {
                SetProperty(ref _loginPassword, value);
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

        private ObservableCollection<AJTableFormSchemaItemOptions> _passagewayOptions;
        public ObservableCollection<AJTableFormSchemaItemOptions> PassagewayOptions
        {
            get { return _passagewayOptions; }
            set
            {
                SetProperty(ref _passagewayOptions, value);
            }
        }

        private ObservableCollection<AJTableFormSchemaItemOptions> _passagewaySource;

        private Dictionary<string, DeviceType> _typeOptions;
        public Dictionary<string, DeviceType> TypeOptions
        {
            get { return _typeOptions; }
            set
            {
                SetProperty(ref _typeOptions, value);
            }
        }

        private Dictionary<string, SerialPortType> _serialPortOptions;
        public Dictionary<string, SerialPortType> SerialPortOptions
        {
            get { return _serialPortOptions; }
            set
            {
                SetProperty(ref _serialPortOptions, value);
            }
        }

        private DelegateCommand<AJTableFormSchemaItemOptions> _watchhouseChangedCmd;
        public DelegateCommand<AJTableFormSchemaItemOptions> WatchhouseChangedCmd =>
            _watchhouseChangedCmd ?? (_watchhouseChangedCmd = new DelegateCommand<AJTableFormSchemaItemOptions>(ExecuteWatchhouseChangedCmd));

        void ExecuteWatchhouseChangedCmd(AJTableFormSchemaItemOptions parameter)
        {
            PassagewayOptions = new ObservableCollection<AJTableFormSchemaItemOptions>(_passagewaySource.Where(p => (long)p.OtherData == (long)parameter.Value));

            PassagewayId = PassagewayOptions.FirstOrDefault();
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

            var code = Code;
            var type = Type;
            _form.IP = IP;
            var wid = (long)WatchhouseId.Value;
            var psgId = (long)PassagewayId.Value;
            _form.LoginName = LoginName;
            _form.LoginPassword = LoginPassword;
            _form.OnlyMonitor = OnlyMonitor;
            _form.Port = Port.TryGetInt();
            _form.SerialPort = SerialPort;

            if (_form.Id == 0)
            {
                _form.Type = type;
                _form.Code = code;
                _form.WatchhouseId = wid;
                _form.PassagewayId = psgId;
                if (db.Devices.Any(p => p.Code == code && p.Type == type
                && p.PassagewayId == psgId && p.WatchhouseId == wid))
                {
                    FriendlyError("编号已存在");
                }
                _form.Id = SnowFlake.GetInstance().GetLongId();
                _form.CreateDate = DateTime.Now;
                db.Devices.Add(_form);
            }
            else
            {
                if ((_form.Code != code || _form.Type != type 
                    || _form.WatchhouseId != wid || _form.PassagewayId != psgId)
                    && db.Devices.Any(p => p.Code == code && p.Type == type
                    && p.PassagewayId == psgId && p.WatchhouseId == wid))
                {
                    FriendlyError("编号已存在");
                }
                _form.Code = code;
                _form.Type = type;
                _form.WatchhouseId = wid;
                _form.PassagewayId = psgId;
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
            parameters.TryGetValue<Device>("data", out var data);

            parameters.TryGetValue<ObservableCollection<AJTableFormSchemaItemOptions>>("watchhouseOptions", out var watchhouseOptions);

            parameters.TryGetValue("passagewayOptions", out _passagewaySource);

            _passagewaySource ??= new ObservableCollection<AJTableFormSchemaItemOptions>();

            WatchHouseOptions = watchhouseOptions;

            if (data != null)
            {
                _form = data;

                Code = _form.Code;
                IP = _form.IP;
                LoginName = _form.LoginName;
                LoginPassword = _form.LoginPassword;
                OnlyMonitor = _form.OnlyMonitor;
                Port = _form.Port.ToString();
                SerialPort = _form.SerialPort;
                Type = _form.Type;
                WatchhouseId = WatchHouseOptions.FirstOrDefault(p => (long)p.Value == _form.WatchhouseId);
                PassagewayId = _passagewaySource.FirstOrDefault(p => (long)p.Value == _form.PassagewayId);
            }
            else
            {
                WatchhouseId = WatchHouseOptions.FirstOrDefault();
                PassagewayId = _passagewaySource.FirstOrDefault(p => (long)p.OtherData == (long)WatchhouseId.Value);
                Type = DeviceType.车牌识别相机_臻识;
                Code = new Random().StrictNext().ToString();
            }

            ExecuteWatchhouseChangedCmd(WatchhouseId);

            DialogTitle = data == null ? $"新增设备" : $"编辑 {data.Code}-{data.Type}";
        }

        private DbService db;
        private AJGlobalExceptionResolvedEvent _globalExceptionEvt;

        public DeviceFormViewModel(DbService dbIns, IEventAggregator eventAggregator)
        {
            _form = new Device();
            _globalExceptionEvt = eventAggregator.GetEvent<AJGlobalExceptionResolvedEvent>();
            _globalExceptionEvt.Subscribe(() =>
            {
                Loading = false;
            });
            db = dbIns;
            TypeOptions = CommonUtil.EnumToDictionary<DeviceType>(null);
            SerialPortOptions = CommonUtil.EnumToDictionary<SerialPortType>(null);
        }
    }
}
