using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AJWPFAdmin.Core.ExtensionMethods;
using AJWPFAdmin.Core.Components.AJTable.ViewModels;

namespace AJWPFAdmin.Core.Components.AJTable.Views
{
    /// <summary>
    /// AJTable.xaml 的交互逻辑
    /// </summary>
    public partial class AJTable : UserControl
    {

        public static DependencyProperty ExportProgressProperty =
            DependencyProperty.Register(nameof(ExportProgress), typeof(AJExportProgress), typeof(AJTable), new PropertyMetadata());

        public AJExportProgress ExportProgress
        {
            get { return (AJExportProgress)GetValue(ExportProgressProperty); }
            set { SetValue(ExportProgressProperty, value); }
        }

        public static DependencyProperty FormConfigProperty =
            DependencyProperty.Register(nameof(FormConfig), typeof(AJTableSearchFormConfig), typeof(AJTable), new PropertyMetadata());

        public AJTableSearchFormConfig FormConfig
        {
            get { return (AJTableSearchFormConfig)GetValue(FormConfigProperty); }
            set { SetValue(FormConfigProperty, value); }
        }


        public static DependencyProperty PaginationProperty =
            DependencyProperty.Register(nameof(Pagination), typeof(AJTablePagination), typeof(AJTable), new PropertyMetadata());

        public AJTablePagination Pagination
        {
            get { return (AJTablePagination)GetValue(PaginationProperty); }
            set { SetValue(PaginationProperty, value); }
        }


        public static DependencyProperty LoadingProperty =
            DependencyProperty.Register(nameof(Loading), typeof(bool), typeof(AJTable), new PropertyMetadata(LoadingPropertyChanged));

        public bool Loading
        {
            get { return (bool)GetValue(LoadingProperty); }
            set { SetValue(LoadingProperty, value); }
        }

