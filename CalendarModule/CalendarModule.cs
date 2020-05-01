using CalendarModule.ViewModels;
using CalendarModule.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Unity;
using Unity.Lifetime;

namespace CalendarModule
{
    public class CalendarModule : IModule
    {
        private IUnityContainer unityContainer;

        public CalendarModule(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            unityContainer.RegisterType<CalendarViewModel>(new ContainerControlledLifetimeManager());
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            ViewModelLocationProvider.Register<CalendarRibbonTab, CalendarViewModel>();
            containerRegistry.RegisterForNavigation(typeof(Calendar), "Calendar");
            containerRegistry.RegisterForNavigation(typeof(CalendarRibbonTab), "CalendarRibbonTab");
        }
    }
}