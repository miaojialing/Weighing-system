using AJWPFAdmin.Modules.Main.Views;
using AJWPFAdmin.Core.Components.AJTable.ViewModels;
using AJWPFAdmin.Core.Utils;
using AJWPFAdmin.Services;
using AJWPFAdmin.Services.EF;
using AJWPFAdmin.Services.EF.Tables;
using Masuit.Tools.Models;
using MaterialDesignExtensions.Controls;
using MaterialDesignThemes.Wpf;
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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static AJWPFAdmin.Core.Components.AJTable.Views.AJTable;
using static AJWPFAdmin.Core.ExceptionTool;
using AJWPFAdmin.Core.Validation;
using Masuit.Tools;
using AJWPFAdmin.Core.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AJWPFAdmin.Core;
using Masuit.Tools.Systems;
using Newtonsoft.Json;
using AJWPFAdmin.Core.Components.AJFilePicker.ViewModels;
using AJWPFAdmin.Core.ExtensionMethods;
using Prism.Ioc;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Masuit.Tools.DateTimeExt;
using LiveChartsCore.SkiaSharpView.WPF;
using System.Windows;
using LiveChartsCore.VisualElements;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Windows.Media;

namespace AJWPFAdmin.Modules.Main.ViewModels
{
    public class ShippingRecordFormViewModel : AnnotationValidationViewModel, IDialogAware, INavigationAware
    {
        private bool _loading;
        public bool Loading
        {
            get { return _loading; }
            set { SetProperty(ref _loading, value); }
        }

        private bool _preparing;
        public bool Preparing
        {
            get { return _preparing; }
            set { SetProperty(ref _preparing, value); }
        }



        private ShippingRecordChart _chart;
        public ShippingRecordChart Chart
        {
            get { return _chart; }
            set { SetProperty(ref _chart, value); }
        }

        private string _fromRegionName;

        private ShippingRecord _form;

