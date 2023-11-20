using AJWPFAdmin.Core;
using AJWPFAdmin.Modules.Main.Views;
using AJWPFAdmin.Modules.Main.ViewModels;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using AJWPFAdmin.Core.Components.AJFilePicker.Views;
using AJWPFAdmin.Core.Components.AJFilePicker.ViewModels;
using AJWPFAdmin.Core.Mvvm;

namespace AJWPFAdmin.Modules.Main
{
    public class MainModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Home>();
            containerRegistry.RegisterForNavigation<PassagewayStatistic>();
            //containerRegistry.RegisterForNavigation<MaterialDictionaryList>();
            //containerRegistry.RegisterForNavigation<ShippingRecordList>();
            //containerRegistry.RegisterForNavigation<DatabaseConfig>();
            containerRegistry.RegisterDialog<ShippingRecordDetial, ShippingRecordDetailViewModel>();
            containerRegistry.RegisterDialog<ImagePreviewer, ImagePreviewerViewModel>();
        }
    }
}