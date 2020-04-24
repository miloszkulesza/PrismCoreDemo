using DocumentsModule.ViewModels;
using DocumentsModule.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Unity;
using Unity.Lifetime;

namespace DocumentsModule
{
    public class DocumentsModule : IModule
    {
        private readonly IUnityContainer unityContainer;

        public DocumentsModule(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            unityContainer.RegisterType<DocumentsListViewModel>(new ContainerControlledLifetimeManager());
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            ViewModelLocationProvider.Register<DocumentsListRibbonTab, DocumentsListViewModel>();
            containerRegistry.RegisterForNavigation(typeof(DocumentsList), "DocumentsList");
            containerRegistry.RegisterForNavigation(typeof(DocumentsListRibbonTab), "DocumentsListRibbonTab");
        }
    }
}