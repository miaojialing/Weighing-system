using AJWPFAdmin.Core.Components.AJFilePicker.Views;
using AJWPFAdmin.Core.Components.AJTable.ViewModels;
using AJWPFAdmin.Core.Mvvm;
using AJWPFAdmin.Core.Utils;
using Masuit.Tools.Systems;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AJWPFAdmin.Core.Components.AJFilePicker.ViewModels
{
    public class AJFilePickerViewModel : ViewModelBase
    {
        private static readonly string _uploadFolder = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "uploads");

        private bool _displayOnly;
        /// <summary>
        /// 是否仅作为展示
        /// </summary>
        public bool DisplayOnly
        {
            get { return _displayOnly; }
            set
            {
                SetProperty(ref _displayOnly, value);
            }
        }

        private Visibility _displayOnlyAndEmtpy;
        public Visibility DisplayOnlyAndEmtpy
        {
            get { return _displayOnlyAndEmtpy; }
            set
            {
                SetProperty(ref _displayOnlyAndEmtpy, value);
                DelBtnVisibility = value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private Visibility _delBtnVisibility;
        public Visibility DelBtnVisibility
        {
            get { return _delBtnVisibility; }
            set { SetProperty(ref _delBtnVisibility, value); }
        }

        private ObservableCollection<UploadFileItem> _images;
        public ObservableCollection<UploadFileItem> Images
        {
            get { return _images; }
            set
            {
                SetProperty(ref _images, value);
                RaiseUploadBtnVisibleChange();
            }
        }

        private UploadConfig _uploadConfig;

        public UploadConfig UploadConfig
        {
            get { return _uploadConfig; }
            set { SetProperty(ref _uploadConfig, value); }
        }

        private Visibility _uploadBtnVisible;
        public Visibility UploadBtnVisible
        {
            get { return _uploadBtnVisible; }
            set { SetProperty(ref _uploadBtnVisible, value); }
        }

        private DelegateCommand<UploadFileItem> _previewImageCmd;
        public DelegateCommand<UploadFileItem> PreviewImageCmd =>
            _previewImageCmd ?? (_previewImageCmd = new DelegateCommand<UploadFileItem>(ExecutePreviewImageCmd));

        void ExecutePreviewImageCmd(UploadFileItem parameter)
        {
            var @params = new DialogParameters
            {
                { "data", parameter.Image },
                { "name", Path.GetFileName( parameter.Url) }
            };
            _dialogSvc.ShowDialog(nameof(ImagePreviewer), @params, (r) => { });
        }

        private DelegateCommand _openDialogCmd;
        public DelegateCommand OpenDialogCmd =>
            _openDialogCmd ?? (_openDialogCmd = new DelegateCommand(ExecuteOpenDialogCmd));

        async void ExecuteOpenDialogCmd()
        {
            var targetDir = Path.Combine(_uploadFolder, UploadConfig.CustomUploadFolder ?? "images");
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            var dialog = new OpenFileDialog
            {
                Filter = UploadConfig.Filters
            };

            if (dialog.ShowDialog().GetValueOrDefault())
            {
                var item = new UploadFileItem
                {
                    Url = dialog.FileName,
                    Loading = Visibility.Visible,
                    Image = CommonUtil.GetImageFromFile(dialog.FileName)
                };

                Images.Add(item);

                RaiseUploadBtnVisibleChange();

                var fName = $"{SnowFlake.GetInstance().GetUniqueShortId()}_{Path.GetFileName(dialog.FileName)}";
                var temp = Path.Combine(targetDir, fName);
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

        private DelegateCommand<UploadFileItem> _deleteCmd;
        public DelegateCommand<UploadFileItem> DeleteCmd =>
            _deleteCmd ?? (_deleteCmd = new DelegateCommand<UploadFileItem>(ExecuteDeleteCmd));

        async void ExecuteDeleteCmd(UploadFileItem item)
        {
            var ret = await CommonUtil.ShowConfirmDialogAsync(new MaterialDesignExtensions.Controls.ConfirmationDialogArguments
            {
                Title = $"确定删除该图片吗?"
            });

            if (ret)
            {
                item.Loading = Visibility.Visible;

                var url = item.Url;

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

                Images.Remove(item);

                item.Loading = Visibility.Collapsed;

                RaiseUploadBtnVisibleChange();
            }
        }

        private void RaiseUploadBtnVisibleChange()
        {
            UploadBtnVisible = !DisplayOnly
                ? Images.Count < UploadConfig.Limit ? Visibility.Visible : Visibility.Collapsed
                : Visibility.Collapsed;

            DisplayOnlyAndEmtpy = DisplayOnly && Images.Count == 0
                ? Visibility.Visible : Visibility.Collapsed;

            DelBtnVisibility = DisplayOnly ? Visibility.Collapsed : Visibility.Visible;
        }

        private IDialogService _dialogSvc;

        public AJFilePickerViewModel(IDialogService dialogSvc)
        {

            UploadConfig = new UploadConfig
            {
                Height = 120,
                Width = 120,
                Text = "上传"
            };
            Images = new ObservableCollection<UploadFileItem>();
            _dialogSvc = dialogSvc;
            DisplayOnlyAndEmtpy = Visibility.Collapsed;
            DelBtnVisibility = Visibility.Visible;
        }
    }

    public class UploadConfig
    {
        public double Height { get; set; }
        public double Width { get; set; }
        public string Text { get; set; }
        public string Filters { get; set; } = "图片文件|*.jpg;*.jpeg;*.png;*.bmp";
        public int Limit { get; set; } = 1;
        public string CustomUploadFolder { get; set; }
    }
    public class UploadFileItem : BindableBase
    {
        private string _url;
        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }

        private Visibility _loading;
        public Visibility Loading
        {
            get { return _loading; }
            set { SetProperty(ref _loading, value); }
        }

        private BitmapImage _image;
        public BitmapImage Image
        {
            get { return _image; }
            set { SetProperty(ref _image, value); }
        }

        public UploadFileItem()
        {
            Loading = Visibility.Collapsed;
        }
    }
}
