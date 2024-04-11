using AJWPFAdmin.Core.Components.AJTable.ViewModels;
using AJWPFAdmin.Core.Utils;
using AJWPFAdmin.Core.Validation;
using AJWPFAdmin.Core;
using AJWPFAdmin.Services.EF.Tables;
using AJWPFAdmin.Services.EF;
using AJWPFAdmin.Services;
using MaterialDesignExtensions.Controls;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AJWPFAdmin.Core.Components.AJTable.Views.AJTable;
using static AJWPFAdmin.Core.ExceptionTool;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Masuit.Tools.Models;

namespace EntranceGuardManager.Modules.Main.ViewModels
{
    public class DatabaseConfigViewModel : AnnotationValidationViewModel, INavigationAware
    {
        private bool _loading;
        public bool Loading
        {
            get { return _loading; }
            set
            {
                SetProperty(ref _loading, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private AJExportProgress _exportProgress;
        public AJExportProgress ExportProgress
        {
            get { return _exportProgress; }
            set { SetProperty(ref _exportProgress, value); }
        }


        private string _dataSource;
        [Display(Name = "服务器地址")]
        [Required(ErrorMessage = "{0}必填")]
        public string DataSource
        {
            get { return _dataSource; }
            set
            {
                SetProperty(ref _dataSource, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private string _databaseName;

        private string _uid;
        [Display(Name = "数据库账号")]
        [Required(ErrorMessage = "{0}必填")]
        public string UId
        {
            get { return _uid; }
            set
            {
                SetProperty(ref _uid, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private string _password;
        [Display(Name = "数据库密码")]
        [Required(ErrorMessage = "{0}必填")]
        public string Password
        {
            get { return _password; }
            set
            {
                SetProperty(ref _password, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private string _autoBackupPath;
        [Display(Name = "自动备份路径")]
        [Required(ErrorMessage = "{0}必填")]
        public string AutoBackupPath
        {
            get { return _autoBackupPath; }
            set
            {
                SetProperty(ref _autoBackupPath, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private DateTime _autoBackupDate;
        [Display(Name = "自动备份时间")]
        [Required(ErrorMessage = "{0}必填")]
        public DateTime AutoBackupDate
        {
            get { return _autoBackupDate; }
            set
            {
                SetProperty(ref _autoBackupDate, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private DelegateCommand _openFileDialogCmd;
        public DelegateCommand OpenFileDialogCmd =>
            _openFileDialogCmd ?? (_openFileDialogCmd = new DelegateCommand(ExecuteOpenFileDialogCmd));

        async void ExecuteOpenFileDialogCmd()
        {
            var ret = await OpenDirectoryDialog.ShowDialogAsync(DialogIds.Root, new OpenDirectoryDialogArguments
            {
                CreateNewDirectoryEnabled = true,
                CurrentDirectory = AutoBackupPath
            });
            if (ret.Confirmed)
            {
                AutoBackupPath = ret.Directory;
            }
        }

        private DelegateCommand _saveCmd;
        public DelegateCommand SaveCmd =>
            _saveCmd ?? (_saveCmd = new DelegateCommand(ExecuteSaveCmdAsync, CanExecuteSaveCmd));

        async void ExecuteSaveCmdAsync()
        {
            if (!ValidateModel())
            {
                SaveCmd.RaiseCanExecuteChanged();
                return;
            }

            Loading = true;

            var str = $"server={_dataSource};user={_uid};password={_password};database={_databaseName}";

            var file = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

            await Task.Delay(2000);

            var obj = CommonUtil.TryGetJSONObject<JObject>(await File.ReadAllTextAsync(file));

            obj["ConnectionStrings"]["MYSQL"] = str;
            obj["DatabaseAutoBack"]["BackupPath"] = AutoBackupPath;
            obj["DatabaseAutoBack"]["BackupDate"] = AutoBackupDate.ToString("yyyy-MM-dd HH:mm:ss");

            await File.WriteAllTextAsync(file, obj.ToString());

            await CommonUtil.ShowAlertDialogAsync(new AlertDialogArguments
            {
                Title = "提示",
                Message = "保存成功!"
            });

            Loading = false;

        }

        bool CanExecuteSaveCmd()
        {
            return !HasErrors;
        }

        private List<DatabaseBackupRecord> _backupFiles;
        public List<DatabaseBackupRecord> BackupFiles
        {
            get { return _backupFiles; }
            set { SetProperty(ref _backupFiles, value); }
        }

        private List<AJTableColumnItem> _columns;

        public List<AJTableColumnItem> Columns
        {
            get { return _columns; }
            set { SetProperty(ref _columns, value); }
        }

        private bool _backupTableloading;

        public bool BackupTableloading
        {
            get { return _backupTableloading; }
            set { SetProperty(ref _backupTableloading, value); }
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
            if (row.DataContext is DatabaseBackupRecord data)
            {
                if (!File.Exists(data.FilePath))
                {
                    row.Foreground = (SolidColorBrush)Application.Current.Resources["AJRed"];
                }
            }
        }

        private DelegateCommand<DatabaseBackupRecord> _restoreCmd;
        public DelegateCommand<DatabaseBackupRecord> RestoreCmd =>
            _restoreCmd ?? (_restoreCmd = new DelegateCommand<DatabaseBackupRecord>(ExecuteRestoreCmd));

        async void ExecuteRestoreCmd(DatabaseBackupRecord parameter)
        {
            var confirm = await CommonUtil.ShowConfirmDialogAsync(new ConfirmationDialogArguments
            {
                Title = "提示",
                Message = $"请确保没有任何程序再录入/修改,否则会造成数据不一致或其他异常结果"
            });

            if (confirm)
            {
                if (!File.Exists(parameter.FilePath))
                {
                    await CommonUtil.ShowAlertDialogAsync(new AlertDialogArguments
                    {
                        Title = "还原失败!",
                        Message = "备份文件已不存在"
                    });
                    return;
                }

                ManualBackupProgress.Icon = "DatabaseArrowLeft";
                ManualBackupProgress.Progress = 0;
                ManualBackupProgress.Text = "准备数据中...";
                ManualBackupProgress.Loading = true;

                var worker = new BackgroundWorker
                {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = false,
                };

                worker.DoWork += (sneder, e) =>
                {
                    _aJDatabaseckupService.Restore((progress) =>
                    {
                        var percent = progress.Total > 0
                        ? Convert.ToInt32(Math.Floor((double)progress.Current / (double)progress.Total) * 100)
                        : 0;
                        worker.ReportProgress(percent, progress);
                        e.Result = progress.Success;
                    }, parameter.FilePath);
                };
                worker.ProgressChanged += (s, e) =>
                {
                    ManualBackupProgress.Progress = e.ProgressPercentage;
                    var progress = e.UserState as AJDatabaseckupService.BackupOrRestoreProgress;
                    ManualBackupProgress.Text = progress.Message;
                };
                worker.RunWorkerCompleted += async (s, e) =>
                {
                    var success = (e.Result as bool?).GetValueOrDefault();
                    if (success)
                    {
                        ExecuteGetListCmd();
                    }

                    if (e.Error != null)
                    {
                        await CommonUtil.ShowAlertDialogAsync(new AlertDialogArguments
                        {
                            Title = "操作发生错误",
                            Message = e.Error.Message,
                        });
                    }

                    if (!success)
                    {
                        await Task.Delay(3000);
                    }

                    ManualBackupProgress.Loading = false;

                };

                worker.RunWorkerAsync();
            }
        }

        private DelegateCommand<DatabaseBackupRecord> _deleteCmd;
        public DelegateCommand<DatabaseBackupRecord> DeleteCmd =>
            _deleteCmd ?? (_deleteCmd = new DelegateCommand<DatabaseBackupRecord>(ExecuteDeleteCmd));

        async void ExecuteDeleteCmd(DatabaseBackupRecord parameter)
        {
            var confirm = await CommonUtil.ShowConfirmDialogAsync(new ConfirmationDialogArguments
            {
                Title = "删除确认",
                Message = $"即将删除 {parameter.FileName} ?"
            });

            if (confirm)
            {

                var filePath = parameter.FilePath;

                db.DatabaseBackupRecords.Remove(parameter);
                await db.SaveChangesAsync();

                CommonUtil.TryDeleteFiles(new string[] { filePath });

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
                .FirstOrDefault(p => p.Field == nameof(DatabaseBackupRecord.FileName)).Value?.ToString();

            var dates = FormConfig.Schemas
                .FirstOrDefault(p => p.Field == nameof(DatabaseBackupRecord.CreateDate)).Value as ObservableCollection<DateTime?>;


            var list = await db.DatabaseBackupRecords.LikeOrLike(name, p => p.FileName)
                .BetweenInDates(dates.ToArray(), p => p.CreateDate)
                .OrderByDescending(p => p.CreateDate)
                .ToPagedListAsync(Pagination.Current, Pagination.PageSize);

            Pagination.Total = list.TotalCount;
            BackupFiles = list.Data;
            Loading = false;
        }

        private ManualBackupProgress _manualBackupProgress;
        public ManualBackupProgress ManualBackupProgress
        {
            get { return _manualBackupProgress; }
            set { SetProperty(ref _manualBackupProgress, value); }
        }

        private DelegateCommand _manualBackupCmd;
        public DelegateCommand ManualBackupCmd =>
            _manualBackupCmd ?? (_manualBackupCmd = new DelegateCommand(ExecuteManualBackupCmd));

        async void ExecuteManualBackupCmd()
        {
            var confirm = await CommonUtil.ShowConfirmDialogAsync(new ConfirmationDialogArguments
            {
                Title = "提示",
                Message = $"请确保没有任何程序再录入/修改记录,否则会造成数据不一致或其他异常"
            });

            if (!confirm)
            {
                return;
            }
            ManualBackupProgress.Icon = "DatabaseArrowRight";
            ManualBackupProgress.Progress = 0;
            ManualBackupProgress.Text = "准备数据中...";
            ManualBackupProgress.Loading = true;

            var worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = false,
            };

            worker.DoWork += (sneder, e) =>
            {
                _aJDatabaseckupService.Backup((progress) =>
                {
                    var percent = progress.Total > 0
                    ? Convert.ToInt32(Math.Floor((double)progress.Current / (double)progress.Total) * 100)
                    : 0;
                    worker.ReportProgress(percent, progress);
                    e.Result = progress.Success;
                }, AutoBackupPath);
            };
            worker.ProgressChanged += (s, e) =>
            {
                ManualBackupProgress.Progress = e.ProgressPercentage;
                var progress = e.UserState as AJDatabaseckupService.BackupOrRestoreProgress;
                ManualBackupProgress.Text = progress.Message;
            };
            worker.RunWorkerCompleted += async (s, e) =>
            {
                var success = (e.Result as bool?).GetValueOrDefault();
                if (success)
                {
                    ExecuteGetListCmd();
                }

                if (e.Error != null)
                {
                    await CommonUtil.ShowAlertDialogAsync(new AlertDialogArguments
                    {
                        Title = "操作发生错误",
                        Message = e.Error.Message,
                    });
                }

                if (!success)
                {
                    await Task.Delay(3000);
                }

                ManualBackupProgress.Loading = false;

            };

            worker.RunWorkerAsync();

        }

        private AJConfigService _cfgSvc;
        private AJDatabaseckupService _aJDatabaseckupService;
        private DbService db;
        private AJGlobalExceptionResolvedEvent _globalExceptionEvt;

        public DatabaseConfigViewModel(AJConfigService cfgSvc, DbService dbIns,
            IEventAggregator eventAggregator,
            AJDatabaseckupService aJDatabaseckupService)
        {
            _cfgSvc = cfgSvc;
            _aJDatabaseckupService = aJDatabaseckupService;
            db = dbIns;
            _globalExceptionEvt = eventAggregator.GetEvent<AJGlobalExceptionResolvedEvent>();
            _globalExceptionEvt.Subscribe(() =>
            {
                Loading = false;
            });
            ManualBackupProgress = new ManualBackupProgress
            {
                Icon = "DatabaseArrowRight"
            };
            

            var mySQLStrData = CommonUtil.DecryptMySqlConnStr(_cfgSvc.Config.GetConnectionString("MYSQL"));

            DataSource = mySQLStrData.host;
            UId = mySQLStrData.user;
            Password = mySQLStrData.password;
            _databaseName = mySQLStrData.database;

            var path = _cfgSvc.Config["DatabaseAutoBack:BackupPath"];
            AutoBackupPath = string.IsNullOrWhiteSpace(path)
                ? System.IO.Path.Combine(Directory.GetCurrentDirectory(), "backup") : path;
            if (!System.IO.Directory.Exists(AutoBackupPath))
            {
                System.IO.Directory.CreateDirectory(AutoBackupPath);
            }
            AutoBackupDate = DateTime.Parse(cfgSvc.Config["DatabaseAutoBack:BackupDate"]);

            FormConfig = new AJTableSearchFormConfig
            {
                Schemas = new ObservableCollection<AJTableSearchFormSchema>
                {
                    new AJTableSearchFormSchema
                    {
                        Label = "搜索文件名",
                        Field = nameof(DatabaseBackupRecord.FileName),
                        Type = AJTableSchemaType.Input,
                        IsPopular = true,
                        Placeholder = "名称"
                    },
                    new AJTableSearchFormSchema
                    {
                        Labels = new string[]{ "备份开始日期","备份结束日期" },
                        Field = nameof(DatabaseBackupRecord.CreateDate),
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
                    DataIndex = nameof(DatabaseBackupRecord.FileName),
                    Title = "备份文件名"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(DatabaseBackupRecord.FileSize),
                    Title = "文件大小",
                    Formatter = new AJTableColumnFormatter
                    {
                        Handler = (val) =>
                        {
                            return CommonUtil.FormatFileSize( (long)val);
                        }
                    }
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(DatabaseBackupRecord.FilePath),
                    Title = "所在路径"
                },
                new AJTableColumnItem
                {
                    DataIndex = nameof(DatabaseBackupRecord.CreateDate),
                    Title = "备份日期",
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

    public class ManualBackupProgress : BindableBase
    {
        private bool _loading;
        public bool Loading
        {
            get { return _loading; }
            set { SetProperty(ref _loading, value); }
        }

        private string _text;
        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        private double _progress;
        public double Progress
        {
            get { return _progress; }
            set { SetProperty(ref _progress, value); }
        }

        private string _icon;
        public string Icon
        {
            get { return _icon; }
            set
            {
                SetProperty(ref _icon, value);
            }
        }
    }
}
