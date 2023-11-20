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
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using AJWPFAdmin.Core.Excel;
using System.Linq.Expressions;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EntranceGuardManager.Modules.Main.ViewModels
{

    public class ShippingRecordListViewModel : BindableBase, INavigationAware
    {
        private ShippingRecordNavParamType _navParamType;

        private ShippingRecordExportConfig _exportConfig;
        public ShippingRecordExportConfig ExportConfig
        {
            get { return _exportConfig; }
            set { SetProperty(ref _exportConfig, value); }
        }

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

        private DelegateCommand<ShippingRecord> _editCmd;
        public DelegateCommand<ShippingRecord> EditCmd =>
            _editCmd ?? (_editCmd = new DelegateCommand<ShippingRecord>(ExecuteEditCmd));

        void ExecuteEditCmd(ShippingRecord parameter)
        {
            var @params = new DialogParameters
            {
                { "data", parameter },
            };
            _dialogSvc.ShowDialog(nameof(ShippingRecordForm), @params, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    ExecuteSearchCmd(true);
                }
            });
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

        private DelegateCommand<string> _toggleExportConfigCmd;
        public DelegateCommand<string> ToggleExportConfigCmd =>
            _toggleExportConfigCmd ?? (_toggleExportConfigCmd = new DelegateCommand<string>(ExecutetoggleExportConfigCmd));

        void ExecutetoggleExportConfigCmd(string parameter)
        {
            ExportConfig.Open = parameter.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        private DelegateCommand _exportCmd;
        public DelegateCommand ExportCmd =>
            _exportCmd ?? (_exportCmd = new DelegateCommand(ExecuteExportCmd));

        void ExecuteExportCmd()
        {
            ExportConfig.Open = false;

            ExportProgress.Current = 0;
            ExportProgress.Total = 100;
            ExportProgress.Text = "导出中...";
            ExportProgress.Loading = true;

            var worker = new BackgroundWorker();

            worker.WorkerReportsProgress = true;

            var page = 1;
            var limit = 10;
            var rows = new List<ShippingRecordExportModel>();

            var name = FormConfig.Schemas
                .FirstOrDefault(p => p.Field == nameof(ShippingRecord.CarNo)).Value?.ToString();

            var dates = FormConfig.Schemas
                .FirstOrDefault(p => p.Field == nameof(ShippingRecord.CreateDate)).Value as ObservableCollection<DateTime?>;

            var exportImage = ExportConfig.ExportImage;


            worker.DoWork += (s, e) =>
            {
                while (true)
                {
                    var list = db.ShippingRecords.LikeOrLike(name, p => p.CarNo)
                    .IfWhere(() => _navParamType == ShippingRecordNavParamType.在场记录,
                                p => p.ShipEndDate == null)
                    .IfWhere(() => _navParamType == ShippingRecordNavParamType.进出记录,
                                p => p.ShipEndDate != null)
                    .BetweenInDates(dates.ToArray(), p => p.CreateDate)
                    .OrderByDescending(p => p.CreateDate)
                    .AsNoTracking()
                    .Select(p => new ShippingRecordExportModel
                    {

                        Id = p.Id,
                        CarNo = p.CarNo,
                        WatchhouseName = p.WatchhouseName,
                        PassagewayName = p.PassagewayName,
                        ExitPassagewayName = p.ExitPassagewayName,
                        ExitWatchhouseName = p.ExitWatchhouseName,
                        WarehouseName = p.WatchhouseName,
                        TypeName = p.TypeName,
                        Status = p.Direction == PassagewayDirection.进 ? "进厂" : "出厂",
                        AutoPass = p.AutoPass ? "是" : "否",
                        EngineNo = p.EngineNo,
                        VIN = p.VIN,
                        PaiFangJieDuan = p.PaiFangJieDuan,
                        RegDate = p.RegDate,
                        MaterialName = p.MaterialName,
                        CarNetWeight = p.CarNetWeight,
                        TeamName = p.TeamName,
                        ShipStartDate = p.ShipStartDate,
                        ShipEndDate = p.ShipEndDate,
                        EntranceIdentifiedCaptureFile = exportImage ? p.EntranceIdentifiedCaptureFile : "",
                        EntranceCameraCaptureFile = exportImage ? p.EntranceCameraCaptureFile : "",
                        ExitIdentifiedCaptureFile = exportImage ? p.ExitIdentifiedCaptureFile : "",
                        ExitCameraCaptureFile = exportImage ? p.ExitCameraCaptureFile : "",
                    }).ToPagedList(page, limit);

                    if (list.Data.Count > 0)
                    {
                        rows.AddRange(list.Data);
                    }

                    if (!list.HasNext)
                    {
                        worker.ReportProgress(page, list.TotalCount);
                        Thread.Sleep(600);
                        break;
                    }

                    page++;
                    worker.ReportProgress(page, list.TotalCount);
                    Thread.Sleep(600);
                }

            };
            worker.ProgressChanged += (s, e) =>
            {
                ExportProgress.Total = (int)e.UserState;
                ExportProgress.Current = rows.Count;
                ExportProgress.Text = $"正在导出...{ExportProgress.Current}/{ExportProgress.Total}";
            };
            worker.RunWorkerCompleted += async (s, e) =>
            {
                ExportProgress.Text = $"选择保存位置...";
                ExportProgress.Current = rows.Count;
                Thread.Sleep(1500);

                var fileName = $"台账记录导出_{DateTime.Now.ToString("yyyyMMddHHmmss")}";

                var fileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    CreatePrompt = true,
                    OverwritePrompt = true,
                    AddExtension = true,
                    DefaultExt = ".xlsx",
                    Filter = "excel 文件（.xlsx）|*.xlsx",
                    FileName = fileName,
                };

                if ((fileDialog.ShowDialog()).GetValueOrDefault())
                {
                    ExportProgress.Text = $"正在写入...";

                    var result = await Task.Run(() =>
                    {
                        return AJExport.CreateExcel(rows, fileDialog.FileName, fileName);
                    });
                    if (!result.Success)
                    {
                        ExportProgress.Text = $"导出失败!{result.Message}";
                        Thread.Sleep(3000);
                    }
                    else
                    {
                        ExportProgress.Text = $"导出成功!";
                        Thread.Sleep(1500);
                    }

                }

                ExportProgress.Loading = false;
                ExportConfig.ExportImage = false;
            };

            worker.RunWorkerAsync();
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
                .FirstOrDefault(p => p.Field == nameof(ShippingRecord.CreateDate)).Value as ObservableCollection<DateTime?>;

            var list = await db.ShippingRecords.LikeOrLike(name, p => p.CarNo)
                .IfWhere(() => _navParamType == ShippingRecordNavParamType.在场记录,
                            p => p.ShipEndDate == null)
                .IfWhere(() => _navParamType == ShippingRecordNavParamType.进出记录,
                            p => p.ShipEndDate != null)
                .BetweenInDates(dates.ToArray(), p => p.CreateDate)
                .OrderByDescending(p => p.CreateDate)
                .ToPagedListAsync(Pagination.Current, Pagination.PageSize);

            Pagination.Total = list.TotalCount;
            Rows = list.Data;
            Loading = false;
        }

        private IDialogService _dialogSvc;
        private DbService db;


        public ShippingRecordListViewModel(DbService dbIns, IEventAggregator eventAggregator, IDialogService dialogSvc)
        {

            _dialogSvc = dialogSvc;
            eventAggregator.GetEvent<AJGlobalExceptionResolvedEvent>().Subscribe(() =>
            {
                Loading = false;
                ExportProgress.Loading = false;
            });

            ExportProgress = new AJExportProgress();
            ExportConfig = new ShippingRecordExportConfig();

            FormConfig = new AJTableSearchFormConfig
            {
                Schemas = new ObservableCollection<AJTableSearchFormSchema>
                {
                    new AJTableSearchFormSchema
                    {
                        Label = "车牌号",
                        Field = nameof(ShippingRecord.CarNo),
                        Type = AJTableSchemaType.Input,
                        IsPopular = true,
                        Placeholder = "搜索"
                    },
                    new AJTableSearchFormSchema
                    {
                        Labels = new string[]{ "入厂开始日期","入厂结束日期" },
                        Field = nameof(ShippingRecord.CreateDate),
                        Type = AJTableSchemaType.RangePicker,
                        Value = new ObservableCollection<DateTime?>( new DateTime?[2]{null,null}),
                        IsPopular = true,
                    },
                },
                AdvFilterVisibility = System.Windows.Visibility.Collapsed,
                ExportVisibility = System.Windows.Visibility.Visible,
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
                    DataIndex = nameof(ShippingRecord.Id),
                    Title = "台账编号"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(ShippingRecord.CarNo),
                    Title = "车牌号"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(ShippingRecord.WatchhouseName),
                    Title = "进厂岗亭"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(ShippingRecord.PassagewayName),
                    Title = "进厂通道"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(ShippingRecord.TypeName),
                    Title = "车辆类型",
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(ShippingRecord.ExitWatchhouseName),
                    Title = "出厂岗亭"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(ShippingRecord.ExitPassagewayName),
                    Title = "出厂通道"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(ShippingRecord.WarehouseName),
                    Title = "仓库名称"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(ShippingRecord.Direction),
                    Title = "状态",
                    Formatter = new AJTableColumnFormatter
                    {
                        Handler = (val) =>
                        {
                            return ((PassagewayDirection)val) == PassagewayDirection.进 ? "进厂" : "出厂";
                        }
                    }
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(ShippingRecord.AutoPass),
                    Title = "自动开闸",
                    Formatter = new AJTableColumnFormatter
                    {
                        Handler = (val) =>
                        {
                            return ((bool)val)  ? "是" : "否";
                        }
                    }
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(ShippingRecord.EngineNo),
                    Title = "发动机号",
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(ShippingRecord.VIN),
                    Title = nameof(ShippingRecord.VIN),
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(ShippingRecord.PaiFangJieDuan),
                    Title = "排放阶段",
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(ShippingRecord.RegDate),
                    Title = "注册日期",
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(ShippingRecord.MaterialName),
                    Title = "货物",
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(ShippingRecord.CarNetWeight),
                    Title = "重量(KG)",
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(ShippingRecord.ShipStartDate),
                    Title = "入厂日期",
                    Formatter = new AJTableColumnFormatter
                    {
                        Handler = (val) =>
                        {
                            return val == null ? "--"
                            : ((DateTime?)val).GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(ShippingRecord.ShipEndDate),
                    Title = "出厂日期",
                    Formatter = new AJTableColumnFormatter
                    {
                        Handler = (val) =>
                        {
                            return val == null ? "--"
                            : ((DateTime?)val).GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }
                }
            };
            Pagination = new AJTablePagination();
            db = dbIns;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            navigationContext.Parameters.TryGetValue("params", out _navParamType);
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

    public class ShippingRecordExportConfig : BindableBase
    {
        private bool _open;
        public bool Open
        {
            get { return _open; }
            set { SetProperty(ref _open, value); }
        }

        private bool _exportImage;
        public bool ExportImage
        {
            get { return _exportImage; }
            set { SetProperty(ref _exportImage, value); }
        }
    }

    public class ShippingRecordExportModel
    {
        [AJExportField(ColumnName = "台账编号", ColumnIndex = 1)]
        public long Id { get; set; }

        [AJExportField(ColumnName = "车牌号", ColumnIndex = 2)]
        public string CarNo { get; set; }

        [AJExportField(ColumnName = "进厂岗亭", ColumnIndex = 3)]
        public string WatchhouseName { get; set; }

        [AJExportField(ColumnName = "进厂通道", ColumnIndex = 4)]
        public string PassagewayName { get; set; }

        [AJExportField(ColumnName = "车辆类型", ColumnIndex = 5)]
        public string TypeName { get; set; }

        [AJExportField(ColumnName = "状态", ColumnIndex = 6)]
        public string Status { get; set; }

        [AJExportField(ColumnName = "出厂岗亭", ColumnIndex = 7)]
        public string ExitWatchhouseName { get; set; }

        [AJExportField(ColumnName = "出厂通道", ColumnIndex = 8)]
        public string ExitPassagewayName { get; set; }

        [AJExportField(ColumnName = "仓库名称", ColumnIndex = 9)]
        public string WarehouseName { get; set; }

        [AJExportField(ColumnName = "自动开闸", ColumnIndex = 10)]
        public string AutoPass { get; set; }

        [AJExportField(ColumnName = "发动机号", ColumnIndex = 11)]
        public string EngineNo { get; set; }

        [AJExportField(ColumnName = "VIN", ColumnIndex = 12)]
        public string VIN { get; set; }

        [AJExportField(ColumnName = "排放阶段", ColumnIndex = 13)]
        public string PaiFangJieDuan { get; set; }

        [AJExportField(ColumnName = "注册日期", ColumnIndex = 14)]
        public DateTime? RegDate { get; set; }

        [AJExportField(ColumnName = "货物", ColumnIndex = 15)]
        public string MaterialName { get; set; }

        [AJExportField(ColumnName = "重量(KG)", ColumnIndex = 16)]
        public decimal CarNetWeight { get; set; }

        [AJExportField(ColumnName = "车队名称", ColumnIndex = 17)]
        public string TeamName { get; set; }

        [AJExportField(ColumnName = "入厂日期", ColumnIndex = 18)]
        public DateTime? ShipStartDate { get; set; }

        [AJExportField(ColumnName = "出厂日期", ColumnIndex = 19)]
        public DateTime? ShipEndDate { get; set; }

        [AJExportField(ColumnName = "入口车牌识别图", IsImage = true, ColumnIndex = 20)]
        public string EntranceIdentifiedCaptureFile { get; set; }

        [AJExportField(ColumnName = "入口监控抓拍图", IsImage = true, ColumnIndex = 21)]
        public string EntranceCameraCaptureFile { get; set; }

        [AJExportField(ColumnName = "出口车牌识别图", IsImage = true, ColumnIndex = 22)]
        public string ExitIdentifiedCaptureFile { get; set; }

        [AJExportField(ColumnName = "出口监控抓拍图", IsImage = true, ColumnIndex = 23)]
        public string ExitCameraCaptureFile { get; set; }
    }
}
