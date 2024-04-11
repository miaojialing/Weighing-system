using AJWPFAdmin.Core.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Modules.Main.ViewModels
{
    public class PassagewayStatisticViewModel: ViewModelBase, INavigationAware
    {
        public PassagewayStatisticViewModel()
        {
                
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            
        }
    }
}
