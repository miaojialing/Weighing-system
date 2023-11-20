using AJWPFAdmin.Core.Components.AJFilePicker.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AJWPFAdmin.Core.Components.AJFilePicker.Views
{
    /// <summary>
    /// AJFilePicker.xaml 的交互逻辑
    /// </summary>
    public partial class AJFilePicker : UserControl
    {

        public static DependencyProperty DisplayOnlyProperty =
            DependencyProperty.Register(nameof(DisplayOnly), typeof(bool), typeof(AJFilePicker), new PropertyMetadata(DisplayOnlyPropertyChanged));

        public bool DisplayOnly
        {
            get { return (bool)GetValue(DisplayOnlyProperty); }
            set { SetValue(DisplayOnlyProperty, value); }
        }

        private static void DisplayOnlyPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is AJFilePicker picker)
            {
                if (picker.DataContext is AJFilePickerViewModel vm)
                {
                    vm.DisplayOnly = picker.DisplayOnly;
                }
            }
        }


        public static DependencyProperty ImagesProperty =
            DependencyProperty.Register(nameof(Images), typeof(ObservableCollection<UploadFileItem>), typeof(AJFilePicker), new PropertyMetadata(ImagesPropertyChanged));

        public ObservableCollection<UploadFileItem> Images
        {
            get { return (ObservableCollection<UploadFileItem>)GetValue(ImagesProperty); }
            set { SetValue(ImagesProperty, value); }
        }

        private static void ImagesPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is AJFilePicker picker)
            {
                if (picker.DataContext is AJFilePickerViewModel vm
                    && picker.Images != null)
                {
                    vm.Images = picker.Images;
                }
            }
        }


        public static DependencyProperty ImageHeightProperty =
            DependencyProperty.Register(nameof(ImageHeight), typeof(double), typeof(AJFilePicker), new PropertyMetadata(ImageHeightPropertyChanged));

        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        private static void ImageHeightPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is AJFilePicker picker)
            {
                if (picker.DataContext is AJFilePickerViewModel vm)
                {
                    vm.UploadConfig.Height = picker.ImageHeight;
                }
            }
        }

        public static DependencyProperty ImageWidthProperty =
            DependencyProperty.Register(nameof(ImageWidth), typeof(double), typeof(AJFilePicker), new PropertyMetadata(ImageWidthPropertyChanged));

        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        private static void ImageWidthPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is AJFilePicker picker)
            {
                if (picker.DataContext is AJFilePickerViewModel vm)
                {
                    vm.UploadConfig.Width = picker.ImageWidth;
                }
            }
        }

        public static DependencyProperty FiltersProperty =
            DependencyProperty.Register(nameof(Filters), typeof(string), typeof(AJFilePicker), new PropertyMetadata(FiltersPropertyChanged));

        public string Filters
        {
            get { return (string)GetValue(FiltersProperty); }
            set { SetValue(FiltersProperty, value); }
        }

        private static void FiltersPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is AJFilePicker picker)
            {
                if (picker.DataContext is AJFilePickerViewModel vm)
                {
                    vm.UploadConfig.Filters = picker.Filters;
                }
            }
        }

        public static DependencyProperty UploadTextProperty =
            DependencyProperty.Register(nameof(UploadText), typeof(string), typeof(AJFilePicker), new PropertyMetadata(UploadTextPropertyChanged));

        public string UploadText
        {
            get { return (string)GetValue(UploadTextProperty); }
            set { SetValue(UploadTextProperty, value); }
        }

        private static void UploadTextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is AJFilePicker picker)
            {
                if (picker.DataContext is AJFilePickerViewModel vm)
                {
                    vm.UploadConfig.Text = picker.UploadText;
                }
            }
        }

        public static DependencyProperty LimitProperty =
            DependencyProperty.Register(nameof(Limit), typeof(int), typeof(AJFilePicker), new PropertyMetadata(LimitPropertyChanged));

        public int Limit
        {
            get { return (int)GetValue(LimitProperty); }
            set { SetValue(LimitProperty, value); }
        }

        private static void LimitPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is AJFilePicker picker)
            {
                if (picker.DataContext is AJFilePickerViewModel vm)
                {
                    vm.UploadConfig.Limit = picker.Limit;
                }
            }
        }

        public AJFilePicker()
        {
            InitializeComponent();
        }
    }
}
