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
using AJWPFAdmin.Core.Mvvm;

namespace AJWPFAdmin.Modules.Main.ViewModels
{
    public class ShippingRecordDetailViewModel : ViewModelBase, IDialogAware
    {

        private ShippingRecord _data;
        public ShippingRecord Data
        {
            get { return _data; }
            set { SetProperty(ref _data, value); }
        }

        private ObservableCollection<UploadFileItem> _entranceIdentifiedCaptureFile;
        public ObservableCollection<UploadFileItem> EntranceIdentifiedCaptureFile
        {
            get { return _entranceIdentifiedCaptureFile; }
            set
            {
                SetProperty(ref _entranceIdentifiedCaptureFile, value);
            }
        }

        private ObservableCollection<UploadFileItem> _entranceCameraCaptureFile;
        public ObservableCollection<UploadFileItem> EntranceCameraCaptureFile
        {
            get { return _entranceCameraCaptureFile; }
            set
            {
                SetProperty(ref _entranceCameraCaptureFile, value);
            }
        }

        private ObservableCollection<UploadFileItem> _exitIdentifiedCaptureFile;
        public ObservableCollection<UploadFileItem> ExitIdentifiedCaptureFile
        {
            get { return _exitIdentifiedCaptureFile; }
            set
            {
                SetProperty(ref _exitIdentifiedCaptureFile, value);
            }
        }

        private ObservableCollection<UploadFileItem> _exitCameraCaptureFile;
        public ObservableCollection<UploadFileItem> ExitCameraCaptureFile
        {
            get { return _exitCameraCaptureFile; }
            set
            {
                SetProperty(ref _exitCameraCaptureFile, value);
            }
        }

        private DbService db;
        private IRegionManager _regionMgr;
        private AJGlobalExceptionResolvedEvent _globalExceptionEvt;

        public ShippingRecordDetailViewModel(DbService dbIns,
            IEventAggregator eventAggregator)
        {
            EntranceCameraCaptureFile = new ObservableCollection<UploadFileItem>();
            EntranceIdentifiedCaptureFile = new ObservableCollection<UploadFileItem>();

            ExitCameraCaptureFile = new ObservableCollection<UploadFileItem>();
            ExitIdentifiedCaptureFile = new ObservableCollection<UploadFileItem>();
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
            parameters.TryGetValue<ShippingRecord>("data", out var data);
            Data = data;

            var entranceIdentImage = (CommonUtil
                .TryGetJSONObject<string[]>(data.EntranceIdentifiedCaptureFile) 
                ?? Array.Empty<string>()).FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(entranceIdentImage))
            {
                EntranceIdentifiedCaptureFile.Add(new UploadFileItem
                {
                    Url = entranceIdentImage,
                    Image = CommonUtil.GetImageFromFile(entranceIdentImage)
                });
            }


            var entranceCameraImage = (CommonUtil
                .TryGetJSONObject<string[]>(data.EntranceCameraCaptureFile)
                ?? Array.Empty<string>()).FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(entranceCameraImage))
            {
                EntranceCameraCaptureFile.Add(new UploadFileItem
                {
                    Url = entranceCameraImage,
                    Image = CommonUtil.GetImageFromFile(entranceCameraImage)
                });
            }

            var exitIdentImage = (CommonUtil
                .TryGetJSONObject<string[]>(data.ExitIdentifiedCaptureFile)
                ?? Array.Empty<string>()).FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(exitIdentImage))
            {
                ExitIdentifiedCaptureFile.Add(new UploadFileItem
                {
                    Url = exitIdentImage,
                    Image = CommonUtil.GetImageFromFile(exitIdentImage)
                });
            }


            var exitCameraImage = (CommonUtil
                .TryGetJSONObject<string[]>(data.ExitCameraCaptureFile)
                ?? Array.Empty<string>()).FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(exitCameraImage))
            {
                ExitCameraCaptureFile.Add(new UploadFileItem
                {
                    Url = exitCameraImage,
                    Image = CommonUtil.GetImageFromFile(exitCameraImage)
                });
            }

        }

    }


}

