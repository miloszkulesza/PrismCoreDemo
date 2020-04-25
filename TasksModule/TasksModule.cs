using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using TasksModule.ViewModels;
using TasksModule.Views;
using Unity;
using Unity.Lifetime;

namespace TasksModule
{
    public class TasksModule : IModule
    {
        private readonly IUnityContainer unityContainer;

        public TasksModule(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            unityContainer.RegisterType<TasksListViewModel>(new ContainerControlledLifetimeManager());
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            ViewModelLocationProvider.Register<TasksListRibbonTab, TasksListViewModel>();
            containerRegistry.RegisterForNavigation(typeof(TasksList), "TasksList");
            containerRegistry.RegisterForNavigation(typeof(TasksListRibbonTab), "TasksListRibbonTab");
        }
    }
}