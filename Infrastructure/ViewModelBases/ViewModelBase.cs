using Infrastructure.Consts;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Infrastructure.ViewModelBases
{
    public class ViewModelBase : BindableBase, INavigationAware, INotifyPropertyChanged
    {
        public readonly IRegionManager regionManager;

        public DelegateCommand CloseViewCommand { get; set; }

        private bool isTabSelected;
        public bool IsTabSelected
        {
            get
            {
                return isTabSelected;
            }
            set
            {
                SetProperty(ref isTabSelected, value);
                TabSelectedChange();
            }
        }

        private Type defaultViewType;
        public Type DefaultViewType
        {
            get { return defaultViewType; }
            set { SetProperty(ref defaultViewType, value); }
        }

        private Type defaultRibbonType;
        public Type DefaultRibbonType
        {
            get { return defaultRibbonType; }
            set { SetProperty(ref defaultRibbonType, value); }
        }

        public ViewModelBase(
            IRegionManager regionManager,
            Type defaultViewType,
            Type defaultRibbonType)
        {
            this.regionManager = regionManager;
            DefaultViewType = defaultViewType;
            DefaultRibbonType = defaultRibbonType;
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            CloseViewCommand = new DelegateCommand(OnCloseView);
        }

        protected virtual void OnCloseView()
        {
            var ribbonTab = regionManager.Regions[RegionNames.RibbonRegion].Views.FirstOrDefault(x => x.GetType() == DefaultRibbonType);
            var view = regionManager.Regions[RegionNames.ViewRegion].Views.FirstOrDefault(x => x.GetType() == DefaultViewType);
            regionManager.Regions[RegionNames.RibbonRegion].Remove(ribbonTab);
            regionManager.Regions[RegionNames.ViewRegion].Remove(view);
            if (regionManager.Regions[RegionNames.ViewRegion].Views.Any())
                regionManager.Regions[RegionNames.ViewRegion].Activate(regionManager.Regions[RegionNames.ViewRegion].Views.LastOrDefault());
        }

        private void TabSelectedChange()
        {
            if (IsTabSelected)
                regionManager.Regions[RegionNames.ViewRegion].Activate(regionManager.Regions[RegionNames.ViewRegion].Views.FirstOrDefault(x => x.GetType() == DefaultViewType));
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
           
        }

        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
           
        }
    }
}
