using AJWPFAdmin.Core;
using AJWPFAdmin.Core.GlobalEvents;
using AJWPFAdmin.Core.HardwareSDKS.Models;
using AJWPFAdmin.Core.Logger;
using AJWPFAdmin.Core.Models;
using AJWPFAdmin.Core.Mvvm;
using AJWPFAdmin.Core.Utils;
using AJWPFAdmin.Services;
using AJWPFAdmin.Services.EF;
using AJWPFAdmin.Services.EF.Tables;
using DnsClient.Protocol;
using Masuit.Tools;
using Masuit.Tools.Systems;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;
using Quartz.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Printing;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Threading;
using static AJWPFAdmin.Modules.Main.ViewModels.CarIdentificationPanelViewModel;
using static AJWPFAdmin.Services.AJMQTTService;

namespace AJWPFAdmin.Modules.Main.ViewModels
{
    public class CarIdentificationPanelViewModel : ViewModelBase
    {
        private HttpClient _client;
        private ThirdPartyCarInfoAPIConfig _thirdPartyCarInfoApiConfig;
        private CarIdentificationConfig _carIdentificationConfig;
        private PassagewayStatisticConfig _passagewayStatisticConfig;
        private AJWebSvcConfig _aJWebSvcConfig;

        private EventHandler<ShippingRecord> _carDequeueEvent;

        private bool _saving;
        public bool Saving
        {
            get { return _saving; }
            set { SetProperty(ref _saving, value); }
        }

        private bool _ajMQTTConnected;
        public bool AJMQTTConnected
        {
            get { return _ajMQTTConnected; }
            set { SetProperty(ref _ajMQTTConnected, value); }
        }

        private IDialogService _dialogSvc;
        private IEventAggregator _eventAggregator;
        private DbService db;
        private DeviceReceiveCmdEvent _deviceReceiveCmdEvent;
        private CarIdentificationPassedEvent _carIdentificationPassedEvent;
        private AJLog4NetLogger _logger;
        private AJMQTTService _aJMQTTService;
        private SemaphoreSlim _locker = new SemaphoreSlim(1, 1);

        private UserControl _ctrl;

        private ShippingRecord _curRecord;
        public ShippingRecord CurRecord
        {
            get { return _curRecord; }
            set { SetProperty(ref _curRecord, value); }
        }

        private int _queueCount;
        public int QueueCount
        {
            get { return _queueCount; }
            set { SetProperty(ref _queueCount, value); }
        }

        private DelegateCommand<UserControl> _controlLoadedCmd;
        public DelegateCommand<UserControl> ControlLoadedCmd =>
            _controlLoadedCmd ?? (_controlLoadedCmd = new DelegateCommand<UserControl>(ExecuteControlLoadedCmd));

        void ExecuteControlLoadedCmd(UserControl parameter)
        {
            _ctrl = parameter;
        }

        public CarIdentificationPanelViewModel(DbService dbIns, IDialogService dialogSvc,
            IEventAggregator eventAggregator, AJLog4NetLogger logger, AJMQTTService aJMQTTService)
        {
            db = dbIns;
            _dialogSvc = dialogSvc;
            _eventAggregator = eventAggregator;
            _carsQueue = new Queue<ShippingRecord>();
            _eventAggregator.GetEvent<DeviceNotifyEvent>().Subscribe(OnDeviceNotify);
            _deviceReceiveCmdEvent = _eventAggregator.GetEvent<DeviceReceiveCmdEvent>();
            _carIdentificationPassedEvent = _eventAggregator.GetEvent<CarIdentificationPassedEvent>();
            _client = new HttpClient();
            _eventAggregator.GetEvent<ApplicationExitEvent>().Subscribe(async () =>
            {
                _client.Dispose();
                await _aJMQTTService.CloseAsync(true);
            });
            _logger = logger;
            _aJWebSvcConfig = new AJWebSvcConfig();
            // 暂时写死使用初始化默认值
            _aJWebSvcConfig.Init();
            _aJMQTTService = aJMQTTService;
            _eventAggregator.GetEvent<AJMQTTSvcStatusChangedEvent>().Subscribe(OnAJMQTTSvcStatusChanged);
            Task.Run(ConnectToMQTTSvc);
            _carDequeueEvent += OnCarDequeue;
            //开启一个长线程,从队列读取数据处理
            Task.Factory.StartNew(CheckQueue, TaskCreationOptions.LongRunning);
            
        }

