using Infrastructure.Consts;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeesModule.ViewModels
{
    public class EmployeesListViewModel : BindableBase, INavigationAware
    {
        public string Message { get; set; } = "Moduł pracownicy, widok lista pracowników";

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }
    }
}
