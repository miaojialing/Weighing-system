using AJWPFAdmin.Core.Components.CommonLoading.Views;
using AJWPFAdmin.Core.GlobalEvents;
using AJWPFAdmin.Core.Models;
using AJWPFAdmin.Core.Mvvm;
using AJWPFAdmin.Core.Utils;
using AJWPFAdmin.Services;
using AJWPFAdmin.Services.EF;
using DocumentFormat.OpenXml.Wordprocessing;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using MQTTnet;
using MQTTnet.Client;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
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

        private DbService db;
        private IEventAggregator _eventAggregator;
        private AJMQTTService _aJMQTTService;

        public StatisticPanelViewModel(DbService dbIns, IEventAggregator eventAggregator, 
            AJMQTTService aJMQTTService)
        {
            db = dbIns;
            _eventAggregator = eventAggregator;
            Rows = new ObservableCollection<WarehousePassageRecordsGroupRecord>();
            EmptyInfoVisible = Visibility.Visible;
            eventAggregator.GetEvent<DeviceSetupProgressEvent>().Subscribe((progress) =>
            {
                if (progress.Loading)
                {
                    return;
                }

                LoadDataAsync();
            });

            _eventAggregator.GetEvent<ApplicationExitEvent>().Subscribe(async () =>
            {
                await _aJMQTTService.CloseAsync();
            });

            _aJWebSvcConfig = new AJWebSvcConfig();
            // 暂时写死使用初始化默认值
            _aJWebSvcConfig.Init();
            _aJMQTTService = aJMQTTService;
            _eventAggregator.GetEvent<AJMQTTSvcStatusChangedEvent>().Subscribe(OnAJMQTTSvcStatusChanged);
            Task.Run(ConnectToMQTTSvc);
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
                _aJMQTTService.SubscribeAsync(AJMQTTService.CARPASSEDTOPIC, OnCarPassedMessageReceived);
            }
        }

        private Task OnCarPassedMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            // e.ApplicationMessage.ConvertPayloadToString()
            System.Diagnostics.Trace.WriteLine("OnCarPassedMessageReceived");
            e.IsHandled = true;
            _ctrl.Dispatcher.Invoke(LoadDataAsync);
            return Task.CompletedTask;
        }
    }
}
