using AJWPFAdmin.Core;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace AJWPFAdmin.Modules.SideMenu
{
    public class SideMenuModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.SideMenu, typeof(Views.SideMenu));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}