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
using AJWPFAdmin.Core.Models;
using AJWPFAdmin.Services.Models;
using MaterialDesignThemes.Wpf;

namespace EntranceGuardManager.Modules.Main.ViewModels
{

    public class WarehousePassageRecordListViewModel : BindableBase, INavigationAware
    {
        private const string DIALOGROOT = "DHMain";

        private List<WarehousePassageRecordsGroupRecord> _rows;
        public List<WarehousePassageRecordsGroupRecord> Rows
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

        private ObservableCollection<AJTableFormSchemaItemOptions> _warehouseOptions;
        public ObservableCollection<AJTableFormSchemaItemOptions> WarehouseOptions
        {
            get { return _warehouseOptions; }
            set
            {
                SetProperty(ref _warehouseOptions, value);
            }
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

        private DelegateCommand<WarehousePassageRecord> _editCmd;
        public DelegateCommand<WarehousePassageRecord> EditCmd =>
            _editCmd ?? (_editCmd = new DelegateCommand<WarehousePassageRecord>(ExecuteEditCmd));

        void ExecuteEditCmd(WarehousePassageRecord parameter)
        {
            //var @params = new DialogParameters
            //{
            //    { "data", parameter },
            //};
            //_dialogSvc.ShowDialog(nameof(WarehousePassageRecordForm), @params, r =>
            //{
            //    if (r.Result == ButtonResult.OK)
            //    {
            //        ExecuteSearchCmd(true);
            //    }
            //});
        }


        private DelegateCommand<WarehousePassageRecord> _deleteCmd;
        public DelegateCommand<WarehousePassageRecord> DeleteCmd =>
            _deleteCmd ?? (_deleteCmd = new DelegateCommand<WarehousePassageRecord>(ExecuteDeleteCmd));

        async void ExecuteDeleteCmd(WarehousePassageRecord parameter)
        {
            //var confirm = await CommonUtil.ShowConfirmDialogAsync(new ConfirmationDialogArguments
            //{
            //    Title = "删除确认",
            //    Message = $"即将删除 {parameter.Name} ?"
            //});

            //if (confirm)
            //{
            //    db.WarehousePassageRecords.Remove(parameter);
            //    await db.SaveChangesAsync();

            //    ExecuteGetListCmd();
            //}
        }

        private DelegateCommand<WarehousePassageRecordsGroupRecord> _showRecordsModal;
        public DelegateCommand<WarehousePassageRecordsGroupRecord> ShowRecordsModal =>
            _showRecordsModal ?? (_showRecordsModal = new DelegateCommand<WarehousePassageRecordsGroupRecord>(ExecuteShowRecordsModal));

        async void ExecuteShowRecordsModal(WarehousePassageRecordsGroupRecord parameter)
        {
            parameter.Records.ForEach(item =>
            {
                item.SmallImages = CommonUtil
                .TryGetJSONObject<string[]>(item.IdentifiedCaptureSmallFile) ?? Array.Empty<string>();

                item.FullImages = CommonUtil
                .TryGetJSONObject<string[]>(item.IdentifiedCaptureFullFile) ?? Array.Empty<string>();

                item.CaptureImages = CommonUtil
                .TryGetJSONObject<string[]>(item.CameraCaptureFile) ?? Array.Empty<string>();

            });
            await DialogHost.Show(new WarehousePassageRecordDetailDialog
            {
                DataContext = new WarehousePassageRecordDetailDialogViewModel(parameter, _dialogSvc)
            }, DIALOGROOT);
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
                .FirstOrDefault(p => p.Field == nameof(WarehousePassageRecordsGroupRecord.CarNo)).Value?.ToString();

            var warehouseFormCfg = FormConfig.Schemas
                .FirstOrDefault(p => p.Field == nameof(WarehousePassageRecordsGroupRecord.WarehouseId));

            var wid = warehouseFormCfg.Value as long?;

            var dates = FormConfig.Schemas
                .FirstOrDefault(p => p.Field == nameof(WarehousePassageRecord.CreateDate)).Value as ObservableCollection<DateTime?>;

            if ((WarehouseOptions?.Count).GetValueOrDefault() == 0)
            {
                var warehouseOptions = await db.Watchhouses.AsNoTracking()
                .Select(p => new AJTableFormSchemaItemOptions
                {
                    Value = p.Id,
                    Label = p.Name,
                }).ToListAsync();

                WarehouseOptions = warehouseFormCfg.Options
                    = new ObservableCollection<AJTableFormSchemaItemOptions>(warehouseOptions);
            }

            var list = await db.WarehousePassageRecords.LikeOrLike(name, p => p.CarNo)
                .IfWhere(() => wid.HasValue, p => p.WarehouseId == wid)
                .BetweenInDates(dates.ToArray(), p => p.CreateDate)
                .OrderByDescending(p => p.CreateDate)
                .GroupBy(p => new
                {
                    p.CarNo,
                    p.WatchhouseId
                })
                .Select(p => new WarehousePassageRecordsGroupRecord
                {
                    WarehouseId = p.Key.WatchhouseId,
                    WarehouseName = p.Max(q => q.WatchhouseName),
                    CarNo = p.Key.CarNo,
                    Count = p.Count(),
                    Records = p.ToList()
                })
                .ToPagedListAsync(Pagination.Current, Pagination.PageSize);

            Pagination.Total = list.TotalCount;
            Rows = list.Data;
            Loading = false;
        }

        private IDialogService _dialogSvc;
        private DbService db;


        public WarehousePassageRecordListViewModel(DbService dbIns, IEventAggregator eventAggregator, IDialogService dialogSvc)
        {

            _dialogSvc = dialogSvc;
            eventAggregator.GetEvent<AJGlobalExceptionResolvedEvent>().Subscribe(() => Loading = false);


            FormConfig = new AJTableSearchFormConfig
            {
                Schemas = new ObservableCollection<AJTableSearchFormSchema>
                {
                    new AJTableSearchFormSchema
                    {
                        Label = "搜索车牌号",
                        Field = nameof(WarehousePassageRecordsGroupRecord.CarNo),
                        Type = AJTableSchemaType.Input,
                        IsPopular = true,
                        Placeholder = "名称"
                    },
                    new AJTableSearchFormSchema
                    {
                        Label = "仓库",
                        Field = nameof(WarehousePassageRecordsGroupRecord.WarehouseId),
                        Type = AJTableSchemaType.Select,
                        IsPopular = true,
                        Placeholder = "筛选仓库",
                    },
                    new AJTableSearchFormSchema
                    {
                        Labels = new string[]{ "统计开始日期","统计结束日期" },
                        Field = nameof(WarehousePassageRecord.CreateDate),
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
                    DataIndex = nameof(WarehousePassageRecordsGroupRecord.WarehouseName),
                    Title = "仓库名称"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(WarehousePassageRecordsGroupRecord.CarNo),
                    Title = "车牌号"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(WarehousePassageRecordsGroupRecord.Count),
                    Title = "车次",
                    CustomTemplate = new AJTableCustomTemplate
                    {
                        Key = "CarCountBtn"
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