        private static void LoadingPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is AJTable dataGrid)
            {
                dataGrid.LoadingPropertyChanged(e);
            }
        }

        private void LoadingPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            DgHst.IsOpen = !DgHst.IsOpen;
        }

        public static DependencyProperty ColumnsProperty =
            DependencyProperty.Register(nameof(Columns), typeof(IEnumerable<AJTableColumnItem>), typeof(AJTable), new PropertyMetadata(ColumnsPropertyChanged));

        public IEnumerable<AJTableColumnItem> Columns
        {
            get { return (IEnumerable<AJTableColumnItem>)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        private static void ColumnsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is AJTable dataGrid)
            {
                dataGrid.ColumnsPropertyChanged(e);
            }
        }

        private void ColumnsPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Columns == null || !Columns.Any())
            {
                return;
            }

            //var styleCenter = new Style(typeof(TextBlock));
            //styleCenter.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center));
            //styleCenter.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center));

            DG_Table.Columns.AddRange(Columns.Select(c => c.CreateColumn()));

        }

        public static DependencyProperty RowsProperty =
            DependencyProperty.Register(nameof(Rows), typeof(IEnumerable<object>), typeof(AJTable), new PropertyMetadata(RowsPropertyChanged));


        public IEnumerable<object> Rows
        {
            get { return (IEnumerable<object>)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }

        private static void RowsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is AJTable dataGrid)
            {
                dataGrid.RowsPropertyChanged(e);
            }
        }

        private void RowsPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            DG_Table.ItemsSource = Rows;
            if (Rows == null || Rows.Count() == 0)
            {
                return;
            }
            UpdateColumnBindings();
        }

        private void UpdateColumnBindings()
        {
            var props = Rows.First().GetType().GetRuntimeProperties();
            var i = 0;

            var styleCenter = new Style(typeof(TextBlock));
            //styleCenter.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center));
            styleCenter.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center));

            foreach (var column in Columns)
            {
                var prop = props
                    .FirstOrDefault(p => p.Name.Equals(column.DataIndex, StringComparison.OrdinalIgnoreCase));

                if (prop != null && !string.IsNullOrWhiteSpace(column.DataIndex))
                {
                    var bindExpStr = column.DataIndex;
                    if (!string.IsNullOrWhiteSpace(column.ExtraDataIndex))
                    {
                        bindExpStr += $".{column.ExtraDataIndex}";
                    }
                    var binding = new Binding(bindExpStr);
                    if (column.Formatter != null)
                    {
                        binding.Converter = new AJTableColumnConverter();
                        binding.ConverterParameter = column.Formatter;
                    }
                    if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                    {
                        binding.StringFormat = "yyyy-MM-dd HH:mm:ss";
                    }

                    if (DG_Table.Columns[i] is DataGridTextColumn col)
                    {
                        col.Binding = binding;
                        col.ElementStyle = styleCenter;
                    }
                }
                if (DG_Table.Columns[i] is DataGridTemplateColumn templateCol)
                {
                    var template = TryFindResource(column.CustomTemplate.Key);
                    if (template != null)
                    {
                        templateCol.CellTemplate = template as DataTemplate;
                    }
                }
                i++;
            }
        }

        #region Events

        public class AJTablePageChangedEventArgs : EventArgs
        {
            public int Page { get; set; }
            public int PageSize { get; set; }
            public AJTablePageChangedEventArgs()
            {

            }
        }

        public event EventHandler<AJTablePageChangedEventArgs> PageChanged;

        public class AJTableSearchClickEventArgs : EventArgs
        {
            public bool IsRefresh { get; set; }
        }

        public event EventHandler<AJTableSearchClickEventArgs> SearchClick;
        public event EventHandler ExportClick;

        public event EventHandler<DataGridRowEventArgs> LoadingRow;

        #endregion

        public AJTable()
        {
            InitializeComponent();
        }

        private void PageItem_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton tb)
            {
                if (tb.Content.Equals("···"))
                {
                    var prev = tb.ToolTip.ToString().Contains("前");
                    var current = prev
                        ? new int[] { 1, Pagination.Current - 5 }.Max()
                        : new int[] { Pagination.Total, Pagination.Current + 5 }.Min();

                    Pagination.Current = current;
                    return;
                }
                Pagination.Current = System.Convert.ToInt32(tb.Content.ToString());
                PageChanged?.Invoke(this, new AJTablePageChangedEventArgs
                {
                    Page = Pagination.Current,
                    PageSize = Pagination.PageSize
                });
            }
        }

        private void PrevOrNextPage_Clicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                var changed = false;
                var prev = btn.ToolTip.ToString().Contains("上");
                if (prev)
                {
                    if (Pagination.CanPrevPage)
                    {
                        Pagination.Current -= 1;
                        changed = true;
                    }
                }
                else
                {
                    if (Pagination.CanNextPage)
                    {
                        Pagination.Current += 1;
                        changed = true;
                    }
                }

                if (changed)
                {
                    PageChanged?.Invoke(this, new AJTablePageChangedEventArgs
                    {
                        Page = Pagination.Current,
                        PageSize = Pagination.PageSize
                    });
                }
            }
        }

        private void PageOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PageChanged?.Invoke(this, new AJTablePageChangedEventArgs
            {
                Page = Pagination.Current,
                PageSize = Pagination.PageSize
            });
        }

        private void JumpToPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox && e.Key == Key.Enter)
            {
                var page = textBox.Text.TryGetInt();
                if (page < 1)
                {
                    page = 1;
                }
                if (page > Pagination.TotalPages)
                {
                    page = Pagination.TotalPages;
                }
                Pagination.Current = page;
                PageChanged?.Invoke(this, new AJTablePageChangedEventArgs
                {
                    Page = Pagination.Current,
                    PageSize = Pagination.PageSize
                });
                textBox.Clear();
            }

        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var refresh = Convert.ToBoolean(((Button)sender).CommandParameter.ToString());
            if (refresh)
            {
                FormConfig.Reset();
            }
            DrawerHst.IsTopDrawerOpen = false;
            SearchClick?.Invoke(sender, new AJTableSearchClickEventArgs { 
                IsRefresh = refresh
            });
            
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            ExportClick?.Invoke(sender, e);
        }

        private void AdvFilterButton_Click(object sender, RoutedEventArgs e)
        {
            DrawerHst.IsTopDrawerOpen = true;
        }

        private void DG_Table_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            LoadingRow?.Invoke(this, e);
        }
    }
}
