using AJWPFAdmin.Core.GlobalEvents;
using AJWPFAdmin.Core.HardwareSDKS;
using AJWPFAdmin.Services;
using AJWPFAdmin.Services.EF.Tables;
using Prism.Events;
using Prism.Mvvm;
using System.Windows;

namespace EntranceGuardManager.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private SystemUser _curUser;
        public SystemUser CurrentUser
        {
            get { return _curUser; }
            set { SetProperty(ref _curUser, value); }
        }

        private bool _menuOpen;
        public bool MenuOpen
        {
            get { return _menuOpen; }
            set { SetProperty(ref _menuOpen, value); }
        }

        private SideMenuNavEvent _sideMenuNavEvent;

        public MainWindowViewModel(IEventAggregator eventAggregator, SystemUserService sysUserSvc)
        {
            MenuOpen = true;
            _curUser = sysUserSvc.CurrnetUser;
            var version = Application.ResourceAssembly.GetName().Version;
            Title = $"停车门禁管理系统-管理端 {version}";
            //_sideMenuNavEvent = eventAggregator.GetEvent<SideMenuNavEvent>();
            //_sideMenuNavEvent.Subscribe((name) =>
            //{
            //    MenuOpen = false;
            //});
        }
    }
}
