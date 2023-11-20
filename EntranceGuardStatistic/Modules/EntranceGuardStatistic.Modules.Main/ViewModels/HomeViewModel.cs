using AJWPFAdmin.Core.Mvvm;
using AJWPFAdmin.Services.EF;
using EntranceGuardStatistic.Modules.Main.Views;
using Prism.Events;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AJWPFAdmin.Core.ExceptionTool;

namespace EntranceGuardStatistic.Modules.Main.ViewModels
{
    public class HomeViewModel :ViewModelBase, INavigationAware
    {
        private bool _loading;
        public bool Loading
        {
            get { return _loading; }
            set { SetProperty(ref _loading, value); }
        }

        private string _deviceSetupProgressText;
        public string DeviceSetupProgressText
        {
            get { return _deviceSetupProgressText; }
            set { SetProperty(ref _deviceSetupProgressText, value); }
        }

        private IDialogService _dialogSvc;
        private IEventAggregator _eventAggregator;
        private DbService db;
        private IRegionManager _regionMgr;

        public HomeViewModel(DbService dbIns, IEventAggregator eventAggregator, IDialogService dialogSvc, IRegionManager region)
        {
            db = dbIns;
            _regionMgr = region;
            _dialogSvc = dialogSvc;
            _eventAggregator = eventAggregator;
            eventAggregator.GetEvent<AJGlobalExceptionResolvedEvent>().Subscribe(() => Loading = false);

            eventAggregator.GetEvent<DeviceScreenViewModel.DeviceSetupProgressEvent>().Subscribe((p) =>
            {
                Loading = p.Loading;
                DeviceSetupProgressText = p.ProgressText;
            });
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
           
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _regionMgr.RegisterViewWithRegion(nameof(DeviceScreen), typeof(DeviceScreen))
            .RegisterViewWithRegion(nameof(StatisticPanel), typeof(StatisticPanel));
        }
    }
}
