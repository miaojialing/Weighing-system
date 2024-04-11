using AJWPFAdmin.Core.Components.CommonLoading.Views;
using AJWPFAdmin.Core.GlobalEvents;
using AJWPFAdmin.Core.HardwareSDKS.Models;
using AJWPFAdmin.Core.Logger;
using AJWPFAdmin.Core.Models;
using AJWPFAdmin.Core.Mvvm;
using AJWPFAdmin.Core.Utils;
using AJWPFAdmin.Services;
using AJWPFAdmin.Services.EF;
using AJWPFAdmin.Services.EF.Tables;
using AJWPFAdmin.Services.Models;
using DocumentFormat.OpenXml.Wordprocessing;
using Masuit.Tools.Systems;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using MQTTnet;
using MQTTnet.Client;
using Prism.Commands;
using Prism.Events;
using Quartz.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static AJWPFAdmin.Services.AJMQTTService;
using static EntranceGuardStatistic.Modules.Main.ViewModels.DeviceScreenViewModel;

namespace EntranceGuardStatistic.Modules.Main.ViewModels
{
    public class StatisticPanelViewModel : ViewModelBase
    {
        private AJWebSvcConfig _aJWebSvcConfig;
        private bool _topicSubscripted;
        private UserControl _ctrl;

        private ThirdPartyCarInfoAPIConfig _thirdPartyCarInfoApiConfig;
        private CarIdentificationConfig _carIdentificationConfig;
        private PassagewayStatisticConfig _passagewayStatisticConfig;

        private EventHandler<WarehousePassageRecord> _carDequeueEvent;

        private WarehousePassageRecord _curRecord;
        public WarehousePassageRecord CurRecord
        {
            get { return _curRecord; }
            set { SetProperty(ref _curRecord, value); }
        }

        private bool _ajMQTTConnected;
        public bool AJMQTTConnected
        {
            get { return _ajMQTTConnected; }
            set { SetProperty(ref _ajMQTTConnected, value); }
        }

        private bool _loading;
        public bool Loading
        {
            get { return _loading; }
            set { SetProperty(ref _loading, value); }
        }

        private int _total;
        public int Total
        {
            get { return _total; }
            set { SetProperty(ref _total, value); }
        }

        private Visibility _emptyInfoVisible;
        public Visibility EmptyInfoVisible
        {
            get { return _emptyInfoVisible; }
            set { SetProperty(ref _emptyInfoVisible, value); }
        }

        private ObservableCollection<WarehousePassageRecordsGroupRecord> _rows;
        public ObservableCollection<WarehousePassageRecordsGroupRecord> Rows
        {
            get { return _rows; }
            set { SetProperty(ref _rows, value); }
        }

        private DelegateCommand<UserControl> _controlLoadedCmd;
        public DelegateCommand<UserControl> ControlLoadedCmd =>
            _controlLoadedCmd ?? (_controlLoadedCmd = new DelegateCommand<UserControl>(ExecuteControlLoadedCmd));

        void ExecuteControlLoadedCmd(UserControl parameter)
        {
            _ctrl = parameter;
        }

        async void LoadDataAsync()
        {
            Loading = true;

            Rows.Clear();

            var today = DateTime.Today;

            var list = await db.WarehousePassageRecords
                .Where(p => p.CreateDate.Date == today)
                .OrderByDescending(p => p.CreateDate)
                .GroupBy(p => new
                {
                    p.CarNo,
                    p.IDCardNo,
                    p.WatchhouseId
                })
                .Select(p => new WarehousePassageRecordsGroupRecord
                {
                    IDCardNo = p.Key.IDCardNo,
                    WarehouseId = p.Key.WatchhouseId,
                    WarehouseName = p.Max(q => q.WatchhouseName),
                    CarNo = p.Key.CarNo,
                    Count = p.Count(),
                })
                .OrderByDescending(p => p.Count)
                .ToListAsync();


            EmptyInfoVisible = list.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

            Total = list.Sum(p => p.Count);

            Rows.AddRange(list);

            Loading = false;

        }

        private HttpClient _client;
        private AJLog4NetLogger _logger;
        private DbService db;
        private IEventAggregator _eventAggregator;
        private AJMQTTService _aJMQTTService;
        private Queue<WarehousePassageRecord> _carsQueue;
        private SemaphoreSlim _locker = new SemaphoreSlim(1, 1);

        private IEnumerable<DeviceInfo> _devices;

