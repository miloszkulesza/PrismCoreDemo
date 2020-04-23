using EmployeesModule.Views;
using Infrastructure.Consts;
using Infrastructure.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls.Ribbon;
using System.Windows.Input;
using Unity;

namespace EmployeesModule.ViewModels
{
    public class EmployeesListViewModel : BindableBase
    {
        private readonly IUnityContainer unityContainer;
        private readonly IRegionManager regionManager;

        public ICommand SelectionChangedCommand { get; set; }
        public DelegateCommand ExitViewCommand { get; set; }
        public DelegateCommand RemoveEmployeeCommand { get; set; }

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

        private bool deleteButtonState;
        public bool DeleteButtonState
        {
            get
            {
                return deleteButtonState;
            }
            set
            {
                SetProperty(ref deleteButtonState, value);
            }
        }

        public static ObservableCollection<Employee> Employees { get; set; } = new ObservableCollection<Employee>
        {
            new Employee
            {
                Id = 1,
                FirstName = "Jan",
                LastName = "Kowalski",
                Age = 30,
                Position = "Programista",
                Email = "jkowalski@gmail.com"
            },
            new Employee
            {
                Id = 2,
                FirstName = "Adam",
                LastName = "Nowak",
                Age = 35,
                Position = "Doradca klienta",
                Email = "anowak@gmail.com"
            },
            new Employee
            {
                Id = 3,
                FirstName = "Anna",
                LastName = "Kowalska",
                Age = 25,
                Position = "Sekretarka",
                Email = "akowalska@gmail.com"
            }
        };

        private Employee selectedEmployee;
        public Employee SelectedEmployee
        {
            get
            {
                return selectedEmployee;
            }
            set
            {
                SetProperty(ref selectedEmployee, value);
            }
        }

        public EmployeesListViewModel(
            IUnityContainer unityContainer,
            IRegionManager regionManager)
        {
            this.unityContainer = unityContainer;
            this.regionManager = regionManager;
            DeleteButtonState = false;
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            SelectionChangedCommand = new DelegateCommand(OnSelectedItemChanged);
            ExitViewCommand = new DelegateCommand(OnExitView);
            RemoveEmployeeCommand = new DelegateCommand(OnRemoveSelectedEmployee);
        }

        private void OnSelectedItemChanged()
        {
            if (SelectedEmployee != null)
                DeleteButtonState = true;
            else
                DeleteButtonState = false;
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

        private void OnRemoveSelectedEmployee()
        {
            Employees.Remove(SelectedEmployee);
        }
    }
}
