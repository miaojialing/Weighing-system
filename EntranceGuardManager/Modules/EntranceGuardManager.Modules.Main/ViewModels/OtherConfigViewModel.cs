using AJWPFAdmin.Core.Utils;
using AJWPFAdmin.Core;
using AJWPFAdmin.Core.Validation;
using AJWPFAdmin.Services.EF;
using MaterialDesignExtensions.Controls;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AJWPFAdmin.Core.ExceptionTool;
using Microsoft.EntityFrameworkCore;
using AJWPFAdmin.Services.EF.Tables;
using AJWPFAdmin.Core.Models;
using AJWPFAdmin.Core.Enums;
using DocumentFormat.OpenXml.Office2010.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;

namespace EntranceGuardManager.Modules.Main.ViewModels
{
    public class OtherConfigViewModel : AnnotationValidationViewModel, INavigationAware
    {

        private bool _loading;
        public bool Loading
        {
            get { return _loading; }
            set
            {
                SetProperty(ref _loading, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        #region 统计配置表单

        private PassagewayStatisticType _passagewayStatistic_Type;
        [Display(Name = "统计方式")]
        [Required(ErrorMessage = "{0}必填")]
        public PassagewayStatisticType PassagewayStatistic_Type
        {
            get { return _passagewayStatistic_Type; }
            set { SetProperty(ref _passagewayStatistic_Type, value); }
        }

        private Dictionary<string,PassagewayStatisticType> _passagewayStatisticOptions;
        public Dictionary<string,PassagewayStatisticType> PassagewayStatisticOptions
        {
            get { return _passagewayStatisticOptions; }
            set { SetProperty(ref _passagewayStatisticOptions, value); }
        }

        #endregion

        #region 接口配置表单

        private string _thirdpartyCarInfoAPI_Url;
        [Display(Name = "接口地址")]
        [Required(ErrorMessage = "{0}必填")]
        [Url(ErrorMessage = "{0}格式有无")]
        public string ThirdpartyCarInfoAPI_Url
        {
            get { return _thirdpartyCarInfoAPI_Url; }
            set { SetProperty(ref _thirdpartyCarInfoAPI_Url, value); }
        }

        private string _thirdpartyCarInfoAPI_CompanyName;
        [Display(Name = "公司名称")]
        [Required(ErrorMessage = "{0}必填")]
        [MaxLength(100, ErrorMessage = "{0}超长")]
        public string ThirdpartyCarInfoAPI_CompanyName
        {
            get { return _thirdpartyCarInfoAPI_CompanyName; }
            set { SetProperty(ref _thirdpartyCarInfoAPI_CompanyName, value); }
        }

        #endregion

        #region 识别配置表单

        private string _carIdentificationConfig_ImageSavePath;
        [Display(Name = "图片存放路径")]
        [Required(ErrorMessage = "{0}必填")]
        public string CarIdentificationConfig_ImageSavePath
        {
            get { return _carIdentificationConfig_ImageSavePath; }
            set { SetProperty(ref _carIdentificationConfig_ImageSavePath, value); }
        }

        #endregion

        #region LED配置表单

        private string _ledCfg_Entrance_Text1;
        [Display(Name = "LED入口第一行文字")]
        [Required(ErrorMessage = "{0}必填")]
        public string LEDCfg_Entrance_Text1
        {
            get { return _ledCfg_Entrance_Text1; }
            set { SetProperty(ref _ledCfg_Entrance_Text1, value); }
        }

        private string _ledCfg_Entrance_Text2;
        [Display(Name = "LED入口第二行文字")]
        [Required(ErrorMessage = "{0}必填")]
        public string LEDCfg_Entrance_Text2
        {
            get { return _ledCfg_Entrance_Text2; }
            set { SetProperty(ref _ledCfg_Entrance_Text2, value); }
        }

        private string _ledCfg_Entrance_Text3;
        [Display(Name = "LED入口第三行文字")]
        [Required(ErrorMessage = "{0}必填")]
        public string LEDCfg_Entrance_Text3
        {
            get { return _ledCfg_Entrance_Text3; }
            set { SetProperty(ref _ledCfg_Entrance_Text3, value); }
        }

        private string _ledCfg_Entrance_Text4;
        [Display(Name = "LED入口第四行文字")]
        [Required(ErrorMessage = "{0}必填")]
        public string LEDCfg_Entrance_Text4
        {
            get { return _ledCfg_Entrance_Text4; }
            set { SetProperty(ref _ledCfg_Entrance_Text4, value); }
        }

        private string _ledCfg_Exit_Text1;
        [Display(Name = "LED出口第一行文字")]
        [Required(ErrorMessage = "{0}必填")]
        public string LEDCfg_Exit_Text1
        {
            get { return _ledCfg_Exit_Text1; }
            set { SetProperty(ref _ledCfg_Exit_Text1, value); }
        }

        private string _ledCfg_Exit_Text2;
        [Display(Name = "LED出口第二行文字")]
        [Required(ErrorMessage = "{0}必填")]
        public string LEDCfg_Exit_Text2
        {
            get { return _ledCfg_Exit_Text2; }
            set { SetProperty(ref _ledCfg_Exit_Text2, value); }
        }

        private string _ledCfg_Exit_Text3;
        [Display(Name = "LED出口第三行文字")]
        [Required(ErrorMessage = "{0}必填")]
        public string LEDCfg_Exit_Text3
        {
            get { return _ledCfg_Exit_Text3; }
            set { SetProperty(ref _ledCfg_Exit_Text3, value); }
        }

        private string _ledCfg_Exit_Text4;
        [Display(Name = "LED出口第四行文字")]
        [Required(ErrorMessage = "{0}必填")]
        public string LEDCfg_Exit_Text4
        {
            get { return _ledCfg_Exit_Text4; }
            set { SetProperty(ref _ledCfg_Exit_Text4, value); }
        }

        #endregion

        private List<SystemConfigDictionary> _systemConfigDictionaries;

        private ThirdPartyCarInfoAPIConfig _thirdpartyCarInfoAPIConfig;
        private CarIdentificationConfig _carIdentificationConfig;
        private LEDConfig _ledConfig;
        private PassagewayStatisticConfig _passagewayStatisticConfig;

        private DbService db;

        public OtherConfigViewModel(DbService dbIns, IEventAggregator eventAggregator)
        {
            db = dbIns;
            eventAggregator.GetEvent<AJGlobalExceptionResolvedEvent>().Subscribe(() =>
            {
                Loading = false;
            });
        }

        private DelegateCommand _openFileDialogCmd;
        public DelegateCommand OpenFileDialogCmd =>
            _openFileDialogCmd ?? (_openFileDialogCmd = new DelegateCommand(ExecuteOpenFileDialogCmd));

        async void ExecuteOpenFileDialogCmd()
        {
            var ret = await OpenDirectoryDialog.ShowDialogAsync(DialogIds.Root, new OpenDirectoryDialogArguments
            {
                CreateNewDirectoryEnabled = true,
                CurrentDirectory = CarIdentificationConfig_ImageSavePath
            });
            if (ret.Confirmed)
            {
                CarIdentificationConfig_ImageSavePath = ret.Directory;
            }
        }

        private DelegateCommand _saveCmd;
        public DelegateCommand SaveCmd =>
            _saveCmd ?? (_saveCmd = new DelegateCommand(ExecuteSaveCmdAsync, CanExecuteSaveCmd));

        async void ExecuteSaveCmdAsync()
        {
            if (!ValidateModel())
            {
                await CommonUtil.ShowAlertDialogAsync(new AlertDialogArguments
                {
                    Title = "提示",
                    Message = "配置表单存在错误,请检查"
                });

                SaveCmd.RaiseCanExecuteChanged();
                return;
            }

            Loading = true;

            FillPassagewayStatisticConfigForm();
            FillThirdPartyCarInfoAPIConfigForm();
            FillCarIdentificationConfigForm();
            FillLEDConfigForm();
            

            db.SaveChanges();

            await CommonUtil.ShowAlertDialogAsync(new AlertDialogArguments
            {
                Title = "提示",
                Message = "保存成功!"
            });

            Loading = false;

        }

        bool CanExecuteSaveCmd()
        {
            return !HasErrors;
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            Loading = true;

            _systemConfigDictionaries = await db.SystemConfigDictionaries.ToListAsync();

            InitPassagewayStatisticConfigForm();
            InitThirdpartyCarInfoAPIConfigForm();
            InitCarIdentificationConfigForm();
            InitLEDConfigForm();

            Loading = false;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        /// <summary>
        /// 统计配置表单初始化
        /// </summary>
        private void InitPassagewayStatisticConfigForm()
        {
            _passagewayStatisticConfig = CommonUtil
                .TryGetJSONObject<PassagewayStatisticConfig>(_systemConfigDictionaries
                .FirstOrDefault(p => p.Key == SystemConfigKey.PassagewayStatisticConfig)
                ?.StringValue) ?? new PassagewayStatisticConfig();

            _passagewayStatistic_Type = _passagewayStatisticConfig.Type;

            PassagewayStatisticOptions = CommonUtil.EnumToDictionary<PassagewayStatisticType>((key) =>
            {
                switch (key)
                {
                    case PassagewayStatisticType.Once:
                        return "经过一次";
                    case PassagewayStatisticType.Twice:
                        return "一进一出";
                    default:
                        return "--";
                }
            });


        }

        /// <summary>
        /// 填充更改统计配置表单
        /// </summary>
        private void FillPassagewayStatisticConfigForm()
        {
            var cfg = _systemConfigDictionaries
                .FirstOrDefault(p => p.Key == SystemConfigKey.PassagewayStatisticConfig);

            _passagewayStatisticConfig.Type = PassagewayStatistic_Type;

            if (cfg == null)
            {
                db.SystemConfigDictionaries.Add(new SystemConfigDictionary
                {
                    Key = SystemConfigKey.PassagewayStatisticConfig,
                    StringValue = CommonUtil.AJSerializeObject(_passagewayStatisticConfig),
                    CreateDate = DateTime.Now
                });
            }
            else
            {
                cfg.StringValue
                    = CommonUtil.AJSerializeObject(_passagewayStatisticConfig);
                db.Entry(_passagewayStatisticConfig).State = EntityState.Modified;
            }
        }

        /// <summary>
        /// LED配置表单初始化
        /// </summary>
        private void InitLEDConfigForm()
        {
            _ledConfig = CommonUtil
                .TryGetJSONObject<LEDConfig>(_systemConfigDictionaries
                .FirstOrDefault(p => p.Key == SystemConfigKey.LEDConfig)
                ?.StringValue) ?? new LEDConfig();

            if (_ledConfig.EntranceTextArray == null)
            {
                _ledConfig.Init();
            }

            LEDCfg_Entrance_Text1 = _ledConfig.EntranceTextArray.ElementAtOrDefault(0);
            LEDCfg_Entrance_Text2 = _ledConfig.EntranceTextArray.ElementAtOrDefault(1);
            LEDCfg_Entrance_Text3 = _ledConfig.EntranceTextArray.ElementAtOrDefault(2);
            LEDCfg_Entrance_Text4 = _ledConfig.EntranceTextArray.ElementAtOrDefault(3);

            LEDCfg_Exit_Text1 = _ledConfig.ExitTextArray.ElementAtOrDefault(0);
            LEDCfg_Exit_Text2 = _ledConfig.ExitTextArray.ElementAtOrDefault(1);
            LEDCfg_Exit_Text3 = _ledConfig.ExitTextArray.ElementAtOrDefault(2);
            LEDCfg_Exit_Text4 = _ledConfig.ExitTextArray.ElementAtOrDefault(3);
        }

        /// <summary>
        /// 填充更改LED配置表单
        /// </summary>
        private void FillLEDConfigForm()
        {
            var ledConfig = _systemConfigDictionaries
                .FirstOrDefault(p => p.Key == SystemConfigKey.LEDConfig);

            _ledConfig.EntranceTextArray[0] = LEDCfg_Entrance_Text1;
            _ledConfig.EntranceTextArray[1] = LEDCfg_Entrance_Text2;
            _ledConfig.EntranceTextArray[2] = LEDCfg_Entrance_Text3;
            _ledConfig.EntranceTextArray[3] = LEDCfg_Entrance_Text4;

            _ledConfig.ExitTextArray[0] = LEDCfg_Exit_Text1;
            _ledConfig.ExitTextArray[1] = LEDCfg_Exit_Text2;
            _ledConfig.ExitTextArray[2] = LEDCfg_Exit_Text3;
            _ledConfig.ExitTextArray[3] = LEDCfg_Exit_Text4;

            if (ledConfig == null)
            {
                db.SystemConfigDictionaries.Add(new SystemConfigDictionary
                {
                    Key = SystemConfigKey.LEDConfig,
                    StringValue = CommonUtil.AJSerializeObject(_ledConfig),
                    CreateDate = DateTime.Now
                });
            }
            else
            {
                ledConfig.StringValue
                    = CommonUtil.AJSerializeObject(_ledConfig);
                db.Entry(ledConfig).State = EntityState.Modified;
            }
        }

        /// <summary>
        /// 识别配置表单初始化
        /// </summary>
        private void InitCarIdentificationConfigForm()
        {
            _carIdentificationConfig = CommonUtil
                .TryGetJSONObject<CarIdentificationConfig>(_systemConfigDictionaries
                .FirstOrDefault(p => p.Key == SystemConfigKey.CarIdentificationConfig)
                ?.StringValue) ?? new CarIdentificationConfig
                {
                    ImageSavePath = CarIdentificationConfig.GetDefaultSavePath()
                };

            if (!Directory.Exists(_carIdentificationConfig.ImageSavePath))
            {
                Directory.CreateDirectory(_carIdentificationConfig.ImageSavePath);
            }

            CarIdentificationConfig_ImageSavePath = _carIdentificationConfig.ImageSavePath;
        }

        /// <summary>
        /// 填充更改识别配置表单
        /// </summary>
        private void FillCarIdentificationConfigForm()
        {
            var carIdentificationConfigData = _systemConfigDictionaries
                .FirstOrDefault(p => p.Key == SystemConfigKey.CarIdentificationConfig);

            _carIdentificationConfig.ImageSavePath = CarIdentificationConfig_ImageSavePath;

            if (carIdentificationConfigData == null)
            {
                db.SystemConfigDictionaries.Add(new SystemConfigDictionary
                {
                    Key = SystemConfigKey.CarIdentificationConfig,
                    StringValue = CommonUtil.AJSerializeObject(_carIdentificationConfig),
                    CreateDate = DateTime.Now
                });
            }
            else
            {
                carIdentificationConfigData.StringValue
                    = CommonUtil.AJSerializeObject(_carIdentificationConfig);
                db.Entry(carIdentificationConfigData).State = EntityState.Modified;
            }
        }

        /// <summary>
        /// 接口配置表单初始化
        /// </summary>
        private void InitThirdpartyCarInfoAPIConfigForm()
        {
            _thirdpartyCarInfoAPIConfig = CommonUtil
                .TryGetJSONObject<ThirdPartyCarInfoAPIConfig>(_systemConfigDictionaries
                .FirstOrDefault(p => p.Key == SystemConfigKey.ThirdPartyCarInfoAPIConfig)
                ?.StringValue) ?? new ThirdPartyCarInfoAPIConfig();

            ThirdpartyCarInfoAPI_Url = _thirdpartyCarInfoAPIConfig.Url;
            ThirdpartyCarInfoAPI_CompanyName = _thirdpartyCarInfoAPIConfig.CompanyName;
        }

        /// <summary>
        /// 填充更改接口配置表单
        /// </summary>
        private void FillThirdPartyCarInfoAPIConfigForm()
        {
            _thirdpartyCarInfoAPIConfig.Url = ThirdpartyCarInfoAPI_Url;
            _thirdpartyCarInfoAPIConfig.CompanyName = ThirdpartyCarInfoAPI_CompanyName;

            var thirdpartyCarInfoAPIConfigData = _systemConfigDictionaries
                .FirstOrDefault(p => p.Key == SystemConfigKey.ThirdPartyCarInfoAPIConfig);

            if (thirdpartyCarInfoAPIConfigData == null)
            {
                db.SystemConfigDictionaries.Add(new SystemConfigDictionary
                {
                    Key = SystemConfigKey.ThirdPartyCarInfoAPIConfig,
                    StringValue = CommonUtil.AJSerializeObject(_thirdpartyCarInfoAPIConfig),
                    CreateDate = DateTime.Now
                });
            }
            else
            {
                thirdpartyCarInfoAPIConfigData.StringValue
                    = CommonUtil.AJSerializeObject(_thirdpartyCarInfoAPIConfig);
                db.Entry(thirdpartyCarInfoAPIConfigData).State = EntityState.Modified;
            }
        }
    }

}
