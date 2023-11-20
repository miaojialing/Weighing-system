using Employee.Modules.Header.Views;
using Employee.Core;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Employee.Modules.Header
{
    public class HeaderModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.Header, typeof(Views.Header));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}