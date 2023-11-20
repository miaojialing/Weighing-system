using AJWPFAdmin.Core.Components.AJTable.ViewModels;
using AJWPFAdmin.Core.Enums;
using AJWPFAdmin.Core.Utils;
using AJWPFAdmin.Services.EF.Tables;
using AJWPFAdmin.Services.EF;
using MaterialDesignExtensions.Controls;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AJWPFAdmin.Core.Components.AJTable.Views.AJTable;
using static AJWPFAdmin.Core.ExceptionTool;
using System.Windows.Controls;
using Masuit.Tools.Models;
using EntranceGuardManager.Modules.Main.Views;
using Microsoft.EntityFrameworkCore;

namespace EntranceGuardManager.Modules.Main.ViewModels
{

    public class DeviceListViewModel : BindableBase, INavigationAware
    {

        private List<Device> _rows;
        public List<Device> Rows
        {
            get { return _rows; }
            set { SetProperty(ref _rows, value); }
        }

        private List<AJTableColumnItem> _columns;

        public List<AJTableColumnItem> Columns
        {
            get { return _columns; }
            set { SetProperty(ref _columns, value); }
        }

        private bool _loading;

        public bool Loading
        {
            get { return _loading; }
            set { SetProperty(ref _loading, value); }
        }

        private AJTablePagination _pagination;

        public AJTablePagination Pagination
        {
            get { return _pagination; }
            set { SetProperty(ref _pagination, value); }
        }

        private AJTableSearchFormConfig _formConfig;

        public AJTableSearchFormConfig FormConfig
        {
            get { return _formConfig; }
            set { SetProperty(ref _formConfig, value); }
        }

        private AJExportProgress _exportProgress;
        public AJExportProgress ExportProgress
        {
            get { return _exportProgress; }
            set { SetProperty(ref _exportProgress, value); }
        }

        private DelegateCommand<bool?> _searchCmd;
        public DelegateCommand<bool?> SearchCmd =>
            _searchCmd ?? (_searchCmd = new DelegateCommand<bool?>(ExecuteSearchCmd));

        void ExecuteSearchCmd(bool? isRefresh)
        {
            if (Pagination.Current != 1)
            {
                Pagination.Current = 1;
            }

            ExecuteGetListCmd();
        }

        private DelegateCommand<DataGridRow> _loadingRowCmd;
        public DelegateCommand<DataGridRow> LoadingRowCmd =>
            _loadingRowCmd ?? (_loadingRowCmd = new DelegateCommand<DataGridRow>(ExecuteLoadingRowCmd));

        void ExecuteLoadingRowCmd(DataGridRow row)
        {

        }

        private DelegateCommand<Device> _editCmd;
        public DelegateCommand<Device> EditCmd =>
            _editCmd ?? (_editCmd = new DelegateCommand<Device>(ExecuteEditCmd));

