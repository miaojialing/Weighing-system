using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Core.Models
{
    public class SysMenu : BindableBase
    {

        public string Name { get; set; }
        public string Icon { get; set; }
        public string Region { get; set; }
        public object Parameter { get; set; }

        private bool _checked;
        public bool Checked
        {
            get { return _checked; }
            set
            {
                SetProperty(ref _checked, value);
            }
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                SetProperty(ref _isExpanded, value);
            }
        }
        public ObservableCollection<SysMenu> Children { get; set; }

        public SysMenu()
        {
            Icon = "";
            Children = new ObservableCollection<SysMenu>();
        }


    }
}