        private async void ConnectToMQTTSvc()
        {
            await _aJMQTTService.ConnectAsync(_aJWebSvcConfig.IP, _aJWebSvcConfig.MQTTPort);
        }

        private void OnAJMQTTSvcStatusChanged(bool status)
        {
            AJMQTTConnected = status;
        }

        private void OnCarDequeue(object sender, ShippingRecord e)
        {
            _ctrl.Dispatcher.Invoke(async () =>
            {
                CurRecord = e;
                PassCmd.RaiseCanExecuteChanged();
                if (CurRecord.AutoPass)
                {
                    await ConfirmPassAsync(string.Empty);
                }
                else
                {
                    _deviceReceiveCmdEvent.Publish(new DeviceReceiveCmdEventArgs
                    {
                        DeviceId = CurRecord.DeviceId,
                        Type = DeviceReceiveCmdType.开闸,
                        Data = new DeviceOpenGateCmdParameter
                        {
                            Direction = CurRecord.Direction,
                            Open = CurRecord.AutoPass,
                            TTSText = "请稍后",
                            LEDTextLines = new string[] {
                                    CurRecord.CarNo,
                                    CurRecord.TypeName,
                                    "请稍后",
                                    (string.IsNullOrWhiteSpace(CurRecord.PaiFangJieDuan)
                                    ? "请登记" : CurRecord.PaiFangJieDuan)
                                }
                        }
                    });
                }
            });

        }

        private async void OnDeviceNotify(DeviceInfo deviceInfo)
        {
            var carNo = deviceInfo.CarIdentificationPlateResult.CarNo;

            var isReaderDevice = deviceInfo is SioReaderDevice;

            var carInfo = await db.Cars
                .IfWhere(() => isReaderDevice, p => p.IDCardNo == carNo)
                .IfWhere(() => !isReaderDevice, p => p.CarNo == carNo)
                .FirstOrDefaultAsync();

            CarType carType;
            if (carInfo != null)
            {
                carType = db.CarTypes.Where(p => p.Id == carInfo.TypeId).AsNoTracking().FirstOrDefault();
            }
            else
            {
                carType = db.CarTypes.Where(p => p.SysRequired && p.Name == "临时车")
                    .AsNoTracking().FirstOrDefault();
            }

            if (carType == null)
            {
                await CommonUtil.ShowAlertDialogAsync(new MaterialDesignExtensions.Controls.AlertDialogArguments
                {
                    Title = $"无法处理 {carNo}",
                    Message = $"无关联车类型,请检查系统配置"
                });
                return;
            }

            // 第三方接口获取台账数据
            ThirdPartyCarInfoAPIResponse carRes = null;
            if ((carInfo?.TeamName ?? string.Empty) != "固定车")
            {
                carRes = await GetThirdPartyCarInfoAsync(carNo);
                if (!carRes.success)
                {
                    // 写日志
                    _logger.Warning($"第三方车辆台账信息接口错误:{carRes.msg}");
                }
                else
                {
                    //第三方台账信息同步
                    carInfo?.UpdateFromThirdPartyAPI(ref db, carRes);
                }
            }


            var now = DateTime.Now;

            var record = new ShippingRecord
            {
                ArriveDate = now,
                CreateDate = now,
                Berth = "",
                CarNo = carNo,
                IDCardNo = carInfo?.IDCardNo,
                DeviceCode = deviceInfo.Code,
                DeviceId = deviceInfo.Id,
                Direction = deviceInfo.Direction,
                EngineNo = carRes == null ? "" : carRes.data?.engineNumber ?? carInfo?.EngineNo,
                MaterialCategory = "",
                MaterialName = carRes == null ? "" : (carRes.data?.goods ?? carInfo?.MaterialName) ?? "",
                PaiFangJieDuan = carRes == null ? "" : carRes.data?.emissionstage ?? carInfo?.PaiFangJieDuan,
                PassagewayId = deviceInfo.PassagewayId,
                PassagewayName = deviceInfo.PassagewayName,
                ReceiverName = "",
                SenderName = "",
                ShipStartDate = now,
                TeamName = carRes == null ? "" : carRes.data?.fleet ?? carInfo?.TeamName,
                TypeId = carType.Id,
                TypeName = carInfo?.TypeName ?? "临时车",
                VIN = carRes == null ? "" : carRes.data?.vin ?? carInfo?.VIN,
                WatchhouseId = deviceInfo.WatchhouseId,
                WatchhouseName = deviceInfo.WatchhouseName,
                AutoPass = carType.AutoPass,
                EntranceCameraCaptureFile
                = deviceInfo.Direction == Core.Enums.PassagewayDirection.进 ? deviceInfo.CarIdentificationPlateResult.FullImgFile : string.Empty,
                EntranceIdentifiedCaptureFile = deviceInfo.Direction == Core.Enums.PassagewayDirection.进
                ? deviceInfo.CarIdentificationPlateResult.ClipImgFile : string.Empty,
                ExitCameraCaptureFile = deviceInfo.Direction == Core.Enums.PassagewayDirection.出 ? deviceInfo.CarIdentificationPlateResult.FullImgFile : string.Empty,
                ExitIdentifiedCaptureFile = deviceInfo.Direction == Core.Enums.PassagewayDirection.出
                ? deviceInfo.CarIdentificationPlateResult.ClipImgFile : string.Empty,
                EnablePassagewayStatistic = carType.EnablePassagewayStatistic,
            };

            _carsQueue.Enqueue(record);
        }

