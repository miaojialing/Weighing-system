using Employee.Core.GlobalEvents;
using Employee.Services;
using Employee.Services.EF.Tables;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Modules.Header.ViewModels
{
    public class HeaderViewModel : BindableBase
    {
        private SystemUser _curUser;

        public SystemUser CurrentUser
        {
            get { return _curUser; }
            set { SetProperty(ref _curUser, value); }
        }

        private string _menuName;

        public string MenuName
        {
            get { return _menuName; }
            set { SetProperty(ref _menuName, value); }
        }

        private DelegateCommand _toggleMenu;
        public DelegateCommand ToggleMenu =>
            _toggleMenu ?? (_toggleMenu = new DelegateCommand(ExecuteToggleMenu));

        void ExecuteToggleMenu()
        {
            _sideMenuEvent.Publish();
        }

        private ToggleSideMenuEvent _sideMenuEvent;

        void OnSideMenuNav(string menuName)
        {
            MenuName = menuName;
        }

        public HeaderViewModel(IEventAggregator eventAggregator, SystemUserService sysUserSvc)
        {
            _curUser = sysUserSvc.CurrnetUser;
            _sideMenuEvent = eventAggregator.GetEvent<ToggleSideMenuEvent>();
            eventAggregator.GetEvent<SideMenuNavEvent>().Subscribe(OnSideMenuNav);
        }
    }
}
