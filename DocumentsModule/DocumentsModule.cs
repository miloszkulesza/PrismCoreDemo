using DocumentsModule.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace DocumentsModule
{
    public class DocumentsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation(typeof(DocumentsList), "DocumentsList");
            containerRegistry.RegisterForNavigation(typeof(DocumentsListRibbonTab), "DocumentsListRibbonTab");
        }
    }
}