        public StatisticPanelViewModel(DbService dbIns, IEventAggregator eventAggregator,
            AJMQTTService aJMQTTService, AJLog4NetLogger logger)
        {
            db = dbIns;
            _logger = logger;
            _eventAggregator = eventAggregator;
            _carsQueue = new Queue<WarehousePassageRecord>();
            Rows = new ObservableCollection<WarehousePassageRecordsGroupRecord>();
            EmptyInfoVisible = Visibility.Visible;
            _eventAggregator.GetEvent<DeviceNotifyEvent>().Subscribe(OnDeviceNotify);
            _client = new HttpClient();
            eventAggregator.GetEvent<DeviceSetupProgressEvent>().Subscribe((progress) =>
            {
                if (progress.Loading)
                {
                    return;
                }
                if (progress.Completed)
                {
                    _devices = progress.Devices;
                }
                LoadDataAsync();
            });

            _eventAggregator.GetEvent<ApplicationExitEvent>().Subscribe(async () =>
            {
                _client?.Dispose();
                await _aJMQTTService.CloseAsync(true);
            });

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

        private void CheckQueue()
        {
            while (true)
            {
                if (CurRecord != null)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                if (_carsQueue.Count > 0)
                {
                    _carDequeueEvent.Invoke(this, _carsQueue.Dequeue());
                }
                Thread.Sleep(1000);
            }
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

            // 获取所有海康设备, 并截图
            var hikDevices = _devices.Where(p => p.Type == AJWPFAdmin.Core.Enums.DeviceType.监控相机_海康).ToList();
            var captureImages = new List<string>();
            foreach (var dev in hikDevices)
            {
                var img = dev.GetCapture();
                if (string.IsNullOrWhiteSpace(img))
                {
                    continue;
                }
                captureImages.Add(img);
            }

            var now = DateTime.Now;

            var record = new WarehousePassageRecord
            {
                CreateDate = now,
                CarNo = carNo,
                IDCardNo = carInfo?.IDCardNo,
                Direction = deviceInfo.Direction,
                PassagewayId = deviceInfo.PassagewayId,
                PassagewayName = deviceInfo.PassagewayName,
                ShipStartDate = now,
                TypeId = carType.Id,
                TypeName = carInfo?.TypeName ?? "临时车",
                WatchhouseId = deviceInfo.WatchhouseId,
                WatchhouseName = deviceInfo.WatchhouseName,
                CameraCaptureFile = CommonUtil.AJSerializeObject(captureImages),
                IdentifiedCaptureSmallFile = deviceInfo.CarIdentificationPlateResult.ClipImgFile,
                IdentifiedCaptureFullFile = deviceInfo.CarIdentificationPlateResult.FullImgFile,
                EnablePassagewayStatistic = carType.EnablePassagewayStatistic,
            };

            _carsQueue.Enqueue(record);
        }


        private async Task ConfirmPassAsync(string parameter)
        {

            await _locker.WaitAsync();

            // 处理通道车辆统计
            var canAdd = ProcessPassagewayStatistic();

            if (!canAdd) { return; }

            // 判断进出场
            if (CurRecord.Direction == AJWPFAdmin.Core.Enums.PassagewayDirection.进)
            {
                ProcessCarIn();
            }
            else
            {
                ProcessCarOut();
            }

            //  扣除仓库剩余数量,
            var warehouseId = CurRecord.WarehouseId;
            var warehouseData = db.Warehouses.FirstOrDefault(p => p.Id == warehouseId);
            if (warehouseData != null)
            {
                warehouseData.ResidualCarLimit -= 1;
                db.Entry(warehouseData).State = EntityState.Modified;
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
                _logger.Error($"{CurRecord.CarNo} 保存通行记录发生异常", e);
                await CommonUtil.ShowAlertDialogAsync(new MaterialDesignExtensions.Controls.AlertDialogArguments
                {
                    Title = $"保存失败",
                    Message = $"{CurRecord.CarNo}：{e.Message}"
                });
                _locker.Release();
                return;
            }

            //  等待1s, 防止一些问题
            await Task.Delay(1000);

            _ctrl.Dispatcher.Invoke(LoadDataAsync);

            _locker.Release();

            CurRecord = null;
        }


        private (string fullImageFile, string clipImageFile, string[] cameraImage) CopyTempImagesToSavePath()
        {
            var targetCarNoImagesPath = Path.Combine(Directory.GetCurrentDirectory(), "CaptureImages");

            if (!Directory.Exists(targetCarNoImagesPath))
            {
                Directory.CreateDirectory(targetCarNoImagesPath);
            }

            var fullImg = CurRecord.IdentifiedCaptureFullFile;
            var clipImage = CurRecord.IdentifiedCaptureSmallFile;
            var cameraImageArrayJsonStr = CurRecord.CameraCaptureFile;

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

            string[] cameraImageArray = null;
            if (!string.IsNullOrWhiteSpace(cameraImageArrayJsonStr))
            {
                cameraImageArray = CommonUtil.TryGetJSONObject<string[]>(cameraImageArrayJsonStr);
                if (cameraImageArray?.Length > 0)
                {
                    for (int i = 0; i < cameraImageArray.Length; i++)
                    {
                        var item = cameraImageArray[i];
                        var targetCameraImage = Path.Combine(targetCarNoImagesPath, Path.GetFileName(item));
                        CommonUtil.TryCopyFile(item, targetCameraImage);
                        cameraImageArray[i] = targetCameraImage;
                    }
                }
                
            }

            return (targetFullImageFile, targetClipImageFile, cameraImageArray);
        }

        private void ProcessCarIn()
        {
            var carNo = CurRecord.CarNo;

            var inRecord = db.WarehousePassageRecords
                        .OrderBy(p => p.CreateDate)
                        .FirstOrDefault(p => p.ShipEndDate == null
                        && p.Direction == AJWPFAdmin.Core.Enums.PassagewayDirection.进
                        && p.CarNo == carNo);

            // 赋值识别临时图片到正式文件夹
            var (fullImageFile, clipImageFile, cameraImageFiles) = CopyTempImagesToSavePath();

            if (!string.IsNullOrWhiteSpace(fullImageFile))
            {
                CurRecord.IdentifiedCaptureFullFile =
                CommonUtil.AJSerializeObject(new string[] { fullImageFile });
            }

            if (!string.IsNullOrWhiteSpace(clipImageFile))
            {
                CurRecord.IdentifiedCaptureSmallFile =
                CommonUtil.AJSerializeObject(new string[] { clipImageFile });
            }

            if (cameraImageFiles?.Length > 0)
            {
                CurRecord.CameraCaptureFile = CommonUtil.AJSerializeObject(cameraImageFiles);
            }

            if (inRecord != null)
            {
                inRecord.ShipStartDate = CurRecord.ShipStartDate;

                inRecord.Direction = CurRecord.Direction;

                inRecord.PassagewayId = CurRecord.PassagewayId;
                inRecord.PassagewayName = CurRecord.PassagewayName;
                inRecord.ShipStartDate = CurRecord.ShipStartDate;

                inRecord.TypeId = CurRecord.TypeId;
                inRecord.TypeName = CurRecord.TypeName;

                inRecord.WatchhouseId = CurRecord.WatchhouseId;
                inRecord.WatchhouseName = CurRecord.WatchhouseName;

                inRecord.WarehouseId = CurRecord.WarehouseId;
                inRecord.WarehouseName = CurRecord.WarehouseName;
                db.Entry(inRecord).State = EntityState.Modified;

            }
            db.WarehousePassageRecords.Add(CurRecord);
        }

        private void ProcessCarOut()
        {
            var carNo = CurRecord.CarNo;
            // 获取已存在的最最早的一条入场的记录
            var existsRecord = db.WarehousePassageRecords
                .OrderBy(p => p.CreateDate)
                .FirstOrDefault(p => p.ShipEndDate == null
                && p.Direction == AJWPFAdmin.Core.Enums.PassagewayDirection.进
                && p.CarNo == carNo);

            // 赋值识别临时图片到正式文件夹
            var (fullImageFile, clipImageFile, cameraImageFiles) = CopyTempImagesToSavePath();

            if (existsRecord == null)
            {
                // 如果 没有,则创建一条, 进厂日期 是  1990-01-01
                CurRecord.Id = SnowFlake.GetInstance().GetLongId();
                CurRecord.ShipStartDate = DateTime.Parse("1990-01-01");
                CurRecord.ShipEndDate = DateTime.Now;

                if (!string.IsNullOrEmpty(fullImageFile))
                {
                    CurRecord.IdentifiedCaptureFullFile = CommonUtil
                    .AJSerializeObject(new string[] { fullImageFile });
                }

                if (!string.IsNullOrEmpty(clipImageFile))
                {
                    CurRecord.IdentifiedCaptureSmallFile = CommonUtil
                    .AJSerializeObject(new string[] { clipImageFile });
                }

                if (cameraImageFiles?.Length > 0)
                {
                    CurRecord.CameraCaptureFile = CommonUtil.AJSerializeObject(cameraImageFiles);
                }

                CurRecord.ExitWatchhouseId = CurRecord.WatchhouseId;
                CurRecord.ExitWatchhouseName = CurRecord.WatchhouseName;
                CurRecord.ExitPassagewayId = CurRecord.PassagewayId;
                CurRecord.ExitPassagewayName = CurRecord.PassagewayName;
                CurRecord.ExitDeviceId = CurRecord.DeviceId;
                CurRecord.ExitDeviceCode = CurRecord.DeviceCode;

            }
            else
            {
                existsRecord.ExitDeviceCode = CurRecord.DeviceCode;
                existsRecord.ExitDeviceId = CurRecord.DeviceId;

                existsRecord.ExitPassagewayId = CurRecord.PassagewayId;
                existsRecord.ExitPassagewayName = CurRecord.PassagewayName;

                existsRecord.TypeId = CurRecord.TypeId;
                existsRecord.TypeName = CurRecord.TypeName;

                existsRecord.ExitWatchhouseId = CurRecord.WatchhouseId;
                existsRecord.ExitWatchhouseName = CurRecord.WatchhouseName;

                existsRecord.Direction = AJWPFAdmin.Core.Enums.PassagewayDirection.出;
                existsRecord.ShipEndDate = DateTime.Now;


                existsRecord.WarehouseId = CurRecord.WarehouseId;
                existsRecord.WarehouseName = CurRecord.WarehouseName;
                db.Entry(existsRecord).State = EntityState.Modified;

            }
            db.WarehousePassageRecords.Add(CurRecord);

        }

        private void OnCarDequeue(object sender, WarehousePassageRecord e)
        {
            _ctrl.Dispatcher.Invoke(async () =>
            {
                CurRecord = e;
                await ConfirmPassAsync(string.Empty);
            });

        }

        private async Task<ThirdPartyCarInfoAPIResponse> GetThirdPartyCarInfoAsync(string carNo)
        {
            var res = new ThirdPartyCarInfoAPIResponse();

            if (_thirdPartyCarInfoApiConfig == null)
            {
                var cfgJson = db.SystemConfigDictionaries
                    .Where(p => p.Key == AJWPFAdmin.Core.Enums.SystemConfigKey.ThirdPartyCarInfoAPIConfig)
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

        private bool ProcessPassagewayStatistic()
        {
            if (!CurRecord.EnablePassagewayStatistic)
            {
                return false;
            }
            // 先判通道启用车辆统计状态
            var pswId = CurRecord.PassagewayId;
            var passwageway = db.Passageways.Where(p => pswId == p.Id).Select(p => new
            {
                p.WarehouseId,
                p.WarehouseName,
                p.CountCarEnable
            }).FirstOrDefault();

            var typeId = CurRecord.TypeId;

            if (!(passwageway?.CountCarEnable).GetValueOrDefault())
            {
                return false;
            }

            CurRecord.WarehouseId = passwageway.WarehouseId;
            CurRecord.WarehouseName = passwageway.WarehouseName;

            if (_passagewayStatisticConfig == null)
            {
                var cfgJson = db.SystemConfigDictionaries
                    .Where(p => p.Key == AJWPFAdmin.Core.Enums.SystemConfigKey.PassagewayStatisticConfig)
                    .Select(p => p.StringValue).FirstOrDefault();

                _passagewayStatisticConfig
                    = CommonUtil.TryGetJSONObject<PassagewayStatisticConfig>(cfgJson)
                    ?? new PassagewayStatisticConfig();
            }

            // 一进一出才算
            var carNo = CurRecord.CarNo;
            var warehouseId = CurRecord.WarehouseId;

            // 如果是一进一出,并且这次方向是出
            if (_passagewayStatisticConfig.Type == PassagewayStatisticType.Twice
                && CurRecord.Direction == AJWPFAdmin.Core.Enums.PassagewayDirection.出)
            {
                // 如果不存在相同仓库的在场记录,则跳出
                if (!db.WarehousePassageRecords.OrderBy(p => p.CreateDate).Any(p => p.ShipEndDate == null
                && p.WarehouseId == warehouseId
                && p.Direction == AJWPFAdmin.Core.Enums.PassagewayDirection.进
                && p.CarNo == carNo))
                {
                    return false;
                }

            }

            CurRecord.Id = SnowFlake.GetInstance().GetLongId();
            CurRecord.CreateDate = DateTime.Now;

            return true;
        }

        private async void ConnectToMQTTSvc()
        {
            await _aJMQTTService.ConnectAsync(_aJWebSvcConfig.IP, _aJWebSvcConfig.MQTTPort);
        }

        private void OnAJMQTTSvcStatusChanged(bool status)
        {
            AJMQTTConnected = status;
            if (!_topicSubscripted && status)
            {
                //_aJMQTTService.SubscribeAsync(AJMQTTService.CARPASSEDTOPIC, OnCarPassedMessageReceived);
                _topicSubscripted = true;
            }
        }

        private Task OnCarPassedMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            // e.ApplicationMessage.ConvertPayloadToString()
            //System.Diagnostics.Trace.WriteLine("OnCarPassedMessageReceived");
            e.IsHandled = true;
            _ctrl.Dispatcher.Invoke(LoadDataAsync);
            return Task.CompletedTask;
        }
    }
}