        void ExecuteEditCmd(Device parameter)
        {
            var @params = new DialogParameters
            {
                { "data", parameter },
                { "watchhouseOptions", _watchHouseOptions },
                { "passagewayOptions", _passagewayOptions },
            };
            _dialogSvc.ShowDialog(nameof(DeviceForm), @params, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    ExecuteSearchCmd(true);
                }
            });
        }


        private DelegateCommand<Device> _deleteCmd;
        public DelegateCommand<Device> DeleteCmd =>
            _deleteCmd ?? (_deleteCmd = new DelegateCommand<Device>(ExecuteDeleteCmd));

        async void ExecuteDeleteCmd(Device parameter)
        {
            var confirm = await CommonUtil.ShowConfirmDialogAsync(new ConfirmationDialogArguments
            {
                Title = "删除确认",
                Message = $"即将删除 {parameter.Type} ?"
            });

            if (confirm)
            {
                db.Devices.Remove(parameter);
                await db.SaveChangesAsync();

                ExecuteGetListCmd();
            }
        }

        private DelegateCommand<AJTablePageChangedEventArgs> _pageChangedCmd;
        public DelegateCommand<AJTablePageChangedEventArgs> PageChangedCmd =>
            _pageChangedCmd ?? (_pageChangedCmd = new DelegateCommand<AJTablePageChangedEventArgs>(ExecutePageChangedCmd));

        void ExecutePageChangedCmd(AJTablePageChangedEventArgs parameter)
        {
            ExecuteGetListCmd();
        }

        private DelegateCommand _getPagedListCmd;
        public DelegateCommand GetPagedListCmd =>
            _getPagedListCmd ?? (_getPagedListCmd = new DelegateCommand(ExecuteGetListCmd));

        async void ExecuteGetListCmd()
        {
            Loading = true;

            var typeFormSchema = FormConfig.Schemas.FirstOrDefault(p => p.Field == nameof(Device.Type));

            var type = typeFormSchema.Value == null ? new short?() 
                : (short)((DeviceType)typeFormSchema.Value);

            var houseFormSchema = FormConfig.Schemas
                .FirstOrDefault(p => p.Field == nameof(Device.WatchhouseId));

            var houseId = houseFormSchema.Value == null ? new long?() : (long)houseFormSchema.Value;

            var passagewayFormSchema = FormConfig.Schemas
                .FirstOrDefault(p => p.Field == nameof(Device.PassagewayId));

            var passagewayId = passagewayFormSchema.Value == null 
                ? new long?() : (long)passagewayFormSchema.Value;

            var dates = FormConfig.Schemas
                .FirstOrDefault(p => p.Field == nameof(Device.CreateDate)).Value as ObservableCollection<DateTime?>;

            var list = await db.Devices
                .FilterEnum(type, p => p.Type)
                .IfWhere(() => houseId.HasValue, p => p.WatchhouseId == houseId)
                .IfWhere(() => passagewayId.HasValue, p => p.PassagewayId == passagewayId)
                .BetweenInDates(dates.ToArray(), p => p.CreateDate)
                .OrderByDescending(p => p.CreateDate)
                .ToPagedListAsync(Pagination.Current, Pagination.PageSize);

            Pagination.Total = list.TotalCount;

            if ((WatchHouseOptions?.Count).GetValueOrDefault() == 0)
            {
                var watchhouseOptions = await db.Watchhouses.AsNoTracking()
                .Select(p => new AJTableFormSchemaItemOptions
                {
                    Value = p.Id,
                    Label = p.Name
                }).ToListAsync();

                WatchHouseOptions = houseFormSchema.Options
                = new ObservableCollection<AJTableFormSchemaItemOptions>(watchhouseOptions);
            }

            if ((PassagewayOptions?.Count).GetValueOrDefault() == 0)
            {
                var passagewayOptions = await db.Passageways.AsNoTracking()
                .Select(p => new AJTableFormSchemaItemOptions
                {
                    Value = p.Id,
                    Label = p.Name,
                    OtherData = p.WatchhouseId
                }).ToListAsync();

                PassagewayOptions = passagewayFormSchema.Options
                    = new ObservableCollection<AJTableFormSchemaItemOptions>(passagewayOptions);
            }
            

            foreach (var item in list.Data)
            {
                item.WatchhouseName = PassagewayOptions
                    .FirstOrDefault(p => (long)p.Value == item.WatchhouseId)?.Label ?? "--";
                item.PassagewayName = PassagewayOptions
                    .FirstOrDefault(p => (long)p.Value == item.PassagewayId)?.Label ?? "--";
            }

            Rows = list.Data;

            Loading = false;
        }

        private ObservableCollection<AJTableFormSchemaItemOptions> _watchHouseOptions;
        public ObservableCollection<AJTableFormSchemaItemOptions> WatchHouseOptions
        {
            get { return _watchHouseOptions; }
            set
            {
                SetProperty(ref _watchHouseOptions, value);
            }
        }

        private ObservableCollection<AJTableFormSchemaItemOptions> _passagewayOptions;
        public ObservableCollection<AJTableFormSchemaItemOptions> PassagewayOptions
        {
            get { return _passagewayOptions; }
            set
            {
                SetProperty(ref _passagewayOptions, value);
            }
        }

        private IDialogService _dialogSvc;
        private DbService db;


        public DeviceListViewModel(DbService dbIns, IEventAggregator eventAggregator, IDialogService dialogSvc)
        {

            _dialogSvc = dialogSvc;
            eventAggregator.GetEvent<AJGlobalExceptionResolvedEvent>().Subscribe(() => Loading = false);


            FormConfig = new AJTableSearchFormConfig
            {
                Schemas = new ObservableCollection<AJTableSearchFormSchema>
                {
                    new AJTableSearchFormSchema
                    {
                        Label = "设备类型",
                        Field = nameof(Device.Type),
                        Type = AJTableSchemaType.Input,
                        IsPopular = true,
                        Placeholder = "搜索"
                    },
                    new AJTableSearchFormSchema
                    {
                        Label = "岗亭",
                        Field = nameof(Device.WatchhouseId),
                        Type = AJTableSchemaType.Select,
                        IsPopular = true,
                        Placeholder = "筛选岗亭"
                    },
                    new AJTableSearchFormSchema
                    {
                        Label = "通道",
                        Field = nameof(Device.PassagewayId),
                        Type = AJTableSchemaType.Select,
                        IsPopular = true,
                        Placeholder = "筛选通道"
                    },
                    new AJTableSearchFormSchema
                    {
                        Labels = new string[]{ "创建开始日期","创建结束日期" },
                        Field = nameof(Device.CreateDate),
                        Type = AJTableSchemaType.RangePicker,
                        Value = new ObservableCollection<DateTime?>( new DateTime?[2]{null,null}),
                        IsPopular = true,
                    },
                },
                AdvFilterVisibility = System.Windows.Visibility.Collapsed,
                ExportVisibility = System.Windows.Visibility.Collapsed,
            };

            Columns = new List<AJTableColumnItem>
            {
                new AJTableColumnItem
                {
                    Title = "操作",
                    CustomTemplate = new AJTableCustomTemplate
                    {
                        Key = "TableOperatioin"
                    }
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(Device.Type),
                    Title = "类型",
                    Formatter = new AJTableColumnFormatter
                    {
                        Handler = (val) =>
                        {
                            return ((DeviceType)val).ToString();
                        }
                    }
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(Device.WatchhouseName),
                    Title = "所属岗亭",
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(Device.PassagewayName),
                    Title = "所属通道",
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(Device.IP),
                    Title = "IP地址"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(Device.Port),
                    Title = "端口号"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(Device.SerialPort),
                    Title = "串口号",
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(Device.LoginName),
                    Title = "登录账户"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(Device.LoginPassword),
                    Title = "登录密码"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(Device.CreateDate),
                    Title = "创建日期",
                    Formatter = new AJTableColumnFormatter
                    {
                        Handler = (val) =>
                        {
                            return ((DateTime)val).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }
                }
            };
            Pagination = new AJTablePagination();
            db = dbIns;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            ExecuteGetListCmd();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
    }
}
