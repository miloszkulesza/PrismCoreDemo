using Infrastructure.Models;
using EmployeesModule.Views;
using Infrastructure.ViewModelBases;
using Prism.Commands;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Infrastructure.DataAccess;
using Prism.Events;
using Infrastructure.Events;
using System;
using Infrastructure.Consts;
using System.Linq;

namespace EmployeesModule.ViewModels
{
    public class EmployeesListViewModel : ViewModelBase
    {
        #region private members
        private readonly IEmployeesRepository employeesRepository;
        private readonly IEventAggregator eventAggregator;
        #endregion

        #region commands
        public DelegateCommand SelectionChangedCommand { get; set; }
        public DelegateCommand RemoveEmployeeCommand { get; set; }
        public DelegateCommand AddEmployeeCommand { get; set; }
        public DelegateCommand EditEmployeeCommand { get; set; }
        #endregion

        #region properties
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

        public static ObservableCollection<Employee> Employees { get; set; }

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
        #endregion

        #region ctor
        public EmployeesListViewModel(
            IRegionManager regionManager,
            IEmployeesRepository employeesRepository,
            IEventAggregator eventAggregator): base(regionManager, typeof(EmployeesList), typeof(EmployeesListRibbonTab))
        {
            this.employeesRepository = employeesRepository;
            this.eventAggregator = eventAggregator;
            Employees = new ObservableCollection<Employee>(employeesRepository.Employees);
            DeleteButtonState = false;
            RegisterCommands();
            this.eventAggregator.GetEvent<EmployeeAddedEvent>().Subscribe(OnEmployeeAddedEvent);
            this.eventAggregator.GetEvent<EmployeeUpdatedEvent>().Subscribe(OnEmployeeUpdatedEvent);
            this.eventAggregator.GetEvent<EmployeeDeletedEvent>().Subscribe(OnEmployeeDeletedEvent);
        }
        #endregion

        #region private methods
        private void RegisterCommands()
        {
            SelectionChangedCommand = new DelegateCommand(OnSelectedItemChanged);
            RemoveEmployeeCommand = new DelegateCommand(OnRemoveSelectedEmployee);
            AddEmployeeCommand = new DelegateCommand(OnAddEmployee);
            EditEmployeeCommand = new DelegateCommand(OnEditEmployee);
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
            employeesRepository.Delete(SelectedEmployee);
        }

        private void OnAddEmployee()
        {
            regionManager.RequestNavigate(RegionNames.ViewRegion, new Uri("EmployeeEdit", UriKind.Relative));
            regionManager.RequestNavigate(RegionNames.RibbonRegion, new Uri("EmployeeEditRibbonTab", UriKind.Relative));
        }

        private void OnEditEmployee()
        {
            regionManager.RequestNavigate(RegionNames.ViewRegion, new Uri("EmployeeEdit", UriKind.Relative));
            regionManager.RequestNavigate(RegionNames.RibbonRegion, new Uri("EmployeeEditRibbonTab", UriKind.Relative), new NavigationParameters($"?employeeId={SelectedEmployee.Id}"));
        }
        #endregion

        #region event handlers
        private void OnEmployeeDeletedEvent(Employee obj)
        {
            Employees.Remove(obj);
        }

        private void OnEmployeeUpdatedEvent(Employee obj)
        {
            for (int i = 0; i < Employees.Count; i++)
            {
                if (Employees[i].Id == obj.Id)
                    Employees[i] = obj;
            }
        }

        private void OnEmployeeAddedEvent(Employee obj)
        {
            Employees.Add(obj);
        }
        #endregion
    }
}
