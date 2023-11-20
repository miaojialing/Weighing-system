using AJWPFAdmin.Modules.Common.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using AJWPFAdmin.Modules.Common.ViewModels;

namespace AJWPFAdmin.Modules.Common
{
    public class CommonModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<DatabaseConfigDialog, DatabaseConfigDialogViewModel>();
        }
    }
}