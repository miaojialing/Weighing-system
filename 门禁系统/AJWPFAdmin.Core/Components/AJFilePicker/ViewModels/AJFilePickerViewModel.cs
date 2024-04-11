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

    public class UploadConfig
    {
        public double Height { get; set; }
        public double Width { get; set; }
        public string Text { get; set; }
        public string Filters { get; set; } = "图片文件|*.jpg;*.jpeg;*.png;*.bmp";
        public int Limit { get; set; } = 1;
        public string CustomUploadFolder { get; set; }

        public static UploadConfig Default
        {
            get
            {
                return new UploadConfig
                {
                    Height = 120,
                    Width = 120,
                    Text = "上传"
                };
            }
        }
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

        private Visibility _canDelete;
        public Visibility CanDelete
        {
            get { return _canDelete; }
            set { SetProperty(ref _canDelete, value); }
        }

        private BitmapImage _image;
        public BitmapImage Image
        {
            get { return _image; }
            set { SetProperty(ref _image, value); }
        }

        private double _displayHeight;
        public double DisplayHeight
        {
            get { return _displayHeight; }
            set { SetProperty(ref _displayHeight, value); }
        }

        private double _displayWidth;
        public double DisplayWidth
        {
            get { return _displayWidth; }
            set { SetProperty(ref _displayWidth, value); }
        }

        public UploadFileItem(string url)
        {
            Loading = Visibility.Collapsed;
            Url = url;
            if (!string.IsNullOrWhiteSpace(_url))
            {
                Image = CommonUtil.GetImageFromLocalOrHttp(_url);
            }
        }
    }
}