        private Queue<ShippingRecord> _carsQueue;

        private DelegateCommand<string> _passCmd;
        public DelegateCommand<string> PassCmd =>
            _passCmd ?? (_passCmd = new DelegateCommand<string>(ExecutePassCmd, CanExecutePassCmd));

        async void ExecutePassCmd(string parameter)
        {
            await ConfirmPassAsync(parameter);
        }

        private async Task ConfirmPassAsync(string parameter)
        {
            if ("ignore".Equals(parameter, StringComparison.OrdinalIgnoreCase))
            {
                CurRecord = null;
                Saving = false;
                PassCmd.RaiseCanExecuteChanged();
                return;
            }

            Saving = true;

            await _locker.WaitAsync();

            // 判断进出场
            if (CurRecord.Direction == Core.Enums.PassagewayDirection.进)
            {
                ProcessCarIn();
            }
            else
            {
                ProcessCarOut();
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // 处理并发问题
                foreach (var entity in ex.Entries)
                {
                    entity.Reload();
                }
                await ConfirmPassAsync(parameter);
                return;
            }
            catch (Exception e)
            {
                // 如果是其他问题， 写日志， 提示错误
                _logger.Error($"{CurRecord.CarNo} 保存台账记录发生异常", e);
                await CommonUtil.ShowAlertDialogAsync(new MaterialDesignExtensions.Controls.AlertDialogArguments
                {
                    Title = $"保存失败",
                    Message = $"{CurRecord.CarNo}：{e.Message}"
                });
                _locker.Release();
                return;
            }

            var ledLine4Text = string.IsNullOrWhiteSpace(CurRecord.PaiFangJieDuan)
                        ? "请登记" : CurRecord.PaiFangJieDuan;

            var now = DateTime.Now.Date;
            var carNo = CurRecord.CarNo;
            var warehouseId = CurRecord.WatchhouseId;
            var count = db.WarehousePassageRecords
                .Count(p => p.CreateDate.Date == now && p.WarehouseId == warehouseId && p.CarNo == carNo);

            ledLine4Text = $"今日 {count}次";

            // 给设备发送开闸事件,由设备进行开闸逻辑处理
            _deviceReceiveCmdEvent.Publish(new DeviceReceiveCmdEventArgs
            {
                DeviceId = CurRecord.DeviceId,
                Type = DeviceReceiveCmdType.开闸,
                Data = new DeviceOpenGateCmdParameter
                {
                    Direction = CurRecord.Direction,
                    Open = true,
                    TTSText = "允许通行",
                    LEDTextLines = new string[] {
                        CurRecord.CarNo,
                        CurRecord.TypeName,
                        "允许通行",
                        ledLine4Text
                    }
                }
            });

            //  等待1s, 防止一些问题
            await Task.Delay(1000);

            // 通知给其他需要刷新台账记录的组件
            _carIdentificationPassedEvent.Publish();
            

            _locker.Release();

            CurRecord = null;
            Saving = false;
            PassCmd.RaiseCanExecuteChanged();
        }