        private string _boatNo;
        [Display(Name = "船号")]
        [Required(ErrorMessage = "{0}必填")]
        [MaxLength(120)]
        [StringLength(120, ErrorMessage = "{0}超长:{1}")]
        public string BoatNo
        {
            get { return _boatNo; }
            set
            {
                if (SetProperty(ref _boatNo, value))
                {
                    _form.CarNo = value;
                }
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private string _materialCategory;
        [Display(Name = "物料种类")]
        [Required(ErrorMessage = "{0}必填")]
        [StringLength(50, ErrorMessage = "{0}超长:{1}")]
        public string MaterialCategory
        {
            get { return _materialCategory; }
            set
            {
                if (SetProperty(ref _materialCategory, value))
                {
                    _form.MaterialCategory = value;
                }
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private string _materialName;
        [Display(Name = "物料名称")]
        [Required(ErrorMessage = "{0}必填")]
        [StringLength(50, ErrorMessage = "{0}超长:{1}")]
        public string MaterialName
        {
            get { return _materialName; }
            set
            {
                if (SetProperty(ref _materialName, value))
                {
                    _form.MaterialName = value;
                }
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private DateTime _arriveDate;
        public DateTime ArriveDate
        {
            get { return _arriveDate; }
            set
            {
                if (SetProperty(ref _arriveDate, value))
                {
                    _form.ArriveDate = value;
                }
            }
        }

        private DateTime? _shipStartDate;
        public DateTime? ShipStartDate
        {
            get { return _shipStartDate; }
            set
            {
                SetProperty(ref _shipStartDate, value);
            }
        }

        private DateTime? _shipEndDate;
        public DateTime? ShipEndDate
        {
            get { return _shipEndDate; }
            set
            {
                SetProperty(ref _shipEndDate, value);
            }
        }

        private string _orderNetWeight;
        [Display(Name = "船运单净重")]
        [Required(ErrorMessage = "{0}必填")]
        [AJFormNumberField]
        public string OrderNetWeight
        {
            get { return _orderNetWeight; }
            set
            {
                SetProperty(ref _orderNetWeight, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private string _jianJin;
        [Display(Name = "检斤")]
        [Required(ErrorMessage = "{0}必填")]
        [AJFormNumberField]
        public string JianJin
        {
            get { return _jianJin; }
            set
            {
                SetProperty(ref _jianJin, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private string _carNetWeight;
        [Display(Name = "汽车衡净重")]
        [Required(ErrorMessage = "{0}必填")]
        [AJFormNumberField]
        public string CarNetWeight
        {
            get { return _carNetWeight; }
            set
            {
                SetProperty(ref _carNetWeight, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private string _senderName;
        [Display(Name = "发货单位")]
        [StringLength(200, ErrorMessage = "{0}超长:{1}")]
        public string SenderName
        {
            get { return _senderName; }
            set
            {
                SetProperty(ref _senderName, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private string _receiverName;
        [Display(Name = "收货单位")]
        [StringLength(200, ErrorMessage = "{0}超长:{1}")]
        public string ReceiverName
        {
            get { return _receiverName; }
            set
            {
                SetProperty(ref _receiverName, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        private string _berth;
        [Display(Name = "装/卸货地点泊位")]
        [MaxLength(50)]
        [StringLength(50, ErrorMessage = "{0}超长:{1}")]
        public string Berth
        {
            get { return _berth; }
            set
            {
                SetProperty(ref _berth, value);
                SaveCmd.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<string> MaterialCategoryOptions { get; set; }
        public ObservableCollection<string> MaterialNameOptions { get; set; }

        private string _dialogTitle;
        public string DialogTitle
        {
            get { return _dialogTitle; }
            set { SetProperty(ref _dialogTitle, value); }
        }

        private DelegateCommand _prepareCmd;
        public DelegateCommand PrepareCmd =>
            _prepareCmd ?? (_prepareCmd = new DelegateCommand(ExecutePrepareCmd));

        async void ExecutePrepareCmd()
        {

            Preparing = true;

            var source = await db.MaterialDictionaries.AsNoTracking().ToListAsync();

            if (string.IsNullOrWhiteSpace(_fromRegionName))
            {

                var startDate = DateTime.Parse(Chart.XAxes[0].Labels[0]);
                var endDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1);

                var summarySrouce = await db.ShippingRecords
                    .Where(p => p.CreateDate >= startDate && p.CreateDate <= endDate)
                    .Select(p => new
                    {
                        p.CreateDate,
                        p.CarNo
                    }).ToListAsync();

                var chartSeries = new int[7];
                for (int i = 0; i < Chart.XAxes[0].Labels.Count; i++)
                {
                    var dateStr = Chart.XAxes[0].Labels[i];
                    chartSeries[i] = summarySrouce.Count(p => p.CreateDate.Date == DateTime.Parse(dateStr));
                }
                Chart.Series = new ISeries[]
                {
                    new LineSeries<int>
                    {
                        Values = chartSeries
                    }
                };

                Chart.Visibility = Visibility.Visible;
            }


            Preparing = false;



            MaterialCategoryOptions.Clear();

            MaterialCategoryOptions.AddRange(source.Where(p => p.Type == MaterialType.种类)
                .OrderBy(p => p.SortNo).Select(p => p.Name));

            MaterialNameOptions.Clear();

            MaterialNameOptions.AddRange(source.Where(p => p.Type == MaterialType.名称)
                .OrderBy(p => p.SortNo).Select(p => p.Name));
        }

        private DelegateCommand _saveCmd;
        public DelegateCommand SaveCmd =>
            _saveCmd ?? (_saveCmd = new DelegateCommand(ExecuteSaveCmdAsync, CanExecuteSaveCmd));

        async void ExecuteSaveCmdAsync()
        {
            if (!ValidateModel())
            {
                SaveCmd.RaiseCanExecuteChanged();
                return;//FriendlyError("所提交数据有误", dialogId: DialogIds.DialogWindow);
            }

            var cateVal = MaterialCategory.Trim();
            var nameVal = MaterialName.Trim();

            if (!db.MaterialDictionaries.Any(p => p.Type == MaterialType.种类 && p.Name == cateVal))
            {
                db.MaterialDictionaries.Add(new MaterialDictionary
                {
                    Id = SnowFlake.GetInstance().GetLongId(),
                    Name = cateVal,
                    SortNo = 0,
                    CreateDate = DateTime.Now,
                    Type = MaterialType.种类
                });
            }

            if (!db.MaterialDictionaries.Any(p => p.Type == MaterialType.名称 && p.Name == nameVal))
            {
                db.MaterialDictionaries.Add(new MaterialDictionary
                {
                    Id = SnowFlake.GetInstance().GetLongId(),
                    Name = nameVal,
                    SortNo = 0,
                    CreateDate = DateTime.Now,
                    Type = MaterialType.名称
                });
            }

            _form.ArriveDate = ArriveDate;
            _form.Berth = Berth;
            _form.CarNo = BoatNo;
            _form.CarNetWeight = CarNetWeight.TryGetDecimal();
            //_form.JianJin = JianJin.TryGetDecimal();
            _form.MaterialCategory = cateVal;
            _form.MaterialName = nameVal;
            _form.OrderNetWeight = OrderNetWeight.TryGetDecimal();
            _form.ReceiverName = ReceiverName;
            _form.SenderName = SenderName;
            _form.ShipEndDate = ShipEndDate;
            _form.ShipStartDate = ShipStartDate;

            if (_form.Id == 0)
            {

                _form.Id = SnowFlake.GetInstance().GetLongId();
                _form.CreateDate = DateTime.Now;
                db.ShippingRecords.Add(_form);
            }
            else
            {
                db.Entry(_form).State = EntityState.Modified;
            }

            Loading = true;

            await db.SaveChangesAsync();

            Loading = false;

            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));

            if (string.IsNullOrWhiteSpace(_fromRegionName))
            {
                _fromRegionName = nameof(ShippingRecordForm);
            }
            _regionMgr.RequestNavigate(RegionNames.Main, _fromRegionName);
        }

        bool CanExecuteSaveCmd()
        {
            return !HasErrors;
        }

        private DbService db;
        private IRegionManager _regionMgr;
        private AJGlobalExceptionResolvedEvent _globalExceptionEvt;

        public ShippingRecordFormViewModel(DbService dbIns, IContainerProvider containerProvider, 
            IEventAggregator eventAggregator)
        {
            _regionMgr = containerProvider.Resolve<IRegionManager>();
            _globalExceptionEvt = eventAggregator.GetEvent<AJGlobalExceptionResolvedEvent>();
            _globalExceptionEvt.Subscribe(() =>
            {
                Loading = false;
            });
            _form = new ShippingRecord();
            Chart = new ShippingRecordChart();
            MaterialCategoryOptions = new ObservableCollection<string>();
            MaterialNameOptions = new ObservableCollection<string>();
            InitForm(false);
            db = dbIns;
            db.Database.EnsureCreated();
        }

        private void InitForm(bool update)
        {
            if (update)
            {
                ShipStartDate = _form.ShipStartDate;
                ShipEndDate = _form.ShipEndDate;
                ArriveDate = _form.ArriveDate;
                BoatNo = _form.CarNo;
                MaterialCategory = _form.MaterialCategory;
                MaterialName = _form.MaterialName;
                OrderNetWeight = _form.OrderNetWeight.ToString();
                CarNetWeight = _form.CarNetWeight.ToString();
                SenderName = _form.SenderName;
                ReceiverName = _form.ReceiverName;
                Berth = _form.Berth;
                //JianJin = _form.JianJin.ToString();
                return;
            }
            ShipStartDate = ShipEndDate = ArriveDate = DateTime.Now;
            BoatNo = null;
            MaterialCategory = null;
            MaterialName = null;
            OrderNetWeight = null;
            CarNetWeight = null;
            SenderName = null;
            ReceiverName = null;
            Berth = null;
            JianJin = null;
        }

        public string Title => string.Empty;

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            //parameters.TryGetValue<Services.EF.Tables.Employee>("data", out var data);
            //if (data != null)
            //{

            //    _form = data.DeepClone();
            //    // 重要,把传入的 Detach, 把副本 Attach, 否则后续更改会报错
            //    db.Entry(data).State = EntityState.Detached;
            //    db.Employees.Attach(_form);

            //    Birthday = _form.Birthday;
            //    CompanyName = _form.CompanyName;
            //    Contact = _form.Contact;
            //    Gender = _form.Gender;
            //    IdCardNo = _form.IdCardNo;
            //    Name = _form.Name;
            //    ProjectName = _form.ProjectName;
            //    WorkCertNo = _form.WorkCertNo;
            //    WorkType = _form.WorkType;

            //    var snImages = CommonUtil.TryGetJSONObject<string[]>(_form.SafetyNoticeImageUrl) ?? Array.Empty<string>();
            //    SafetyNoticeImageUrl = new ObservableCollection<UploadFileItem>(snImages.Select(p=>new UploadFileItem
            //    {
            //        Url = p,
            //        Image = CommonUtil.GetImageFromFile(p)
            //    }));

            //    var setImages = CommonUtil.TryGetJSONObject<string[]>(_form.SafetyETImageUrl) 
            //        ?? Array.Empty<string>();
            //    SafetyETImageUrl = new ObservableCollection<UploadFileItem>(setImages.Select(p => new UploadFileItem
            //    {
            //        Url = p,
            //        Image = CommonUtil.GetImageFromFile(p)
            //    }));

            //    var saImages = CommonUtil.TryGetJSONObject<string[]>(_form.SalaryAgreementImageUrl) 
            //        ?? Array.Empty<string>();
            //    SalaryAgreementImageUrl = new ObservableCollection<UploadFileItem>(saImages
            //        .Select(p => new UploadFileItem
            //    {
            //        Url = p,
            //        Image = CommonUtil.GetImageFromFile(p)
            //    }));

            //}
            //DialogTitle = data == null ? "新增人员" : $"编辑 {data.Name}";

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            navigationContext.Parameters.TryGetValue<ShippingRecord>("data", out var data);
            navigationContext.Parameters.TryGetValue<string>("from", out _fromRegionName);

            if (data != null)
            {
                _form = data.DeepClone();
                // 重要,把传入的 Detach, 把副本 Attach, 否则后续更改会报错
                db.Entry(data).State = EntityState.Detached;
                db.ShippingRecords.Attach(_form);

                InitForm(true);
            }

            ExecutePrepareCmd();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
    }

    public class ShippingRecordChart : BindableBase
    {
        private static readonly SolidColorPaint _zhCNPaint = new SolidColorPaint
        {
            SKTypeface = SKFontManager.Default.MatchCharacter('汉'),
            //SKTypeface = SKTypeface.FromFamilyName("Microsoft YaHei UI")
            //FontFamily = "Microsoft YaHei UI",
            Color = SKColor.Parse("#40a9ff"),
        };

        private ISeries[] _series;
        public ISeries[] Series
        {
            get { return _series; }
            set { SetProperty(ref _series, value); }
        }

        private Axis[] _xAxes;
        public Axis[] XAxes
        {
            get { return _xAxes; }
            set { SetProperty(ref _xAxes, value); }
        }

        private Axis[] _yAxes;
        public Axis[] YAxes
        {
            get { return _yAxes; }
            set { SetProperty(ref _yAxes, value); }
        }

        private Visibility _visibility;
        public Visibility Visibility
        {
            get { return _visibility; }
            set { SetProperty(ref _visibility, value); }
        }

        private LabelVisual _title;
        public LabelVisual Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ShippingRecordChart()
        {
            Visibility = Visibility.Hidden;
            var now = DateTime.Now;
            var startDay = now.AddDays(-6);
            var labels = new string[7];
            for (int i = 0; i < 7; i++)
            {
                labels[i] = startDay.AddDays(i).ToString("yyyy-MM-dd");
            }
            XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = labels,
                    Name = "录入时间",
                    NameTextSize = 12,
                    NamePaint = _zhCNPaint
                }
            };
            YAxes = new Axis[]
            {
                new Axis
                {
                    MinStep = 1,
                    Name = "数量",
                    NameTextSize = 14,
                    NamePaint = _zhCNPaint
                }
            };
            Title = new LabelVisual
            {
                Text = "7日内记录统计",
                TextSize = 14,
                Paint = _zhCNPaint
            };
        }

    }
}

