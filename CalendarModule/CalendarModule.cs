using CalendarModule.ViewModels;
using CalendarModule.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;

namespace CalendarModule
{
    public class CalendarModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider) { }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // One screen of each kind; the form and its Ribbon share that screen instance.
            containerRegistry.RegisterSingleton<CalendarViewModel>();
            ViewModelLocationProvider.Register<Calendar, CalendarViewModel>();
            containerRegistry.RegisterForNavigation<Calendar>("Calendar");
            containerRegistry.Register<object, CalendarRibbonTab>("CalendarRibbonTab");
        }
    }
}