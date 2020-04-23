using DocumentsModule.Views;
using Infrastructure.Consts;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Ribbon;

namespace DocumentsModule.ViewModels
{
    public class DocumentsListRibbonTabViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;

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

        public DelegateCommand ExitViewCommand { get; set; }

        public DocumentsListRibbonTabViewModel(
            IRegionManager regionManager)
        {
            this.regionManager = regionManager;
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            ExitViewCommand = new DelegateCommand(OnExitViewCommand);
        }

        private void OnExitViewCommand()
        {
            var ribbonTab = regionManager.Regions[RegionNames.RibbonRegion].Views.FirstOrDefault(x => x.GetType() == typeof(DocumentsListRibbonTab));
            var view = regionManager.Regions[RegionNames.ViewRegion].Views.FirstOrDefault(x => x.GetType() == typeof(DocumentsList));
            regionManager.Regions[RegionNames.RibbonRegion].Remove(ribbonTab);
            regionManager.Regions[RegionNames.ViewRegion].Remove(view);
            RibbonTab selectedRibbon = regionManager.Regions[RegionNames.RibbonRegion].Views.LastOrDefault() as RibbonTab;
            selectedRibbon.IsSelected = true;
            if (regionManager.Regions[RegionNames.ViewRegion].Views.Any())
                regionManager.Regions[RegionNames.ViewRegion].Activate(regionManager.Regions[RegionNames.ViewRegion].Views.LastOrDefault());
        }

        private void TabSelectedChange()
        {
            if (IsTabSelected)
                regionManager.Regions[RegionNames.ViewRegion].Activate(regionManager.Regions[RegionNames.ViewRegion].Views.FirstOrDefault(x => x.GetType() == typeof(DocumentsList)));
        }
    }
}
