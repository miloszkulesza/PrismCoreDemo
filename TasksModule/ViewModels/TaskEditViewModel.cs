using Infrastructure.DataAccess;
using Infrastructure.Models;
using Infrastructure.ViewModelBases;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using TasksModule.Views;

namespace TasksModule.ViewModels
{
    public class TaskEditViewModel : ViewModelBase
    {
        #region private members
        private readonly ITasksRepository tasksRepository;
        private readonly IEmployeesRepository employeesRepository;
        #endregion

        #region properties
        private Task task;
        public Task Task
        {
            get { return task; }
            set { SetProperty(ref task, value); }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private bool saveButtonState;
        public bool SaveButtonState
        {
            get { return saveButtonState; }
            set { SetProperty(ref saveButtonState, value); }
        }

        private Employee employee;
        public Employee Employee
        {
            get { return employee; }
            set { SetProperty(ref employee, value); }
        }

        private ObservableCollection<Employee> employees;
        public ObservableCollection<Employee> Employees
        {
            get { return employees; }
            set { SetProperty(ref employees, value); }
        }
        #endregion

        #region commands
        public DelegateCommand SelectedEmployeeChangedCommand { get; set; }
        public DelegateCommand SaveAndCloseCommand { get; set; }
        public DelegateCommand CancelAndCloseViewCommand { get; set; }
        #endregion

        #region ctor
        public TaskEditViewModel(
            IRegionManager regionManager,
            ITasksRepository tasksRepository,
            IEmployeesRepository employeesRepository) : base(regionManager, typeof(TaskEdit), typeof(TaskEditRibbonTab))
        {
            this.tasksRepository = tasksRepository;
            this.employeesRepository = employeesRepository;
            RegisterCommands();
        }
        #endregion

        #region private methods
        private void RegisterCommands()
        {
            SelectedEmployeeChangedCommand = new DelegateCommand(OnSelectedEmployeeChangedCommand);
            SaveAndCloseCommand = new DelegateCommand(OnSaveAndClose);
            CancelAndCloseViewCommand = new DelegateCommand(OnCancelAndCloseViewCommand);
        }

        private void OnCancelAndCloseViewCommand()
        {
            var result = MessageBox.Show("Czy na pewno chcesz anulować zmiany i zamknąć?", "Anuluj i zamknij", MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    var task = tasksRepository.Tasks.FirstOrDefault(x => x.Id == Task.Id);
                    Task = task;
                    base.OnCloseView();
                    break;
                case MessageBoxResult.No:
                    break;
                default:
                    break;
            }
        }

        private void OnSelectedEmployeeChangedCommand()
        {
            Task.EmployeeId = Employee.Id;
        }

        private void OnSaveAndClose()
        {
            tasksRepository.Save(Task);
            CloseViewCommand.Execute();
        }

        private bool CanAddTask()
        {
            return Task.IsValid();
        }

        private void Task_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (CanAddTask())
                SaveButtonState = true;
            else
                SaveButtonState = false;
        }
        #endregion

        #region override
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            Employees = new ObservableCollection<Employee>(employeesRepository.Employees);
            if (navigationContext.Parameters.Count > 0)
            {
                int id = navigationContext.Parameters.GetValue<int>("taskId");
                Task = new Task(tasksRepository.Tasks.FirstOrDefault(x => x.Id == id));
                Employee = Employees.FirstOrDefault(x => x.Id == Task.EmployeeId);
                Title = $"Edycja {Task.Name}";
                SaveButtonState = true;
            }
            else
            {
                Title = "Nowe zadanie";
                Task = new Task();
                Task.TaskDate = DateTime.Now;
                SaveButtonState = false;
                Employee = Employees.FirstOrDefault();
                Task.EmployeeId = Employee.Id;
            }
            Task.PropertyChanged += Task_PropertyChanged;
        }
        #endregion
    }
}
