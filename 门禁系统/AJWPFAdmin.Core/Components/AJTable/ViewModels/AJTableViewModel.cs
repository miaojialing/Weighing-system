using AJWPFAdmin.Core.ExtensionMethods;
using AJWPFAdmin.Core.Mvvm;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AJWPFAdmin.Core.Components.AJTable.ViewModels
{
    public class AJTableViewModel : ViewModelBase
    {
        public AJTableViewModel()
        {

        }
    }

    public class AJTableColumnItem
    {
        public string DataIndex { get; set; }
        public string ExtraDataIndex { get; set; }
        public string Title { get; set; }
        public DataGridLength Width { get; set; } = DataGridLength.Auto;
        public double? MinWidth { get; set; }
        public double? MaxWidth { get; set; }

        public AJTableColumnFormatter Formatter { get; set; }

        public AJTableCustomTemplate CustomTemplate { get; set; }

        internal DataGridColumn CreateColumn()
        {
            if (CustomTemplate != null)
            {
                return CreateTemplateColumn();
            }

            return CreateTextColumn();
        }

        private DataGridColumn CreateTemplateColumn()
        {
            var col = new DataGridTemplateColumn
            {
                Header = Title,
                Width = Width,
            };
            if (MinWidth.HasValue)
            {
                col.MinWidth = MinWidth.Value;
            }
            if (MaxWidth.HasValue)
            {
                col.MaxWidth = MaxWidth.Value;
            }

            return col;
        }

        private DataGridColumn CreateTextColumn()
        {

            var col = new DataGridTextColumn
            {
                Header = Title,
                Width = Width,
                //ElementStyle = styleCenter
            };
            if (MinWidth.HasValue)
            {
                col.MinWidth = MinWidth.Value;
            }
            if (MaxWidth.HasValue)
            {
                col.MaxWidth = MaxWidth.Value;
            }
            return col;
        }
    }

    public class AJTableColumnFormatter
    {
        public Func<object, string> Handler { get; set; }
    }

    public class AJTableCustomTemplate
    {
        public string Key { get; set; }
    }

    public class AJTableColumnConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is AJTableColumnFormatter formatter)
            {
                return formatter?.Handler(value) ?? string.Empty;
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class AJTablePageItem : BindableBase
    {
        private string _pageNumber;
        private bool _checked;
        private string _toolTip;

        public string PageNumber { get { return _pageNumber; } set { SetProperty(ref _pageNumber, value); } }
        public bool Checked { get { return _checked; } set { SetProperty(ref _checked, value); } }
        public string ToolTip { get { return _toolTip; } set { SetProperty(ref _toolTip, value); } }

    }

    /// <summary>
    /// 分页配置模型
    /// </summary>
    public class AJTablePagination : BindableBase
    {
        private int _total;
        private int _current;
        private int _pageSize;
        private int[] _pageSizeOptions;

        private HorizontalAlignment _hAlign;

        private ObservableCollection<AJTablePageItem> _pageItems;

        private bool _canPrevPage;
        private bool _canNextPage;

        private int _totalPages;

        /// <summary>
        /// 数据总数
        /// </summary>
        public int Total
        {
            get { return _total; }
            set
            {
                if (SetProperty(ref _total, value))
                {
                    CalculatePageItems();
                }
            }
        }
        /// <summary>
        /// 当前页,默认1
        /// </summary>
        public int Current
        {
            get { return _current; }
            set
            {
                if (SetProperty(ref _current, value))
                {
                    CalculatePageItems();
                }
            }
        }
        /// <summary>
        /// 每页条数, 默认10
        /// </summary>
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                if (SetProperty(ref _pageSize, value))
                {
                    CalculatePageItems();
                }
            }
        }
        /// <summary>
        /// 指定每页可以显示多少条,选项下拉框
        /// </summary>
        public int[] PageSizeOptions
        {
            get { return _pageSizeOptions; }
            set
            {
                if (SetProperty(ref _pageSizeOptions, value))
                {
                    var firstItem = _pageSizeOptions.FirstOrDefault();
                    if (firstItem != 0)
                    {
                        PageSize = firstItem;
                    }
                }
            }
        }

        /// <summary>
        /// 位置对齐,默认居右
        /// </summary>
        public HorizontalAlignment Alignment { get { return _hAlign; } set { SetProperty(ref _hAlign, value); } }

        /// <summary>
        /// 是否能上一页
        /// </summary>
        public bool CanPrevPage { get { return _canPrevPage; } set { SetProperty(ref _canPrevPage, value); } }

        /// <summary>
        /// 是否能上下一页
        /// </summary>
        public bool CanNextPage { get { return _canNextPage; } set { SetProperty(ref _canNextPage, value); } }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get { return _totalPages; } }

        /// <summary>
        /// 分页器页码按钮条目数组 如: 1,2,3
        /// </summary>
        public ObservableCollection<AJTablePageItem> PageItems
        {
            get { return _pageItems; }
            set { SetProperty(ref _pageItems, value); }
        }

        public AJTablePagination()
        {
            Current = 1;
            PageSize = 10;
            PageSizeOptions = new int[] { 10, 50, 100, 500, 1000 };
            Alignment = HorizontalAlignment.Center;
            CalculatePageItems();
        }

        private void CalculatePageItems()
        {
            var items = new List<AJTablePageItem>();

            if (Total == 0)
            {
                if (Current != 1)
                {
                    Current = 1;
                    return;
                }
                CanPrevPage = false;
                CanNextPage = false;
                PageItems = new ObservableCollection<AJTablePageItem>(items);
                return;
            }

            _totalPages = Convert.ToInt32(Math.Floor(((double)Total - 1) / PageSize) + 1);

            if (Current > _totalPages)
            {
                Current = 1;
            }

            CanPrevPage = Current > 1;
            CanNextPage = Current < _totalPages;

            var pageBufferSize = 2;

            if (_totalPages <= 5 + pageBufferSize * 2)
            {
                for (var i = 1; i <= _totalPages; i++)
                {
                    items.Add(new AJTablePageItem
                    {
                        PageNumber = i.ToString(),
                        Checked = Current == i
                    });
                }
            }
            else
            {
                var jumpPrev = new AJTablePageItem
                {
                    PageNumber = "···",
                    ToolTip = "向前5页"
                };
                var jumpNext = new AJTablePageItem
                {
                    PageNumber = "···",
                    ToolTip = "向后5页"
                };

                var lastPager = new AJTablePageItem
                {
                    PageNumber = _totalPages.ToString()
                };
                var firstPager = new AJTablePageItem
                {
                    PageNumber = "1"
                };

                var left = new int[] { 1, Current - pageBufferSize }.Max();
                var right = new int[] { Current + pageBufferSize, _totalPages }.Min();

                if (Current - 1 <= pageBufferSize)
                {
                    right = 1 + pageBufferSize * 2;
                }

                if (_totalPages - Current <= pageBufferSize)
                {
                    left = _totalPages - pageBufferSize * 2;
                }

                for (int i = left; i <= right; i++)
                {
                    items.Add(new AJTablePageItem
                    {
                        PageNumber = i.ToString(),
                        Checked = i == Current
                    });
                }

                if (Current - 1 >= pageBufferSize * 2 && Current != 1 + 2)
                {
                    items[0].Checked = false;
                    items[0].PageNumber = left.ToString();
                    items.Insert(0, jumpPrev);
                }

                if (_totalPages - Current >= pageBufferSize * 2 && Current != _totalPages - 2)
                {
                    items[items.Count - 1].Checked = false;
                    items[items.Count - 1].PageNumber = right.ToString();
                    items.Add(jumpNext);
                }

                if (left != 1)
                {
                    items.Insert(0, firstPager);
                }
                if (right != _totalPages)
                {
                    items.Add(lastPager);
                }
            }
            PageItems = new ObservableCollection<AJTablePageItem>(items);
        }

    }

    /// <summary>
    /// 筛选表单配置模型
    /// </summary>
    public class AJTableSearchFormConfig : BindableBase
    {
        private Visibility _exportVisibility = Visibility.Collapsed;

        public Visibility ExportVisibility
        {
            get { return _exportVisibility; }
            set
            {
                SetProperty(ref _exportVisibility, value);
            }
        }

        private Visibility _advFilterVisibility = Visibility.Visible;

        public Visibility AdvFilterVisibility
        {
            get { return _advFilterVisibility; }
            set
            {
                SetProperty(ref _advFilterVisibility, value);
            }
        }

        private ObservableCollection<AJTableSearchFormSchema> _schemas;

        /// <summary>
        /// 表单项目列表
        /// </summary>
        public ObservableCollection<AJTableSearchFormSchema> Schemas
        {
            get { return _schemas; }
            set
            {
                SetProperty(ref _schemas, value);
            }
        }

        public List<AJTableSearchFormSchema> PopularSchemas
        {
            get
            {
                return _schemas.Where(p => p.IsPopular).ToList();
            }
        }

        public void Reset()
        {
            foreach (var item in Schemas)
            {
                switch (item.Type)
                {
                    case AJTableSchemaType.Input:
                    case AJTableSchemaType.Select:
                    case AJTableSchemaType.DateTimePicker:
                    case AJTableSchemaType.CheckBox:
                    case AJTableSchemaType.RadioBox:
                    case AJTableSchemaType.Switch:
                        item.Value = null;
                        break;
                    case AJTableSchemaType.RangePicker:
                        item.Value = new ObservableCollection<DateTime?>(new DateTime?[] { null, null });
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public class AJTableSearchFormSchema : BindableBase
    {
        private string _label;
        /// <summary>
        /// 表单标签
        /// </summary>
        public string Label
        {
            get { return _label; }
            set { SetProperty(ref _label, value); }
        }

        private string[] _labels;
        /// <summary>
        /// 表单标签,日期范围选择使用
        /// </summary>
        public string[] Labels
        {
            get { return _labels; }
            set { SetProperty(ref _labels, value); }
        }

        private string _field;
        /// <summary>
        /// 表单字段名称
        /// </summary>
        public string Field
        {
            get { return _field; }
            set { SetProperty(ref _field, value); }
        }

        private string _placeholder;
        /// <summary>
        /// 占位提示内容
        /// </summary>
        public string Placeholder
        {
            get { return _placeholder; }
            set { SetProperty(ref _placeholder, value); }
        }

        private object _value;
        /// <summary>
        /// 表单值
        /// </summary>
        public object Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        private ObservableCollection<AJTableFormSchemaItemOptions> _options;
        /// <summary>
        /// 表单选项, 针对 Select,Checkbox,RadioBox 有效
        /// </summary>
        public ObservableCollection<AJTableFormSchemaItemOptions> Options
        {
            get { return _options; }
            set { SetProperty(ref _options, value); }
        }

        private AJTableSchemaType _type;
        /// <summary>
        /// 表单类型
        /// </summary>
        public AJTableSchemaType Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }

        private bool _isPopular;
        /// <summary>
        /// 是否常用, 常用会直接显示, 否则会隐藏到高级筛选中
        /// </summary>
        public bool IsPopular
        {
            get { return _isPopular; }
            set { SetProperty(ref _isPopular, value); }
        }

    }

    /// <summary>
    /// 表单项目的选项条目
    /// </summary>
    public class AJTableFormSchemaItemOptions
    {
        public string Label { get; set; }
        public object Value { get; set; }
        public object OtherData { get; set; }
    }

    public class AJTableFormSchemaItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Input { get; set; }
        public DataTemplate Select { get; set; }
        public DataTemplate DateTimePicker { get; set; }
        public DataTemplate RangePicker { get; set; }
        public DataTemplate CheckBox { get; set; }
        public DataTemplate RadioBox { get; set; }
        public DataTemplate Switch { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is AJTableSearchFormSchema schema)
            {
                switch (schema.Type)
                {
                    case AJTableSchemaType.Input:
                        return Input;
                    case AJTableSchemaType.Select:
                        return Select;
                    case AJTableSchemaType.DateTimePicker:
                        return DateTimePicker;
                    case AJTableSchemaType.RangePicker:
                        return RangePicker;
                    case AJTableSchemaType.CheckBox:
                        return CheckBox;
                    case AJTableSchemaType.RadioBox:
                        return RadioBox;
                    case AJTableSchemaType.Switch:
                        return Switch;
                    default:
                        break;
                }
            }
            return null;
        }
    }

    public enum AJTableSchemaType : short
    {
        Input,
        Select,
        DateTimePicker,
        RangePicker,
        CheckBox,
        RadioBox,
        Switch
    }

    public class AJExportProgress : BindableBase
    {
        private int _total;
        public int Total
        {
            get { return _total; }
            set { SetProperty(ref _total, value); }
        }

        private int _current;
        public int Current
        {
            get { return _current; }
            set { SetProperty(ref _current, value); }
        }

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
    }
}
