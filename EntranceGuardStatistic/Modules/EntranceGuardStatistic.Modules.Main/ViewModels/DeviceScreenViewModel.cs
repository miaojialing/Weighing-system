using AJWPFAdmin.Core;
using AJWPFAdmin.Core.Enums;
using AJWPFAdmin.Core.GlobalEvents;
using AJWPFAdmin.Core.HardwareSDKS.Models;
using AJWPFAdmin.Core.HardwareSDKS.VzClient;
using AJWPFAdmin.Core.Logger;
using AJWPFAdmin.Core.Models;
using AJWPFAdmin.Core.Mvvm;
using AJWPFAdmin.Core.Utils;
using EntranceGuardStatistic.Modules.Main.Views;
using AJWPFAdmin.Services.EF;
using AJWPFAdmin.Services.EF.Tables;
using Masuit.Tools;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Formats.Asn1;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using static AJWPFAdmin.Core.ExceptionTool;

namespace EntranceGuardStatistic.Modules.Main.ViewModels
{
    public partial class DeviceScreenViewModel : ViewModelBase
    {
        public class DeviceSetupProgressEvent : PubSubEvent<DeviceSetupProgressModel>
        {

        }

        private static readonly DeviceType[] _displayDeviceTypes = new DeviceType[]
                {
                    DeviceType.监控相机_海康
                };

        public class DeviceSetupProgressModel
        {
            public bool Loading { get; set; }
            public string ProgressText { get; set; }
        }

        private Visibility _emptyInfoVisibility;
        public Visibility EmptyInfoVisibility
        {
            get { return _emptyInfoVisibility; }
            set { SetProperty(ref _emptyInfoVisibility, value); }
        }

        private List<DeviceInfo> _deviceSource;


        private ObservableCollection<DeviceInfo> _devices;
        public ObservableCollection<DeviceInfo> DeviceList
        {
            get { return _devices; }
            set { SetProperty(ref _devices, value); }
        }


        private DelegateCommand<ItemsControl> _cameraContainerLoadCmd;
        public DelegateCommand<ItemsControl> CameraContainerLoadCmd =>
            _cameraContainerLoadCmd ?? (_cameraContainerLoadCmd = new DelegateCommand<ItemsControl>(ExecuteCameraContainerLoadCmd));

        void ExecuteCameraContainerLoadCmd(ItemsControl parameter)
        {
            var worker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };

            _setupEvent.Publish(_setupProgressModel);

