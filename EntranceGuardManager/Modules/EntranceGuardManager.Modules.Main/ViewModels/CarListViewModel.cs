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

    public class CarListViewModel : BindableBase, INavigationAware
    {

        private List<Car> _rows;
        public List<Car> Rows
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

        private DelegateCommand<Car> _editCmd;
        public DelegateCommand<Car> EditCmd =>
            _editCmd ?? (_editCmd = new DelegateCommand<Car>(ExecuteEditCmd));

        void ExecuteEditCmd(Car parameter)
        {
            var @params = new DialogParameters
            {
                { "data", parameter },
                { "typeOptions", _typeOptions },
            };
            _dialogSvc.ShowDialog(nameof(CarForm), @params, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    ExecuteSearchCmd(true);
                }
            });
        }


        private DelegateCommand<Car> _deleteCmd;
        public DelegateCommand<Car> DeleteCmd =>
            _deleteCmd ?? (_deleteCmd = new DelegateCommand<Car>(ExecuteDeleteCmd));

        async void ExecuteDeleteCmd(Car parameter)
        {
            var confirm = await CommonUtil.ShowConfirmDialogAsync(new ConfirmationDialogArguments
            {
                Title = "删除确认",
                Message = $"即将删除 {parameter.CarNo} ?"
            });

            if (confirm)
            {
                db.Cars.Remove(parameter);
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
                .FirstOrDefault(p => p.Field == nameof(Car.CarNo)).Value?.ToString();

            var typeformSchema = FormConfig.Schemas
                .FirstOrDefault(p => p.Field == nameof(Car.TypeId));

            var typeId = typeformSchema.Value == null ? new long?() : (long)typeformSchema.Value;

            var dates = FormConfig.Schemas
                .FirstOrDefault(p => p.Field == nameof(Car.CreateDate)).Value as ObservableCollection<DateTime?>;

            var list = await db.Cars
                .LikeOrLike(name, p => p.CarNo)
                .IfWhere(() => typeId.HasValue, p => p.TypeId == typeId)
                .BetweenInDates(dates.ToArray(), p => p.CreateDate)
                .OrderByDescending(p => p.CreateDate)
                .ToPagedListAsync(Pagination.Current, Pagination.PageSize);

            Pagination.Total = list.TotalCount;

            if ((TypeOptions?.Count).GetValueOrDefault() == 0)
            {
                var options = await db.CarTypes.AsNoTracking()
                .OrderBy(p => p.Id)
                .Select(p => new AJTableFormSchemaItemOptions
                {
                    Value = p.Id,
                    Label = p.Name
                }).ToListAsync();

                TypeOptions = typeformSchema.Options
                    = new ObservableCollection<AJTableFormSchemaItemOptions>(options);
            }
            

            Rows = list.Data;

            Loading = false;
        }

        private ObservableCollection<AJTableFormSchemaItemOptions> _typeOptions;
        public ObservableCollection<AJTableFormSchemaItemOptions> TypeOptions
        {
            get { return _typeOptions; }
            set
            {
                SetProperty(ref _typeOptions, value);
            }
        }

        private IDialogService _dialogSvc;
        private DbService db;


        public CarListViewModel(DbService dbIns, IEventAggregator eventAggregator, IDialogService dialogSvc)
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
                        Field = nameof(Car.CarNo),
                        Type = AJTableSchemaType.Input,
                        IsPopular = true,
                        Placeholder = "车牌号"
                    },
                    new AJTableSearchFormSchema
                    {
                        Label = "类型",
                        Field = nameof(Car.TypeId),
                        Type = AJTableSchemaType.Select,
                        IsPopular = true,
                        Placeholder = "筛选类型"
                    },
                    new AJTableSearchFormSchema
                    {
                        Labels = new string[]{ "创建开始日期","创建结束日期" },
                        Field = nameof(Car.CreateDate),
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
                    DataIndex = nameof(Car.CarNo),
                    Title = "车牌号"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(Car.TypeName),
                    Title = "类型"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(Car.PaiFangJieDuan),
                    Title = "排放阶段"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(Car.EngineNo),
                    Title = "发动机号"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(Car.VIN),
                    Title = "VIN"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(Car.RegDate),
                    Title = "注册日期",
                    Formatter = new AJTableColumnFormatter
                    {
                        Handler = (val) =>
                        {
                            return val == null
                            ? "--"
                            : ((DateTime?)val).GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(Car.TeamName),
                    Title = "车队名称"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(Car.MaterialName),
                    Title = "货物"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(Car.CarNetWeight),
                    Title = "重量(KG)"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(Car.ExpireDate),
                    Title = "有效期",
                    Formatter = new AJTableColumnFormatter
                    {
                        Handler = (val) =>
                        {
                            return val == null 
                            ? "长期有效" 
                            : ((DateTime?)val).GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(Car.CreateDate),
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
