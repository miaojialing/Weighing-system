using AJWPFAdmin.Core.Components.AJFilePicker.Views;
using AJWPFAdmin.Core.Mvvm;
using AJWPFAdmin.Modules.Common.Views;
using AJWPFAdmin.Services.Models;
using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntranceGuardManager.Modules.Main.ViewModels
{
    public class WarehousePassageRecordDetailDialogViewModel : ViewModelBase
    {
        private WarehousePassageRecordsGroupRecord _record;

        public WarehousePassageRecordsGroupRecord Record
        {
            get { return _record; }
            set
            {
                SetProperty(ref _record, value);
            }
        }

        private IDialogService _dialogSvc;
        public WarehousePassageRecordDetailDialogViewModel(WarehousePassageRecordsGroupRecord record, 
            IDialogService dialogSvc)
        {
            Record = record;
            _dialogSvc = dialogSvc;
        }


        private DelegateCommand<string[]> _openImgPreviewDialogCmd;
        public DelegateCommand<string[]> OpenImgPreviewDialogCmd =>
            _openImgPreviewDialogCmd ?? (_openImgPreviewDialogCmd = new DelegateCommand<string[]>(ExecuteOpenImgPreviewDialogCmd));

        void ExecuteOpenImgPreviewDialogCmd(string[] parameter)
        {
            var @params = new DialogParameters
            {
                { "data", parameter }
            };
            _dialogSvc.ShowDialog(nameof(ImagesDialog), @params, (r) => { });
        }
    }
}
