using AJWPFAdmin.Core.Components.AJFilePicker.ViewModels;
using AJWPFAdmin.Core.Utils;
using Masuit.Tools.Systems;
using Microsoft.Win32;
using Prism.Ioc;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
            DependencyProperty.Register(nameof(DisplayOnly), typeof(bool), typeof(AJFilePicker), new PropertyMetadata(false, DisplayOnlyPropertyChanged));

        public bool DisplayOnly
        {
            get { return (bool)GetValue(DisplayOnlyProperty); }
            set { SetValue(DisplayOnlyProperty, value); }
        }

        private static void DisplayOnlyPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is AJFilePicker picker)
            {
                picker.UpdateComponent();
            }
        }


        public static DependencyProperty ImagesProperty =
            DependencyProperty.Register(nameof(Images), typeof(ObservableCollection<UploadFileItem>), typeof(AJFilePicker), new PropertyMetadata(new ObservableCollection<UploadFileItem>(), ImagesPropertyChanged));

        public ObservableCollection<UploadFileItem> Images
        {
            get { return (ObservableCollection<UploadFileItem>)GetValue(ImagesProperty); }
            set { SetValue(ImagesProperty, value); }
        }

        private static void ImagesPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is AJFilePicker picker)
            {
                picker.UpdateComponent();
            }
        }


        public static DependencyProperty OptionsProperty =
            DependencyProperty.Register(nameof(Options), typeof(UploadConfig), typeof(AJFilePicker), new PropertyMetadata(UploadConfig.Default, OptionsPropertyChanged));

        public UploadConfig Options
        {
            get { return (UploadConfig)GetValue(OptionsProperty); }
            set { SetValue(OptionsProperty, value); }
        }

        private static void OptionsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is AJFilePicker picker)
            {
                picker.UpdateComponent();
            }
        }

        private static readonly string _uploadFolder = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "uploads");
        private IDialogService _dialogSvc;

        public AJFilePicker()
        {

            InitializeComponent();

            UpdateComponent();
            _dialogSvc = Prism.Ioc.ContainerLocator.Current.Resolve<IDialogService>();
        }

        private void UpdateComponent()
        {
            Btn_Upload.Width = Options.Width;
            Btn_Upload.Height = Options.Height;
            Btn_Upload.Content = Options.Text;
            if (Images?.Count > 0)
            {
                foreach (var item in Images)
                {
                    item.CanDelete = DisplayOnly ? Visibility.Collapsed : Visibility.Visible;
                    item.DisplayHeight = Btn_Upload.Height;
                    item.DisplayWidth = Btn_Upload.Width;
                }
            }

            RaiseUploadBtnVisibleChange();
        }

        private async void Btn_Upload_Click(object sender, RoutedEventArgs e)
        {
            var targetDir = System.IO.Path.Combine(_uploadFolder, Options.CustomUploadFolder ?? "images");
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            var dialog = new OpenFileDialog
            {
                Filter = Options.Filters
            };

            if (dialog.ShowDialog().GetValueOrDefault())
            {
                var item = new UploadFileItem(dialog.FileName)
                {
                    Url = dialog.FileName,
                    DisplayWidth = Options.Width,
                    DisplayHeight = Options.Height,
                };
                item.Loading = Visibility.Visible;
                Images.Add(item);

                RaiseUploadBtnVisibleChange();

                var fName = $"{SnowFlake.GetInstance().GetUniqueShortId()}_{System.IO.Path.GetFileName(dialog.FileName)}";
                var temp = System.IO.Path.Combine(targetDir, fName);
                var success = await Task.Run(() =>
                {
                    return CommonUtil.CopyFile(dialog.FileName, temp);
                });

                item.Loading = Visibility.Collapsed;

                if (success)
                {
                    item.Url = temp;
                }
                else
                {
                    Images.Remove(item);
                    RaiseUploadBtnVisibleChange();
                    await CommonUtil.ShowAlertDialogAsync(new MaterialDesignExtensions.Controls.AlertDialogArguments
                    {
                        Title = "上传失败!",
                        Message = "请确认文件读写权限或文件是否存在",
                        OkButtonLabel = "知道了"
                    });
                }

            }
        }

        private void RaiseUploadBtnVisibleChange()
        {
            Btn_Upload.Visibility = !DisplayOnly
                ? Images.Count < Options.Limit ? Visibility.Visible : Visibility.Collapsed
                : Visibility.Collapsed;

            Border_ItemBorder.Visibility = DisplayOnly && Images.Count == 0
                ? Visibility.Visible : Visibility.Collapsed;

        }

        private async void Btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            var ret = await CommonUtil.ShowConfirmDialogAsync(new MaterialDesignExtensions.Controls.ConfirmationDialogArguments
            {
                Title = $"确定删除该图片吗?"
            });

            if (ret)
            {
                var item = ((Button)sender).DataContext as UploadFileItem;
                item.Loading = Visibility.Visible;

                // 如果删除的是系统破图， 那直接通过逻辑，不要实际删除文件 
                if ("media-empty.png".Equals(System.IO.Path.GetFileName(item.Url)))
                {
                    await Task.Delay(800);
                }
                else
                {
                    var url = item.Url;
                    await Task.Delay(1000);
                    await Task.Run(() =>
                    {
                        try
                        {
                            File.Delete(url);
                        }
                        catch
                        {

                        }
                    });
                }

                Images.Remove(item);

                item.Loading = Visibility.Collapsed;

                RaiseUploadBtnVisibleChange();
            }
        }

        private void Btn_PreView_Click(object sender, RoutedEventArgs e)
        {
            var parameter = ((Button)sender).DataContext as UploadFileItem;
            var @params = new DialogParameters
            {
                { "data", parameter.Image },
                { "name", System.IO.Path.GetFileName( parameter.Url) }
            };
            _dialogSvc.ShowDialog(nameof(ImagePreviewer), @params, (r) => { });
        }
    }
}
