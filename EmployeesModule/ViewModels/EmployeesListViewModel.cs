using EmployeesModule.Models;
using EmployeesModule.Views;
using Infrastructure.ViewModelBases;
using Prism.Commands;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace EmployeesModule.ViewModels
{
    public class EmployeesListViewModel : ViewModelBase
    {
        public ICommand SelectionChangedCommand { get; set; }
        public DelegateCommand RemoveEmployeeCommand { get; set; }

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
            IRegionManager regionManager): base(regionManager, typeof(EmployeesList), typeof(EmployeesListRibbonTab))
        {
            DeleteButtonState = false;
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            SelectionChangedCommand = new DelegateCommand(OnSelectedItemChanged);
            RemoveEmployeeCommand = new DelegateCommand(OnRemoveSelectedEmployee);
        }

        private void OnSelectedItemChanged()
        {
            if (SelectedEmployee != null)
                DeleteButtonState = true;
            else
                DeleteButtonState = false;
        }

        private void OnRemoveSelectedEmployee()
        {
            Employees.Remove(SelectedEmployee);
        }
    }
}
