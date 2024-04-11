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

    public class PassagewayListViewModel : BindableBase, INavigationAware
    {

        private List<Passageway> _rows;
        public List<Passageway> Rows
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

        private DelegateCommand<Passageway> _editCmd;
        public DelegateCommand<Passageway> EditCmd =>
            _editCmd ?? (_editCmd = new DelegateCommand<Passageway>(ExecuteEditCmd));

        void ExecuteEditCmd(Passageway parameter)
        {
            var @params = new DialogParameters
            {
                { "data", parameter },
                { "watchhouseOptions", _watchHouseOptions },
            };
            _dialogSvc.ShowDialog(nameof(PassagewayForm), @params, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    ExecuteSearchCmd(true);
                }
            });
        }


        private DelegateCommand<Passageway> _deleteCmd;
        public DelegateCommand<Passageway> DeleteCmd =>
            _deleteCmd ?? (_deleteCmd = new DelegateCommand<Passageway>(ExecuteDeleteCmd));

        async void ExecuteDeleteCmd(Passageway parameter)
        {
            var confirm = await CommonUtil.ShowConfirmDialogAsync(new ConfirmationDialogArguments
            {
                Title = "删除确认",
                Message = $"即将删除 {parameter.Name} ?"
            });

            if (confirm)
            {
                db.Passageways.Remove(parameter);
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
                .FirstOrDefault(p => p.Field == nameof(Passageway.Name)).Value?.ToString();

            var houseFormSchema = FormConfig.Schemas
                .FirstOrDefault(p => p.Field == nameof(Passageway.WatchhouseId));

            var houseId = houseFormSchema.Value == null ? new long?() : (long)houseFormSchema.Value;

            var dates = FormConfig.Schemas
                .FirstOrDefault(p => p.Field == nameof(Passageway.CreateDate)).Value as ObservableCollection<DateTime?>;

            var list = await db.Passageways
                .LikeOrLike(name, p => p.Name, p => p.Code)
                .IfWhere(() => houseId.HasValue, p => p.WatchhouseId == houseId)
                .BetweenInDates(dates.ToArray(), p => p.CreateDate)
                .OrderByDescending(p => p.CreateDate)
                .ToPagedListAsync(Pagination.Current, Pagination.PageSize);

            Pagination.Total = list.TotalCount;

            if ((WatchHouseOptions?.Count).GetValueOrDefault() == 0)
            {
                var options = await db.Watchhouses.AsNoTracking()
                .Select(p => new AJTableFormSchemaItemOptions
                {
                    Value = p.Id,
                    Label = p.Name
                }).ToListAsync();

                WatchHouseOptions = houseFormSchema.Options
                    = new ObservableCollection<AJTableFormSchemaItemOptions>(options);
            }
            

            foreach (var item in list.Data)
            {
                item.WatchhouseName = WatchHouseOptions
                    .FirstOrDefault(p => (long)p.Value == item.WatchhouseId)?.Label ?? "--";
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

        private IDialogService _dialogSvc;
        private DbService db;


        public PassagewayListViewModel(DbService dbIns, IEventAggregator eventAggregator, IDialogService dialogSvc)
        {

            _dialogSvc = dialogSvc;
            eventAggregator.GetEvent<AJGlobalExceptionResolvedEvent>().Subscribe(() => Loading = false);


            FormConfig = new AJTableSearchFormConfig
            {
                Schemas = new ObservableCollection<AJTableSearchFormSchema>
                {
                    new AJTableSearchFormSchema
                    {
                        Label = "名称/编号",
                        Field = nameof(Passageway.Name),
                        Type = AJTableSchemaType.Input,
                        IsPopular = true,
                        Placeholder = "搜索"
                    },
                    new AJTableSearchFormSchema
                    {
                        Label = "名称/编号",
                        Field = nameof(Passageway.WatchhouseId),
                        Type = AJTableSchemaType.Select,
                        IsPopular = true,
                        Placeholder = "筛选岗亭"
                    },
                    new AJTableSearchFormSchema
                    {
                        Labels = new string[]{ "创建开始日期","创建结束日期" },
                        Field = nameof(Passageway.CreateDate),
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
                    DataIndex = nameof(Passageway.Code),
                    Title = "编号"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(Passageway.Name),
                    Title = "名称"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(Passageway.WatchhouseName),
                    Title = "所属岗亭",
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(Passageway.WarehouseName),
                    Title = "关联仓库",
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(Passageway.Direction),
                    Title = "进出方向",
                    Formatter = new AJTableColumnFormatter
                    {
                        Handler = (val) =>
                        {
                            return ((PassagewayDirection)val).ToString();
                        }
                    }
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(Passageway.CountCarEnable),
                    Title = "统计车辆",
                    Formatter = new AJTableColumnFormatter
                    {
                        Handler = (val) =>
                        {
                            return ((bool)val) ? "是" : "否";
                        }
                    }
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(Passageway.CreateDate),
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
