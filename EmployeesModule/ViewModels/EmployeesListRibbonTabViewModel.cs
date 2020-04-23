using EmployeesModule.Views;
using Infrastructure.Consts;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Linq;
using System.Windows.Controls.Ribbon;
using System.Windows.Input;

namespace EmployeesModule.ViewModels
{
    public class EmployeesListRibbonTabViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;

        public DelegateCommand ExitViewCommand { get; set; }

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

        public EmployeesListRibbonTabViewModel(
            IRegionManager regionManager)
        {
            this.regionManager = regionManager;
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            ExitViewCommand = new DelegateCommand(OnExitView);
        }

        private void OnExitView()
        {
            var ribbonTab = regionManager.Regions[RegionNames.RibbonRegion].Views.FirstOrDefault(x => x.GetType() == typeof(EmployeesListRibbonTab));
            var view = regionManager.Regions[RegionNames.ViewRegion].Views.FirstOrDefault(x => x.GetType() == typeof(EmployeesList));
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
                regionManager.Regions[RegionNames.ViewRegion].Activate(regionManager.Regions[RegionNames.ViewRegion].Views.FirstOrDefault(x => x.GetType() == typeof(EmployeesList)));
        }
    }
}