        bool CanExecutePassCmd(string parameter)
        {
            return CurRecord != null && !string.IsNullOrWhiteSpace(CurRecord.CarNo);
        }

        private void ProcessCarIn()
        {
            var carNo = CurRecord.CarNo;

            var inRecord = db.ShippingRecords
                        .OrderBy(p => p.CreateDate)
                        .FirstOrDefault(p => p.ShipEndDate == null
                        && p.Direction == Core.Enums.PassagewayDirection.进
                        && p.CarNo == carNo);

            // 赋值识别临时图片到正式文件夹
            var (fullImageFile, clipImageFile) = CopyTempImagesToSavePath();

            if (!string.IsNullOrWhiteSpace(fullImageFile))
            {
                CurRecord.EntranceCameraCaptureFile =
                CommonUtil.AJSerializeObject(new string[] { fullImageFile });
            }

            if (!string.IsNullOrWhiteSpace(clipImageFile))
            {
                CurRecord.EntranceIdentifiedCaptureFile =
                CommonUtil.AJSerializeObject(new string[] { clipImageFile });
            }

            if (inRecord != null)
            {
                inRecord.ArriveDate = DateTime.Now;
                inRecord.ShipStartDate = CurRecord.ShipStartDate;

                var delFiles = new List<string>();

                var fullImages = CommonUtil.TryGetJSONObject<string[]>(inRecord.EntranceCameraCaptureFile);

                if (fullImages != null && fullImages.Length > 0)
                {
                    delFiles.AddRange(fullImages);
                }

                var clipImages = CommonUtil.TryGetJSONObject<string[]>(inRecord.EntranceIdentifiedCaptureFile);
                if (clipImages != null && clipImages.Length > 0)
                {
                    delFiles.AddRange(clipImages);
                }

                if (delFiles.Count > 0)
                {
                    CommonUtil.TryDeleteFiles(delFiles);
                }

                inRecord.DeviceCode = CurRecord.DeviceCode;
                inRecord.DeviceId = CurRecord.DeviceId;
                inRecord.Direction = CurRecord.Direction;
                inRecord.EngineNo = CurRecord.EngineNo;
                inRecord.MaterialName = CurRecord.MaterialName;
                inRecord.PaiFangJieDuan = CurRecord.PaiFangJieDuan;
                inRecord.PassagewayId = CurRecord.PassagewayId;
                inRecord.PassagewayName = CurRecord.PassagewayName;
                inRecord.ShipStartDate = CurRecord.ShipStartDate;
                inRecord.TeamName = CurRecord.TeamName;
                inRecord.TypeId = CurRecord.TypeId;
                inRecord.TypeName = CurRecord.TypeName;
                inRecord.VIN = CurRecord.VIN;
                inRecord.WatchhouseId = CurRecord.WatchhouseId;
                inRecord.WatchhouseName = CurRecord.WatchhouseName;
                inRecord.AutoPass = CurRecord.AutoPass;
                inRecord.EntranceCameraCaptureFile = CurRecord.EntranceCameraCaptureFile;
                inRecord.EntranceIdentifiedCaptureFile = CurRecord.EntranceIdentifiedCaptureFile;
                inRecord.WarehouseId = CurRecord.WarehouseId;
                inRecord.WarehouseName = CurRecord.WarehouseName;
                db.Entry(inRecord).State = EntityState.Modified;

                CurRecord.Id = inRecord.Id;
            }
            else
            {
                CurRecord.Id = SnowFlake.GetInstance().GetLongId();
                db.ShippingRecords.Add(CurRecord);
            }
        }

