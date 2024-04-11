using AJWPFAdmin.Core;
using AJWPFAdmin.Core.Components.AJFilePicker.ViewModels;
using AJWPFAdmin.Core.Components.AJFilePicker.Views;
using EntranceGuardManager.Modules.Main.ViewModels;
using EntranceGuardManager.Modules.Main.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace EntranceGuardManager.Modules.Main
{
    public class MainModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.Main, typeof(Views.WatchhouseList));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<WatchhouseList>();
            containerRegistry.RegisterDialog<WatchhouseForm, WatchhouseFormViewModel>();

            containerRegistry.RegisterForNavigation<PassagewayList>();
            containerRegistry.RegisterDialog<PassagewayForm, PassagewayFormViewModel>();

            containerRegistry.RegisterForNavigation<DeviceList>();
            containerRegistry.RegisterDialog<DeviceForm, DeviceFormViewModel>();

            containerRegistry.RegisterForNavigation<CarTypeList>();
            containerRegistry.RegisterDialog<CarTypeForm, CarTypeFormViewModel>();

            containerRegistry.RegisterForNavigation<CarList>();
            containerRegistry.RegisterDialog<CarForm, CarFormViewModel>();

            containerRegistry.RegisterForNavigation<ShippingRecordList>();
            containerRegistry.RegisterDialog<ShippingRecordForm, ShippingRecordFormViewModel>();

            containerRegistry.RegisterForNavigation<SystemUserList>();
            containerRegistry.RegisterDialog<SystemUserForm, SystemUserFormViewModel>();

            containerRegistry.RegisterForNavigation<SystemRoleList>();
            containerRegistry.RegisterDialog<SystemRoleForm, SystemRoleFormViewModel>();

            containerRegistry.RegisterForNavigation<WarehouseList>();
            containerRegistry.RegisterDialog<WarehouseForm, WarehouseFormViewModel>();

            containerRegistry.RegisterForNavigation<WarehousePassageRecordList>();

            containerRegistry.RegisterForNavigation<DatabaseConfig>();

            containerRegistry.RegisterForNavigation<OtherConfig>();

            containerRegistry.RegisterDialog<ImagePreviewer, ImagePreviewerViewModel>();
        }
    }
}