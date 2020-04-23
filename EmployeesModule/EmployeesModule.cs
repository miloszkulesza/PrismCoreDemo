using EmployeesModule.ViewModels;
using EmployeesModule.Views;
using Infrastructure.Consts;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Unity;

namespace EmployeesModule
{
    public class EmployeesModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation(typeof(EmployeesListRibbonTab), "EmployeesListRibbonTab");
            containerRegistry.RegisterForNavigation(typeof(EmployeesList), "EmployeesList");
        }
    }
}