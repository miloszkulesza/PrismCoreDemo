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

namespace EmployeesModule.ViewModels
{
    public class EmployeesListViewModel : ViewModelBase
    {
        private readonly IEmployeesRepository employeesRepository;
        private readonly IEventAggregator eventAggregator;

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
            employeesRepository.Delete(SelectedEmployee);
        }
    }
}
