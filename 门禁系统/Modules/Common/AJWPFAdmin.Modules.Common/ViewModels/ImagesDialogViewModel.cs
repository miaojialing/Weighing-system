using AJWPFAdmin.Core.Components.AJFilePicker.ViewModels;
using AJWPFAdmin.Core.Mvvm;
using AJWPFAdmin.Core.Utils;
using AJWPFAdmin.Core.Validation;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Modules.Common.ViewModels
{
    public class ImagesDialogViewModel: ViewModelBase, IDialogAware
    {
        private string _title = "查看图片";
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
            }
        }

        private ObservableCollection<UploadFileItem> _images;
        public ObservableCollection<UploadFileItem> Images
        {
            get { return _images; }
            set { SetProperty(ref _images, value); }
        }

        public event Action<IDialogResult> RequestClose;
        private IDialogService _dialogService;

        public ImagesDialogViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var files = parameters.GetValue<string[]>("data");

            Images = new ObservableCollection<UploadFileItem>(files.Select(p => new UploadFileItem(p)).ToList());
        }
    }
}