        private void ProcessCarOut()
        {
            var carNo = CurRecord.CarNo;
            // 获取已存在的最最早的一条入场的记录
            var existsRecord = db.ShippingRecords
                .OrderBy(p => p.CreateDate)
                .FirstOrDefault(p => p.ShipEndDate == null
                && p.Direction == Core.Enums.PassagewayDirection.进
                && p.CarNo == carNo);

            // 赋值识别临时图片到正式文件夹
            var (fullImageFile, clipImageFile) = CopyTempImagesToSavePath();

            if (existsRecord == null)
            {
                // 如果 没有,则创建一条, 进厂日期 是  1990-01-01
                CurRecord.Id = SnowFlake.GetInstance().GetLongId();
                CurRecord.ShipStartDate = DateTime.Parse("1990-01-01");
                CurRecord.ShipEndDate = DateTime.Now;

                if (!string.IsNullOrEmpty(fullImageFile))
                {
                    CurRecord.ExitCameraCaptureFile = CommonUtil
                    .AJSerializeObject(new string[] { fullImageFile });
                }

                if (!string.IsNullOrEmpty(clipImageFile))
                {
                    CurRecord.ExitIdentifiedCaptureFile = CommonUtil
                    .AJSerializeObject(new string[] { clipImageFile });
                }

                CurRecord.ExitWatchhouseId = CurRecord.WatchhouseId;
                CurRecord.ExitWatchhouseName = CurRecord.WatchhouseName;
                CurRecord.ExitPassagewayId = CurRecord.PassagewayId;
                CurRecord.ExitPassagewayName = CurRecord.PassagewayName;
                CurRecord.ExitDeviceId = CurRecord.DeviceId;
                CurRecord.ExitDeviceCode = CurRecord.DeviceCode;

                db.ShippingRecords.Add(CurRecord);
            }
            else
            {
                existsRecord.ExitDeviceCode = CurRecord.DeviceCode;
                existsRecord.ExitDeviceId = CurRecord.DeviceId;
                existsRecord.EngineNo = CurRecord.EngineNo;
                existsRecord.MaterialName = CurRecord.MaterialName;
                existsRecord.PaiFangJieDuan = CurRecord.PaiFangJieDuan;
                existsRecord.ExitPassagewayId = CurRecord.PassagewayId;
                existsRecord.ExitPassagewayName = CurRecord.PassagewayName;
                existsRecord.TeamName = CurRecord.TeamName;
                existsRecord.TypeId = CurRecord.TypeId;
                existsRecord.TypeName = CurRecord.TypeName;
                existsRecord.VIN = CurRecord.VIN;
                existsRecord.ExitWatchhouseId = CurRecord.WatchhouseId;
                existsRecord.ExitWatchhouseName = CurRecord.WatchhouseName;
                existsRecord.AutoPass = CurRecord.AutoPass;
                existsRecord.Direction = Core.Enums.PassagewayDirection.出;
                existsRecord.ShipEndDate = DateTime.Now;

                if (!string.IsNullOrEmpty(fullImageFile))
                {
                    existsRecord.ExitCameraCaptureFile = CommonUtil
                        .AJSerializeObject(new string[] { fullImageFile });
                }

                if (!string.IsNullOrEmpty(clipImageFile))
                {
                    existsRecord.ExitIdentifiedCaptureFile = CommonUtil
                        .AJSerializeObject(new string[] { clipImageFile });
                }

                existsRecord.WarehouseId = CurRecord.WarehouseId;
                existsRecord.WarehouseName = CurRecord.WarehouseName;
                db.Entry(existsRecord).State = EntityState.Modified;

                CurRecord.Id = existsRecord.Id;
            }

        }

