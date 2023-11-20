using AJWPFAdmin.Core;
using AJWPFAdmin.Core.Enums;
using AJWPFAdmin.Core.GlobalEvents;
using AJWPFAdmin.Core.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace AJWPFAdmin.Modules.SideMenu.ViewModels
{
    public class SideMenuViewModel : BindableBase
    {
        private bool _menuOpen;
        public bool MenuOpen
        {
            get { return _menuOpen; }
            set { SetProperty(ref _menuOpen, value); }
        }

        private ObservableCollection<SysMenu> _menus;
        public ObservableCollection<SysMenu> Menus
        {
            get { return _menus; }
            set
            {
                SetProperty(ref _menus, value);
            }
        }

        private DelegateCommand<SysMenu> _menuExpanded;
        public DelegateCommand<SysMenu> MenuExpanded =>
            _menuExpanded ?? (_menuExpanded = new DelegateCommand<SysMenu>(ExecuteMenuExpanded));

        void ExecuteMenuExpanded(SysMenu curMenu)
        {
            foreach (var menu in Menus)
            {
                menu.IsExpanded = curMenu.Name == menu.Name;
                foreach (var childMenu in menu.Children)
                {
                    childMenu.IsExpanded = curMenu.Name == childMenu.Name;
                }
            }
        }

        private DelegateCommand<object> _menuSelected;
        public DelegateCommand<object> MenuSelected =>
            _menuSelected ?? (_menuSelected = new DelegateCommand<object>(ExecuteMenuSelected));

        void ExecuteMenuSelected(object eventArgs)
        {
            SysMenu selectedMenu = null;
            var isSysMenu = eventArgs is SysMenu;
            if (isSysMenu)
            {
                selectedMenu = (SysMenu)eventArgs;
            }

            if (eventArgs is TabItem item)
            {
                selectedMenu = item.DataContext as SysMenu;
            }

            if (selectedMenu != null)
            {
                if (!string.IsNullOrWhiteSpace(selectedMenu.Region))
                {
                    var navParams = new NavigationParameters();
                    if (selectedMenu.Parameter != null)
                    {
                        navParams.Add("params", selectedMenu.Parameter);
                    }
                    _regionMgr.RequestNavigate(RegionNames.Main, selectedMenu.Region, (ret) =>
                    {
                        if (ret.Result.GetValueOrDefault())
                        {
                            _sideMenuNavEvent.Publish(selectedMenu.Name);
                        }
                    }, navParams);
                }

                OnSideMenuNavEvent(selectedMenu.Name);
            }

        }

        void OnSideMenuEvent()
        {
            MenuOpen = !MenuOpen;
        }

        void OnSideMenuNavEvent(string name)
        {
            foreach (var menu in Menus)
            {
                menu.Checked = name == menu.Name;
                foreach (var childMenu in menu.Children)
                {
                    childMenu.Checked = name == childMenu.Name;
                }
            }
        }

        private IRegionManager _regionMgr;
        //private ToggleSideMenuEvent _sideMenuEvent;
        private SideMenuNavEvent _sideMenuNavEvent;

        public SideMenuViewModel(IContainerProvider containerProvider, IEventAggregator eventAggregator)
        {
            _regionMgr = containerProvider.Resolve<IRegionManager>();
            //_sideMenuEvent = eventAggregator.GetEvent<ToggleSideMenuEvent>();
            //_sideMenuEvent.Subscribe(OnSideMenuEvent);
            _sideMenuNavEvent = eventAggregator.GetEvent<SideMenuNavEvent>();

            _sideMenuNavEvent.Subscribe(OnSideMenuNavEvent);

            Menus = new ObservableCollection<SysMenu>
            {
                new SysMenu
                {
                    Name = "岗亭收费",
                    Icon = "Cctv",
                    Checked = true,
                    Region = "Home"
                },
                new SysMenu
                {
                    Name = "车辆统计",
                    Icon = "Warehouse",
                    Region = "PassagewayStatistic"
                },
            };
            var firstMenu = Menus.FirstOrDefault();
            _regionMgr.RequestNavigate(RegionNames.Main, firstMenu.Region);
            //_sideMenuNavEvent.Publish(firstMenu.Name);
        }


    }

    
}
