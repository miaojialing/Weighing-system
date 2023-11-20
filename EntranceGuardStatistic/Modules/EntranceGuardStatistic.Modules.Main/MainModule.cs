using AJWPFAdmin.Core;
using AJWPFAdmin.Core.Components.AJFilePicker.ViewModels;
using AJWPFAdmin.Core.Components.AJFilePicker.Views;
using EntranceGuardStatistic.Modules.Main.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace EntranceGuardStatistic.Modules.Main
{
    public class MainModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Home>();
            //containerRegistry.RegisterDialog<ImagePreviewer, ImagePreviewerViewModel>();
        }
    }
}