using EmployeesModule.ViewModels;
using EmployeesModule.Views;
using Infrastructure.Consts;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Unity;
using Unity.Lifetime;

namespace EmployeesModule
{
    public class EmployeesModule : IModule
    {
        private IUnityContainer unityContainer;

        public EmployeesModule(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            unityContainer.RegisterType<EmployeesListViewModel>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<EmployeeEditViewModel>(new ContainerControlledLifetimeManager());
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            ViewModelLocationProvider.Register<EmployeesListRibbonTab, EmployeesListViewModel>();
            ViewModelLocationProvider.Register<EmployeeEditRibbonTab, EmployeeEditViewModel>();
            containerRegistry.RegisterForNavigation(typeof(EmployeesListRibbonTab), "EmployeesListRibbonTab");
            containerRegistry.RegisterForNavigation(typeof(EmployeesList), "EmployeesList");
            containerRegistry.RegisterForNavigation(typeof(EmployeeEditRibbonTab), "EmployeeEditRibbonTab");
            containerRegistry.RegisterForNavigation(typeof(EmployeeEdit), "EmployeeEdit");
        }
    }
}