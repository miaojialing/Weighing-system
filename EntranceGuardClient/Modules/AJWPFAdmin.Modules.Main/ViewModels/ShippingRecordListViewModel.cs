using AJWPFAdmin.Core.Components.AJTable.ViewModels;
using AJWPFAdmin.Core.Utils;
using AJWPFAdmin.Services.EF;
using MaterialDesignExtensions.Controls;
using Microsoft.EntityFrameworkCore;
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
using System.Windows.Media;
using AJWPFAdmin.Services.EF.Tables;
using Masuit.Tools.Models;
using AJWPFAdmin.Core.Enums;
using AJWPFAdmin.Modules.Main.Views;
using Prism.Ioc;
using AJWPFAdmin.Core;
using AJWPFAdmin.Core.GlobalEvents;

namespace AJWPFAdmin.Modules.Main.ViewModels
{
    public class ShippingRecordListViewModel : BindableBase, INavigationAware
    {

        private List<ShippingRecord> _rows;
        public List<ShippingRecord> Rows
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

        private DelegateCommand<ShippingRecord> _editCmd;
        public DelegateCommand<ShippingRecord> EditCmd =>
            _editCmd ?? (_editCmd = new DelegateCommand<ShippingRecord>(ExecuteEditCmd));

        void ExecuteEditCmd(ShippingRecord parameter)
        {
            var @params = new NavigationParameters
            {
                { "data", parameter },
                { "from",  nameof(ShippingRecordList)}
            };
            _regionMgr.RequestNavigate(RegionNames.Main, nameof(ShippingRecordForm), @params);

        }


        private DelegateCommand<ShippingRecord> _deleteCmd;
        public DelegateCommand<ShippingRecord> DeleteCmd =>
            _deleteCmd ?? (_deleteCmd = new DelegateCommand<ShippingRecord>(ExecuteDeleteCmd));

        async void ExecuteDeleteCmd(ShippingRecord parameter)
        {
            var confirm = await CommonUtil.ShowConfirmDialogAsync(new ConfirmationDialogArguments
            {
                Title = "删除确认",
                Message = $"即将删除 {parameter.CarNo} ?"
            });

            if (confirm)
            {
                db.ShippingRecords.Remove(parameter);
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


            var name = FormConfig.Schemas
                .FirstOrDefault(p => p.Field == nameof(ShippingRecord.CarNo)).Value?.ToString();

            var dates = FormConfig.Schemas
                .FirstOrDefault(p => p.Field == nameof(ShippingRecord.ArriveDate)).Value as ObservableCollection<DateTime?>;

            var list = await db.ShippingRecords.LikeOrLike(name, p => p.CarNo)
                .BetweenInDates(dates.ToArray(), p => p.CreateDate)
                .OrderByDescending(p => p.CreateDate)
                .ToPagedListAsync(Pagination.Current, Pagination.PageSize);

            Pagination.Total = list.TotalCount;
            Rows = list.Data;
            Loading = false;
        }

        private IDialogService _dialogSvc;
        private DbService db;
        private IRegionManager _regionMgr;
        private SideMenuNavEvent _sideMenuNavEvent;

        public ShippingRecordListViewModel(DbService dbIns,
            IEventAggregator eventAggregator,
            IDialogService dialogSvc,
            IContainerProvider containerProvider)
        {
            _regionMgr = containerProvider.Resolve<IRegionManager>();
            _dialogSvc = dialogSvc;
            eventAggregator.GetEvent<AJGlobalExceptionResolvedEvent>().Subscribe(() => Loading = false);
            _sideMenuNavEvent = eventAggregator.GetEvent<SideMenuNavEvent>();

            FormConfig = new AJTableSearchFormConfig
            {
                Schemas = new ObservableCollection<AJTableSearchFormSchema>
                {
                    new AJTableSearchFormSchema
                    {
                        Label = "船号",
                        Field = nameof(ShippingRecord.CarNo),
                        Type = AJTableSchemaType.Input,
                        IsPopular = true,
                        Placeholder = "船号"
                    },
                    new AJTableSearchFormSchema
                    {
                        Labels = new string[]{ "到港开始日期","到港结束日期" },
                        Field = nameof(ShippingRecord.ArriveDate),
                        Type = AJTableSchemaType.RangePicker,
                        Value = new ObservableCollection<DateTime?>( new DateTime?[2]{null,null}),
                        IsPopular = true,
                    },
                },
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
                    DataIndex = nameof(ShippingRecord.CarNo),
                    Title = "船号"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(ShippingRecord.MaterialCategory),
                    Title = "物料种类"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(ShippingRecord.MaterialName),
                    Title = "物料名称"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(ShippingRecord.ArriveDate),
                    Title = "到港时间",
                    Formatter = new AJTableColumnFormatter
                    {
                        Handler = (val) =>
                        {
                            return ((DateTime)val).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(ShippingRecord.ShipStartDate),
                    Title = "装/卸货时间",
                    Formatter = new AJTableColumnFormatter
                    {
                        Handler = (val) =>
                        {
                            var date = (DateTime?)val;
                            return date  == null ? "--" : date.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(ShippingRecord.ShipEndDate),
                    Title = "结束时间",
                    Formatter = new AJTableColumnFormatter
                    {
                        Handler = (val) =>
                        {
                            var date = (DateTime?)val;
                            return date  == null ? "--" : date.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(ShippingRecord.OrderNetWeight),
                    Title = "船运单净重"
                },
                //new AJTableColumnItem
                //{
                //    DataIndex = nameof(ShippingRecord.JianJin),
                //    Title = "检斤"
                //},
                new AJTableColumnItem
                {
                    DataIndex = nameof(ShippingRecord.CarNetWeight),
                    Title = "汽车衡净重"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(ShippingRecord.SenderName),
                    Title = "发货单位"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(ShippingRecord.ReceiverName),
                    Title = "收货单位"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(ShippingRecord.Berth),
                    Title = "装/卸货地点泊位"
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