        private void CheckQueue()
        {
            while (true)
            {
                if (CurRecord != null)
                {
                    QueueCount = _carsQueue.Count;
                    PassCmd.RaiseCanExecuteChanged();
                    Thread.Sleep(1000);
                    continue;
                }

                if (_carsQueue.Count > 0)
                {
                    _carDequeueEvent.Invoke(this, _carsQueue.Dequeue());
                }
                QueueCount = _carsQueue.Count;
                Thread.Sleep(1000);
            }
        }

        private (string fullImageFile, string clipImageFile) CopyTempImagesToSavePath()
        {
            var targetCarNoImagesPath = CarIdentificationConfig.GetDefaultSavePath();
            if (_carIdentificationConfig == null)
            {
                var cfgJson = db.SystemConfigDictionaries
                    .Where(p => p.Key == Core.Enums.SystemConfigKey.CarIdentificationConfig)
                    .Select(p => p.StringValue).FirstOrDefault();

                _carIdentificationConfig = CommonUtil.TryGetJSONObject<CarIdentificationConfig>(cfgJson);
            }

            if (!string.IsNullOrWhiteSpace(_carIdentificationConfig?.ImageSavePath))
            {
                targetCarNoImagesPath = _carIdentificationConfig.ImageSavePath;
            }

            if (!Directory.Exists(targetCarNoImagesPath))
            {
                Directory.CreateDirectory(targetCarNoImagesPath);
            }

            var fullImg = CurRecord.Direction == Core.Enums.PassagewayDirection.进
                ? CurRecord.EntranceCameraCaptureFile : CurRecord.ExitCameraCaptureFile;
            var clipImage = CurRecord.Direction == Core.Enums.PassagewayDirection.进
                ? CurRecord.EntranceIdentifiedCaptureFile : CurRecord.ExitIdentifiedCaptureFile;

            var targetFullImageFile = string.Empty;

            if (!string.IsNullOrWhiteSpace(fullImg))
            {
                targetFullImageFile = Path.Combine(targetCarNoImagesPath, Path.GetFileName(fullImg));

                CommonUtil.TryCopyFile(fullImg, targetFullImageFile);
            }

            var targetClipImageFile = string.Empty;

            if (!string.IsNullOrWhiteSpace(clipImage))
            {
                targetClipImageFile = Path.Combine(targetCarNoImagesPath, Path.GetFileName(clipImage));
                CommonUtil.TryCopyFile(clipImage, targetClipImageFile);
            }

            return (targetFullImageFile, targetClipImageFile);
        }

        private async Task<ThirdPartyCarInfoAPIResponse> GetThirdPartyCarInfoAsync(string carNo)
        {
            var res = new ThirdPartyCarInfoAPIResponse();

            if (_thirdPartyCarInfoApiConfig == null)
            {
                var cfgJson = db.SystemConfigDictionaries
                    .Where(p => p.Key == Core.Enums.SystemConfigKey.ThirdPartyCarInfoAPIConfig)
                    .Select(p => p.StringValue).FirstOrDefault();

                _thirdPartyCarInfoApiConfig = CommonUtil
                    .TryGetJSONObject<ThirdPartyCarInfoAPIConfig>(cfgJson);

                if (_thirdPartyCarInfoApiConfig == null
                    || string.IsNullOrWhiteSpace(_thirdPartyCarInfoApiConfig.Url))
                {
                    res.success = false;
                    res.msg = "未配置接口地址";

                    return res;
                }

            }
            try
            {
                var json = await _client.GetStringAsync($"{_thirdPartyCarInfoApiConfig.Url}?plate={carNo}&company={_thirdPartyCarInfoApiConfig.CompanyName}");

                res = CommonUtil.TryGetJSONObject<ThirdPartyCarInfoAPIResponse>(json);
            }
            catch (Exception e)
            {
                res.success = false;
                res.msg = e.Message;
            }

            return res;

        }

        public class CarIdentificationPassedEvent : PubSubEvent
        {

        }
    }
}
