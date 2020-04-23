using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Controls.Ribbon;

namespace Infrastructure.RegionAdapters
{
    public class RibbonRegionAdapter : RegionAdapterBase<Ribbon>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="behaviorFactory">Allows the registration
        /// of the default set of RegionBehaviors.</param>
        public RibbonRegionAdapter(IRegionBehaviorFactory behaviorFactory)
            : base(behaviorFactory)
        {
        }

        /// <summary>
        /// Adapts a WPF control to serve as a Prism IRegion. 
        /// </summary>
        /// <param name="region">The new region being used.</param>
        /// <param name="regionTarget">The WPF control to adapt.</param>
        protected override void Adapt(IRegion region, Ribbon regionTarget)
        {
            region.Views.CollectionChanged += (sender, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (RibbonTab element in e.NewItems)
                        {
                            regionTarget.Items.Add(element);
                            regionTarget.SelectedItem = element;
                        }
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        foreach (RibbonTab elementLoopVariable in e.OldItems)
                        {
                            var element = elementLoopVariable;
                            if (regionTarget.Items.Contains(element))
                            {
                                regionTarget.Items.Remove(element);
                            }
                        }
                        regionTarget.SelectedIndex = regionTarget.Items.Count - 1;
                        break;
                }
            };
        }

        protected override IRegion CreateRegion()
        {
            return new SingleActiveRegion();
        }
    }
}
