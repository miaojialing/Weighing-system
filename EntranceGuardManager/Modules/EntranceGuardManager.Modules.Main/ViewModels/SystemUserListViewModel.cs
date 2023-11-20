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

namespace EntranceGuardManager.Modules.Main.ViewModels
{

    public class SystemUserListViewModel : BindableBase, INavigationAware
    {

        private List<SystemUser> _rows;
        public List<SystemUser> Rows
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

        private DelegateCommand<SystemUser> _editCmd;
        public DelegateCommand<SystemUser> EditCmd =>
            _editCmd ?? (_editCmd = new DelegateCommand<SystemUser>(ExecuteEditCmd));

        void ExecuteEditCmd(SystemUser parameter)
        {
            var @params = new DialogParameters
            {
                { "data", parameter },
            };
            _dialogSvc.ShowDialog(nameof(SystemUserForm), @params, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    ExecuteSearchCmd(true);
                }
            });
        }


        private DelegateCommand<SystemUser> _deleteCmd;
        public DelegateCommand<SystemUser> DeleteCmd =>
            _deleteCmd ?? (_deleteCmd = new DelegateCommand<SystemUser>(ExecuteDeleteCmd));

        async void ExecuteDeleteCmd(SystemUser parameter)
        {
            if (_sysUserSvc.CurrnetUser.Id == parameter.Id)
            {
                await CommonUtil.ShowAlertDialogAsync(new AlertDialogArguments
                {
                    Title = "提示",
                    Message = "当前登录账户不允许删除自身,请更换其他账号重试"
                });
                return;
            }
            var confirm = await CommonUtil.ShowConfirmDialogAsync(new ConfirmationDialogArguments
            {
                Title = "删除确认",
                Message = $"即将删除 {parameter.AccountName} ?"
            });

            if (confirm)
            {
                db.SystemUsers.Remove(parameter);
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
                .FirstOrDefault(p => p.Field == nameof(SystemUser.AccountName)).Value?.ToString();

            var dates = FormConfig.Schemas
                .FirstOrDefault(p => p.Field == nameof(SystemUser.CreateDate)).Value as ObservableCollection<DateTime?>;

            var list = await db.SystemUsers.LikeOrLike(name, p => p.AccountName, p => p.Phone)
                .BetweenInDates(dates.ToArray(), p => p.CreateDate)
                .OrderByDescending(p => p.CreateDate)
                .ToPagedListAsync(Pagination.Current, Pagination.PageSize);

            Pagination.Total = list.TotalCount;
            Rows = list.Data;
            Loading = false;
        }

        private IDialogService _dialogSvc;
        private DbService db;
        private SystemUserService _sysUserSvc;


        public SystemUserListViewModel(DbService dbIns, SystemUserService sysUserSvc,
            IEventAggregator eventAggregator,
            IDialogService dialogSvc)
        {

            _dialogSvc = dialogSvc;
            _sysUserSvc = sysUserSvc;
            eventAggregator.GetEvent<AJGlobalExceptionResolvedEvent>().Subscribe(() => Loading = false);


            FormConfig = new AJTableSearchFormConfig
            {
                Schemas = new ObservableCollection<AJTableSearchFormSchema>
                {
                    new AJTableSearchFormSchema
                    {
                        Label = "名称/手机号",
                        Field = nameof(SystemUser.AccountName),
                        Type = AJTableSchemaType.Input,
                        IsPopular = true,
                        Placeholder = "搜索"
                    },
                    new AJTableSearchFormSchema
                    {
                        Labels = new string[]{ "创建开始日期","创建结束日期" },
                        Field = nameof(SystemUser.CreateDate),
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
                    DataIndex = nameof(SystemUser.AccountName),
                    Title = "账号"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(SystemUser.Enable),
                    Title = "状态",
                    Formatter = new AJTableColumnFormatter
                    {
                        Handler = (val) =>
                        {
                            return (bool)val  ? "启用" : "禁用";
                        }
                    }
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(SystemUser.RoleName),
                    Title = "角色"
                },
                //new AJTableColumnItem
                //{
                //    DataIndex = nameof(SystemUser.Phone),
                //    Title = "手机号"
                //},
                //new AJTableColumnItem
                //{
                //    DataIndex = nameof(SystemUser.NickName),
                //    Title = "昵称"
                //},
                new AJTableColumnItem
                {
                    DataIndex = nameof(SystemUser.CreateDate),
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
