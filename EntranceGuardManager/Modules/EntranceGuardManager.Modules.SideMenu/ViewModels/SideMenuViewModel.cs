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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EntranceGuardManager.Modules.SideMenu.ViewModels
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

            if (eventArgs is ListViewItem item)
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
        private ToggleSideMenuEvent _sideMenuEvent;
        private SideMenuNavEvent _sideMenuNavEvent;

        public SideMenuViewModel(IContainerProvider containerProvider, IEventAggregator eventAggregator)
        {
            _regionMgr = containerProvider.Resolve<IRegionManager>();
            _sideMenuEvent = eventAggregator.GetEvent<ToggleSideMenuEvent>();
            _sideMenuEvent.Subscribe(OnSideMenuEvent);
            _sideMenuNavEvent = eventAggregator.GetEvent<SideMenuNavEvent>();

            _sideMenuNavEvent.Subscribe(OnSideMenuNavEvent);

            Menus = new ObservableCollection<SysMenu>
            {
                new SysMenu
                {
                    Name = "基础信息管理",
                    Icon = "Home",
                    IsExpanded = true,
                    Children = new ObservableCollection<SysMenu>
                    {
                        new SysMenu
                        {
                            Name = "岗亭管理",
                            Checked = true,
                            Region = "WatchhouseList"
                        },
                        new SysMenu
                        {
                            Name = "通道管理",
                            Region = "PassagewayList"
                        },
                        new SysMenu
                        {
                            Name = "设备管理",
                            Region = "DeviceList"
                        },
                    }
                },
                new SysMenu
                {
                    Name = "车辆管理",
                    Icon = "CarMultiple",
                    Children = new ObservableCollection<SysMenu>
                    {
                        new SysMenu
                        {
                            Name = "车辆类型",
                            Region = "CarTypeList"
                        },
                        new SysMenu
                        {
                            Name = "车辆列表",
                            Region = "CarList"
                        },
                    }
                },
                new SysMenu
                {
                    Name = "记录查询",
                    Icon = "BookCogOutline",
                    Children = new ObservableCollection<SysMenu>
                    {
                        new SysMenu
                        {
                            Name = "在场记录",
                            Region = "ShippingRecordList",
                            Parameter = ShippingRecordNavParamType.在场记录
                        },
                        new SysMenu
                        {
                            Name = "进出记录",
                            Region = "ShippingRecordList",
                            Parameter = ShippingRecordNavParamType.进出记录
                        },
                    }
                },
                new SysMenu
                {
                    Name = "车辆统计",
                    Icon = "Warehouse",
                    Children = new ObservableCollection<SysMenu>
                    {
                        new SysMenu
                        {
                            Name = "仓库管理",
                            Region = "WarehouseList",
                        },
                        new SysMenu
                        {
                            Name = "统计列表",
                            Region = "WarehousePassageRecordList",
                        }
                    }
                },
                new SysMenu
                {
                    Name = "系统管理",
                    Icon = "CogOutline",
                    Children = new ObservableCollection<SysMenu>
                    {
                        new SysMenu
                        {
                            Name = "数据库配置",
                            Region = "DatabaseConfig",
                        },
                        new SysMenu
                        {
                            Name = "其他配置",
                            Region = "OtherConfig",
                        }
                    }
                },
                new SysMenu
                {
                    Name = "账号管理",
                    Icon = "AccountMultiple",
                    Children = new ObservableCollection<SysMenu>
                    {
                        new SysMenu
                        {
                            Name = "账户列表",
                            Region = "SystemUserList",
                        },
                        new SysMenu
                        {
                            Name = "角色列表",
                            Region = "SystemRoleList",
                        }
                    }
                },
            };
            //_regionMgr.RequestNavigate(RegionNames.Main, firstMenu.Region);
        }

    }

    public class MenuItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SingleMenu { get; set; }
        public DataTemplate ChildrenMenu { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
            {
                return null;
            }
            if (item is not SysMenu menu)
            {
                return null;
            }
            return menu.Children?.Count > 0 ? ChildrenMenu : SingleMenu;
        }
    }
}
