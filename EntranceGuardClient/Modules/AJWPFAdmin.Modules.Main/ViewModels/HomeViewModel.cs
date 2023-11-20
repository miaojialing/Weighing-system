using AJWPFAdmin.Core;
using AJWPFAdmin.Core.Enums;
using AJWPFAdmin.Core.GlobalEvents;
using AJWPFAdmin.Core.HardwareSDKS;
using AJWPFAdmin.Core.HardwareSDKS.Models;
using AJWPFAdmin.Core.Mvvm;
using AJWPFAdmin.Core.Utils;
using AJWPFAdmin.Modules.Main.Views;
using AJWPFAdmin.Services.EF;
using AJWPFAdmin.Services.EF.Tables;
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
using System.Drawing;
using System.Formats.Asn1;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using static AJWPFAdmin.Core.ExceptionTool;

namespace AJWPFAdmin.Modules.Main.ViewModels
{
    public class HomeViewModel : ViewModelBase, INavigationAware
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

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _regionMgr.RegisterViewWithRegion(nameof(DeviceScreen), typeof(DeviceScreen))
            .RegisterViewWithRegion(nameof(CarIdentificationPanel), typeof(CarIdentificationPanel))
            .RegisterViewWithRegion(nameof(ShippingRecordScrollViewer), typeof(ShippingRecordScrollViewer));
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
    }
}
