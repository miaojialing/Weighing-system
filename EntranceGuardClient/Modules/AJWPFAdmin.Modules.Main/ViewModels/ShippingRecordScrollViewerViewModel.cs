using AJWPFAdmin.Core.Mvvm;
using AJWPFAdmin.Core.Utils;
using AJWPFAdmin.Modules.Main.Views;
using AJWPFAdmin.Services.EF;
using AJWPFAdmin.Services.EF.Tables;
using DnsClient.Protocol;
using Masuit.Tools.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using static AJWPFAdmin.Modules.Main.ViewModels.DeviceScreenViewModel;

namespace AJWPFAdmin.Modules.Main.ViewModels
{
    public class ShippingRecordScrollViewerViewModel : ViewModelBase
    {

        private UserControl _userControl;

        private bool _loading = true;
        public bool Loading
        {
            get { return _loading; }
            set { SetProperty(ref _loading, value); }
        }

        private Visibility _emptyInfoVisibility;
        public Visibility EmptyInfoVisibility
        {
            get { return _emptyInfoVisibility; }
            set { SetProperty(ref _emptyInfoVisibility, value); }
        }

        private string _search;
        public string Search
        {
            get { return _search; }
            set
            {
                if (SetProperty(ref _search, value))
                {
                    _debounce.Trigger();
                }
            }
        }

        private ObservableCollection<ShippingRecord> _records;
        public ObservableCollection<ShippingRecord> Records
        {
            get { return _records; }
            set { SetProperty(ref _records, value); }
        }

        private IDialogService _dialogSvc;
        private IEventAggregator _eventAggregator;
        private DbService db;
        private Debounce _debounce;

        public ShippingRecordScrollViewerViewModel(DbService dbIns, IDialogService dialogSvc,
            IEventAggregator eventAggregator)
        {
            EmptyInfoVisibility = Visibility.Visible;
            Records = new ObservableCollection<ShippingRecord>();
            db = dbIns;
            _dialogSvc = dialogSvc;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<CarIdentificationPanelViewModel.CarIdentificationPassedEvent>()
                .Subscribe(async () =>
                {
                    Records.Clear();
                    await GetListAsync();
                });

            _eventAggregator.GetEvent<DeviceSetupProgressEvent>().Subscribe((progress) =>
            {
                if (progress.Loading)
                {
                    return;
                }

                _debounce.Trigger();
            });

            _debounce = new Debounce(GetListFun, 1000);

        }

        private void GetListFun(params object[] args)
        {
            _userControl.Dispatcher.Invoke(async () =>
            {
                Records.Clear();
                await GetListAsync();
            });
        }

        private DelegateCommand<ShippingRecord> _openDetailDialogCmd;
        public DelegateCommand<ShippingRecord> OpenDetailDialogCmd =>
            _openDetailDialogCmd ?? (_openDetailDialogCmd = new DelegateCommand<ShippingRecord>(ExecuteOpenDetailDialogCmd));

        void ExecuteOpenDetailDialogCmd(ShippingRecord parameter)
        {
            var @params = new DialogParameters
            {
                { "data", parameter }
            };
            _dialogSvc.ShowDialog(nameof(ShippingRecordDetial), @params, r =>
            {
                //if (r.Result == ButtonResult.OK)
                //{
                //    ExecuteSearchCmd(true);
                //}
            });
        }

        private DelegateCommand<UserControl> _controlLoadedCmd;
        public DelegateCommand<UserControl> ControlLoadedCmd =>
            _controlLoadedCmd ?? (_controlLoadedCmd = new DelegateCommand<UserControl>(ExecuteControlLoadedCmd));

        void ExecuteControlLoadedCmd(UserControl ctrl)
        {
            _userControl = ctrl;
        }

        private async Task GetListAsync()
        {
            Loading = true;

            var records = await db.ShippingRecords.LikeOrLike(Search, p => p.CarNo)
                .OrderByDescending(p => p.Id).ToPagedListAsync(1, 50);

            EmptyInfoVisibility = records.Data.Count > 0 ? Visibility.Collapsed : Visibility.Visible;

            Records.AddRange(records.Data);

            Loading = false;
        }

    }
}