            worker.DoWork += (_, e) =>
            {
                worker.ReportProgress(0, "初始化设备...");

                var watchhouseList = db.Watchhouses.Include(p => p.Passageways)
                    .Select(p => new
                    {
                        p.Id,
                        p.Name,
                        Passageways = p.Passageways.Where(q => q.CountCarEnable)
                        .OrderBy(q => q.Code).Select(q => new
                        {
                            q.Id,
                            q.Name,
                            q.Direction
                        })
                    }).Where(p => p.Passageways.Any()).ToList();


                var watchhouseIds = watchhouseList.Select(p => p.Id).ToList();

                var list = db.Devices.Where(p => watchhouseIds.Contains(p.WatchhouseId) 
                && p.Type == DeviceType.监控相机_海康)
                    .AsNoTracking().OrderBy(p => p.Type).ToList();

                //if (list.Any(p => p.Type == DeviceType.车牌识别相机_臻识))
                //{
                //    VzClientSDK.VzLPRClient_Setup();
                //}

                var matchedDeviceList = new List<DeviceInfo>();

                foreach (var wh in watchhouseList)
                {
                    foreach (var pw in wh.Passageways)
                    {
                        var device = list
                        .FirstOrDefault(p => p.WatchhouseId == wh.Id && p.PassagewayId == pw.Id);
                        if (device == null)
                        {
                            continue;
                        }
                        worker.ReportProgress(0, $"加载 {wh.Name}>{pw.Name}...");
                        var df = (DeviceInfo)device;
                        df.Direction = pw.Direction;
                        df.WatchhouseId = wh.Id;
                        df.WatchhouseName = wh.Name;
                        df.PassagewayId = pw.Id;
                        df.PassagewayName = pw.Name;

                        matchedDeviceList.Add(df);

                    }
                }

                var count = matchedDeviceList.Count;
                var height = parameter.ActualHeight;
                var width = parameter.ActualWidth;

                var leftHeight = 0d;
                var leftWidth = 0d;

                var sizeArray = new List<Size>();

                if (count >= 1 && count <= 4)
                {
                    if (count == 1)
                    {
                        leftHeight = height;
                        leftWidth = width;
                    }
                    else if (count == 2)
                    {
                        leftHeight = width;
                        leftWidth = width /= 2;
                    }
                    else
                    {
                        leftHeight = height /= 2;
                        leftWidth = width /= 2;
                    }

                }
                else
                {
                    var v = count % 2 == 0 ? count : Math.Ceiling((double)count + 0.1);
                    height /= (v / 2);
                    width /= 2;
                }

                for (int i = 0; i < count; i++)
                {
                    var item = matchedDeviceList.ElementAt(i);

                    item.RenderWidth = width - 8; // 这个 8=4*2 是 Margin
                    item.RenderHeight = height - 8;
                    item.EventAggregator = _eventAggregator;
                    item.Logger = _logger;
                }

                e.Result = matchedDeviceList;

            };
            worker.ProgressChanged += (_, e) =>
            {
                _setupProgressModel.ProgressText = e.UserState?.ToString();
                _setupEvent.Publish(_setupProgressModel);
            };
            worker.RunWorkerCompleted += async (_, e) =>
            {
                if (e.Error != null)
                {
                    _setupProgressModel.ProgressText = $"设备加载失败:{e.Error.Message}";
                    _setupEvent.Publish(_setupProgressModel);
                    await Task.Delay(2000);

                    _setupProgressModel.Loading = false;
                    _setupEvent.Publish(_setupProgressModel);
                    return;
                }

                _setupProgressModel.Loading = false;
                _setupEvent.Publish(_setupProgressModel);
                _deviceSource = e.Result as List<DeviceInfo>;

                DeviceList.AddRange(_deviceSource.Where(p => _displayDeviceTypes.Contains(p.Type)));

                EmptyInfoVisibility = DeviceList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            };

            worker.RunWorkerAsync();
        }

        private DeviceSetupProgressModel _setupProgressModel;

        private DbService db;
        private DeviceSetupProgressEvent _setupEvent;
        private IEventAggregator _eventAggregator;
        private AJLog4NetLogger _logger;


        public DeviceScreenViewModel(DbService dbIns, IEventAggregator eventAggregator,
            AJLog4NetLogger logger)
        {
            db = dbIns;
            _logger = logger;
            DeviceList = new ObservableCollection<DeviceInfo>();
            _setupProgressModel = new DeviceSetupProgressModel
            {
                Loading = true,
            };
            _eventAggregator = eventAggregator;
            _setupEvent = eventAggregator.GetEvent<DeviceSetupProgressEvent>();
            eventAggregator.GetEvent<ApplicationExitEvent>().Subscribe(() =>
            {
                foreach (DeviceInfo deviceInfo in DeviceList)
                {
                    try
                    {
                        deviceInfo.Close();
                    }
                    catch (Exception)
                    {

                    }
                }

                //if (DeviceList.Any(p => p.Type == DeviceType.车牌识别相机_臻识))
                //{
                //    VzClientSDK.VzLPRClient_Cleanup();
                //}
            });
        }
    }

    public class DeviceTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CCTVCamera { get; set; }
        
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
            {
                return null;
            }
            if (item is not DeviceInfo device)
            {
                return null;
            }

            return CCTVCamera;
        }
    }
}
