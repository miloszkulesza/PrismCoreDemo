using TasksModule.ViewModels;
using TasksModule.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;

namespace TasksModule
{
    public class TasksModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider) { }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // The list has one instance; editors below each own their draft.
            containerRegistry.RegisterSingleton<TasksListViewModel>();
            ViewModelLocationProvider.Register<TasksList, TasksListViewModel>();
            containerRegistry.RegisterForNavigation<TasksList>("TasksList");
            containerRegistry.Register<object, TasksListRibbonTab>("TasksListRibbonTab");
            // Each editor shares its instance with its own Ribbon through the workspace.
            containerRegistry.Register<TaskEditViewModel>();
            ViewModelLocationProvider.Register<TaskEdit, TaskEditViewModel>();
            containerRegistry.RegisterForNavigation<TaskEdit>("TaskEdit");
            containerRegistry.Register<object, TaskEditRibbonTab>("TaskEditRibbonTab");
        }
    }
}
