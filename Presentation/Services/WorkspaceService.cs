using Presentation.Consts;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Navigation.Regions;
using System.Collections.Generic;


using System.Linq;
using System.Windows;
using System.Windows.Controls.Ribbon;

namespace Presentation.Services
{
    public class WorkspaceService : IWorkspaceService
    {
        private sealed record Screen(string Name, FrameworkElement View, RibbonTab Ribbon);
        private readonly List<Screen> screens = new();
        private readonly IRegionManager regionManager;
        private readonly IContainerProvider container;
        private readonly IUserDialogService dialogs;
        public WorkspaceService(IRegionManager regionManager, IContainerProvider container, IUserDialogService dialogs)
        {
            this.regionManager = regionManager;
            this.container = container;
            this.dialogs = dialogs;
        }

        public void Open(string viewName, string ribbonName, NavigationParameters parameters = null)
        {
            parameters ??= new NavigationParameters();
            var existing = screens.FirstOrDefault(x => x.Name == viewName);
            if (existing?.View.DataContext is IWorkspaceScreen screen && !screen.CanNavigateTo(parameters)) return;

            regionManager.RequestNavigate(RegionNames.ViewRegion, viewName, result =>
            {
                if (result.Success != true)
                {
                    if (result.Exception != null) dialogs.ShowError(result.Exception.Message);
                    return;
                }

                var view = (FrameworkElement)regionManager.Regions[RegionNames.ViewRegion].ActiveViews.Single();
                var pair = screens.FirstOrDefault(x => ReferenceEquals(x.View, view));
                if (pair == null)
                {
                    // The form owns navigation. Its Ribbon shares the context without receiving navigation callbacks.
                    var ribbon = (RibbonTab)container.Resolve<object>(ribbonName);
                    ribbon.DataContext = view.DataContext;
                    pair = new Screen(viewName, view, ribbon);
                    screens.Add(pair);
                    regionManager.Regions[RegionNames.RibbonRegion].Add(ribbon);
                }
                regionManager.Regions[RegionNames.RibbonRegion].Activate(pair.Ribbon);
            }, parameters);
        }

        public void Activate(object screen)
        {
            var pair = screens.FirstOrDefault(x => ReferenceEquals(x.View.DataContext, screen));
            if (pair != null)
            {
                regionManager.Regions[RegionNames.ViewRegion].Activate(pair.View);
                var ribbonRegion = regionManager.Regions[RegionNames.RibbonRegion];
                if (!ribbonRegion.ActiveViews.Contains(pair.Ribbon)) ribbonRegion.Activate(pair.Ribbon);
            }
        }

        public void Close(object screen)
        {
            var pair = screens.FirstOrDefault(x => ReferenceEquals(x.View.DataContext, screen));
            if (pair == null) return;
            var viewRegion = regionManager.Regions[RegionNames.ViewRegion];
            var ribbonRegion = regionManager.Regions[RegionNames.RibbonRegion];
            var wasActive = viewRegion.ActiveViews.Contains(pair.View);
            screens.Remove(pair);
            viewRegion.Remove(pair.View);
            ribbonRegion.Remove(pair.Ribbon);
            (screen as IWorkspaceScreen)?.OnClosed();
            if (wasActive && (screens.LastOrDefault(x => Equals(x.Ribbon.ContextualTabGroupHeader, pair.Ribbon.ContextualTabGroupHeader)) ?? screens.LastOrDefault()) is Screen next)
            {
                viewRegion.Activate(next.View);
                ribbonRegion.Activate(next.Ribbon);
            }
        }
    }
}
