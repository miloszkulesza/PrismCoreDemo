using EmployeesModule.ViewModels;
using EmployeesModule.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;

namespace EmployeesModule
{
    public class EmployeesModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider) { }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // One screen of each kind; the form and its Ribbon share that screen instance.
            containerRegistry.RegisterSingleton<EmployeesListViewModel>();
            ViewModelLocationProvider.Register<EmployeesList, EmployeesListViewModel>();
            containerRegistry.RegisterForNavigation<EmployeesList>("EmployeesList");
            containerRegistry.Register<object, EmployeesListRibbonTab>("EmployeesListRibbonTab");
            // Each employee editor owns its draft; its Ribbon receives the same instance from the workspace.
            containerRegistry.Register<EmployeeEditViewModel>();
            ViewModelLocationProvider.Register<EmployeeEdit, EmployeeEditViewModel>();
            containerRegistry.RegisterForNavigation<EmployeeEdit>("EmployeeEdit");
            containerRegistry.Register<object, EmployeeEditRibbonTab>("EmployeeEditRibbonTab");
        }
    }
}
