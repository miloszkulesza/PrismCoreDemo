using Presentation.Services;
using Prism.Navigation.Regions;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Controls.Ribbon;
using System.Windows.Threading;

namespace Presentation.RegionAdapters
{
    public class RibbonRegionAdapter : RegionAdapterBase<Ribbon>
    {
        private readonly IWorkspaceService workspace;
        public RibbonRegionAdapter(IRegionBehaviorFactory behaviorFactory, IWorkspaceService workspace)
            : base(behaviorFactory) => this.workspace = workspace;

        protected override void Adapt(IRegion region, Ribbon ribbon)
        {
            bool synchronizing = false;
            void SynchronizeSelection()
            {
                if (synchronizing) return;
                synchronizing = true;
                try
                {
                    var active = region.ActiveViews.FirstOrDefault();
                    ribbon.SelectedItem = active;
                    if (active is RibbonTab tab) tab.SetCurrentValue(RibbonTab.IsSelectedProperty, true);
                }
                finally { synchronizing = false; }
                if (ribbon.SelectedItem is RibbonTab selected) workspace.Activate(selected.DataContext);
            }
            region.Views.CollectionChanged += (_, e) =>
            {
                synchronizing = true;
                try
                {
                    if (e.OldItems != null)
                        foreach (var item in e.OldItems) ribbon.Items.Remove(item);
                    if (e.NewItems != null)
                        foreach (var item in e.NewItems) ribbon.Items.Add(item);
                    if (e.Action == NotifyCollectionChangedAction.Reset)
                    {
                        ribbon.Items.Clear();
                        foreach (var view in region.Views) ribbon.Items.Add(view);
                    }
                }
                finally { synchronizing = false; }

                if (!region.ActiveViews.Any() && ribbon.SelectedItem != null && region.Views.Contains(ribbon.SelectedItem))
                    region.Activate(ribbon.SelectedItem);
                SynchronizeSelection();
            };
            region.ActiveViews.CollectionChanged += (_, _) =>
            {
                SynchronizeSelection();
                // Contextual tabs may need their item containers generated before Ribbon can select them.
                ribbon.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(SynchronizeSelection));
            };
            ribbon.SelectionChanged += (_, e) =>
            {
                if (synchronizing || !ReferenceEquals(e.OriginalSource, ribbon)) return;
                var selected = ribbon.SelectedItem;
                if (selected == null || !region.Views.Contains(selected)) return;
                region.Activate(selected);
                if (selected is RibbonTab tab) workspace.Activate(tab.DataContext);
            };
        }
        protected override IRegion CreateRegion() => new SingleActiveRegion();
    }
}
