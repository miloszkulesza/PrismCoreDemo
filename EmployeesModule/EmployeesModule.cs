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
           
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            unityContainer.RegisterType<EmployeesListViewModel>(new ContainerControlledLifetimeManager());
            containerRegistry.RegisterForNavigation(typeof(EmployeesListRibbonTab), "EmployeesListRibbonTab");
            containerRegistry.RegisterForNavigation(typeof(EmployeesList), "EmployeesList");
        }
    }
}