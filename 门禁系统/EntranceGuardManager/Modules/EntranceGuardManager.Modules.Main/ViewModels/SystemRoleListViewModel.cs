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
using AJWPFAdmin.Services;
using Microsoft.EntityFrameworkCore;

namespace EntranceGuardManager.Modules.Main.ViewModels
{

    public class SystemRoleListViewModel : BindableBase, INavigationAware
    {

        private List<SystemRole> _rows;
        public List<SystemRole> Rows
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

        private DelegateCommand<SystemRole> _editCmd;
        public DelegateCommand<SystemRole> EditCmd =>
            _editCmd ?? (_editCmd = new DelegateCommand<SystemRole>(ExecuteEditCmd));

        void ExecuteEditCmd(SystemRole parameter)
        {
            var @params = new DialogParameters
            {
                { "data", parameter },
            };
            _dialogSvc.ShowDialog(nameof(SystemRoleForm), @params, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    ExecuteSearchCmd(true);
                }
            });
        }


        private DelegateCommand<SystemRole> _deleteCmd;
        public DelegateCommand<SystemRole> DeleteCmd =>
            _deleteCmd ?? (_deleteCmd = new DelegateCommand<SystemRole>(ExecuteDeleteCmd));

        async void ExecuteDeleteCmd(SystemRole parameter)
        {
            var rid = parameter.Id;
            var rel = await db.SystemUsers.AnyAsync(p => p.RoleId == rid);

            if (rel)
            {
                await CommonUtil.ShowAlertDialogAsync(new AlertDialogArguments
                {
                    Title = "提示",
                    Message = "还有其他账号关联该角色,请调整后重试"
                });
                return;
            }
            var confirm = await CommonUtil.ShowConfirmDialogAsync(new ConfirmationDialogArguments
            {
                Title = "删除确认",
                Message = $"即将删除 {parameter.Name} ?"
            });

            if (confirm)
            {
                db.SystemRoles.Remove(parameter);
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
                .FirstOrDefault(p => p.Field == nameof(SystemRole.Name)).Value?.ToString();

            var dates = FormConfig.Schemas
                .FirstOrDefault(p => p.Field == nameof(SystemRole.CreateDate)).Value as ObservableCollection<DateTime?>;

            var list = await db.SystemRoles.LikeOrLike(name, p => p.Name)
                .BetweenInDates(dates.ToArray(), p => p.CreateDate)
                .OrderByDescending(p => p.CreateDate)
                .ToPagedListAsync(Pagination.Current, Pagination.PageSize);

            Pagination.Total = list.TotalCount;
            Rows = list.Data;
            Loading = false;
        }

        private IDialogService _dialogSvc;
        private DbService db;
        


        public SystemRoleListViewModel(DbService dbIns,
            IEventAggregator eventAggregator,
            IDialogService dialogSvc)
        {

            _dialogSvc = dialogSvc;
            eventAggregator.GetEvent<AJGlobalExceptionResolvedEvent>().Subscribe(() => Loading = false);


            FormConfig = new AJTableSearchFormConfig
            {
                Schemas = new ObservableCollection<AJTableSearchFormSchema>
                {
                    new AJTableSearchFormSchema
                    {
                        Label = "搜索名称",
                        Field = nameof(SystemRole.Name),
                        Type = AJTableSchemaType.Input,
                        IsPopular = true,
                        Placeholder = "搜索"
                    },
                    new AJTableSearchFormSchema
                    {
                        Labels = new string[]{ "创建开始日期","创建结束日期" },
                        Field = nameof(SystemRole.CreateDate),
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
                    DataIndex = nameof(SystemRole.Name),
                    Title = "名称"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(SystemRole.Description),
                    Title = "描述/说明"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(SystemRole.Permission),
                    Title = "权限概览",
                    Formatter = new AJTableColumnFormatter
                    {
                        Handler = (val) =>
                        {
                            return "所有权限";
                        }
                    }
                },
                //new AJTableColumnItem
                //{
                //    DataIndex = nameof(SystemRole.Phone),
                //    Title = "手机号"
                //},
                //new AJTableColumnItem
                //{
                //    DataIndex = nameof(SystemRole.NickName),
                //    Title = "昵称"
                //},
                new AJTableColumnItem
                {
                    DataIndex = nameof(SystemRole.CreateDate),
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
