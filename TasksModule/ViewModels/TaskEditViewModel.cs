using Core.DataAccess;
using Core.Models;
using ApplicationCore.Events;
using Presentation.Services;
using Presentation.ViewModelBases;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using Prism.Navigation.Regions;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace TasksModule.ViewModels
{
    public class TaskEditViewModel : EditorViewModelBase<Task>
    {
        private readonly ITasksRepository tasksRepository;
        private readonly IEmployeesRepository employeesRepository;
        private Employee employee;
        private ObservableCollection<Employee> employees = new();
        protected override string ParameterName => "taskId";
        public override bool CanNavigateTo(NavigationParameters parameters) => true;
        public override bool IsNavigationTarget(NavigationContext context)
        {
            int id = context.Parameters.GetValue<int>(ParameterName);
            return id > 0 && Task?.Id == id;
        }
        public Task Task => Model;
        public string Title => Task?.Id > 0 ? $"Edycja {Task.Name}" : "Nowe zadanie";
        public ObservableCollection<Employee> Employees
        {
            get => employees;
            private set => SetProperty(ref employees, value);
        }
        public Employee Employee
        {
            get => employee;
            set
            {
                if (SetProperty(ref employee, value))
                {
                    if (Task != null) Task.EmployeeId = value?.Id ?? 0;
                    NotifyValidation();
                }
            }
        }
        public bool HasEmployees => Employees.Count > 0;
        public string EmployeeMessage => HasEmployees ? "" : "Dodaj pracownika przed zapisaniem zadania.";
        public DelegateCommand SelectedEmployeeChangedCommand { get; }

        public TaskEditViewModel(IWorkspaceService workspace, ITasksRepository tasksRepository,
            IEmployeesRepository employeesRepository, IUserDialogService dialogs, IEventAggregator events) : base(workspace, dialogs)
        {
            this.tasksRepository = tasksRepository;
            this.employeesRepository = employeesRepository;
            SelectedEmployeeChangedCommand = new DelegateCommand(() => NotifyValidation());
            events.GetEvent<EmployeeAddedEvent>().Subscribe(_ => RefreshEmployees());
            events.GetEvent<EmployeeUpdatedEvent>().Subscribe(_ => RefreshEmployees());
            events.GetEvent<EmployeeDeletedEvent>().Subscribe(_ => RefreshEmployees());
        }

        protected override Task Load(int id) => id == 0 ? new Task { TaskDate = DateTime.Today } :
            new Task(tasksRepository.Tasks.FirstOrDefault(x => x.Id == id) ??
                throw new InvalidOperationException("Zadanie zostało usunięte."));

        protected override void OnModelLoaded()
        {
            RefreshEmployees();
            RaisePropertyChanged(nameof(Task));
            RaisePropertyChanged(nameof(Title));
        }

        private void RefreshEmployees()
        {
            var employeeId = Task?.EmployeeId ?? 0;
            Employees = new ObservableCollection<Employee>(employeesRepository.Employees);
            Employee = Employees.FirstOrDefault(x => x.Id == employeeId);
            // A new draft may choose the first available employee. Existing tasks keep their assignment.
            if (Employee == null && Task?.Id == 0) Employee = Employees.FirstOrDefault();
            RaisePropertyChanged(nameof(HasEmployees));
            RaisePropertyChanged(nameof(EmployeeMessage));
            NotifyValidation();
        }

        protected override bool CanSave() => Task?.IsValid() == true && Task.TaskDate != default &&
            Employee != null && employeesRepository.Employees.Any(x => x.Id == Employee.Id);
        protected override void Save()
        {
            if (!CanSave()) throw new InvalidOperationException("Wybierz istniejącego pracownika i popraw dane zadania.");
            tasksRepository.Save(Task);
        }
    }
}
