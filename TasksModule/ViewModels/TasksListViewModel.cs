using Infrastructure.DataAccess;
using Infrastructure.Events;
using Infrastructure.Models;
using Infrastructure.ViewModelBases;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using TasksModule.Views;

namespace TasksModule.ViewModels
{
    public class TasksListViewModel : ViewModelBase
    {
        #region pivate members
        private readonly IEventAggregator eventAggregator;
        private readonly ITasksRepository tasksRepository;
        private readonly IEmployeesRepository employeesRepository;
        #endregion

        #region commands
        public DelegateCommand AddTaskCommand { get; set; }
        public DelegateCommand SelectionChangedCommand { get; set; }
        #endregion

        #region properties
        public ObservableCollection<Task> Tasks { get; set; }
        private Task selectedTask;
        public Task SelectedTask
        {
            get { return selectedTask; }
            set { SetProperty(ref selectedTask, value); }
        }
        #endregion

        #region ctor
        public TasksListViewModel(
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            ITasksRepository tasksRepository,
            IEmployeesRepository employeesRepository): base(regionManager, typeof(TasksList), typeof(TasksListRibbonTab))
        {
            this.eventAggregator = eventAggregator;
            this.tasksRepository = tasksRepository;
            this.employeesRepository = employeesRepository;
            var tasks = this.tasksRepository.Tasks;
            foreach(var task in tasks)
            {
                var employee = this.employeesRepository.Employees.FirstOrDefault(x => x.Id == task.EmployeeId);
                task.Employee = $"{employee.FirstName} {employee.LastName}";
            }
            Tasks = new ObservableCollection<Task>(tasks);
            this.eventAggregator.GetEvent<TaskAddedEvent>().Subscribe(OnTaskAddedEvent);
            this.eventAggregator.GetEvent<TaskDeletedEvent>().Subscribe(OnTaskDeletedEvent);
            this.eventAggregator.GetEvent<TaskUpdatedEvent>().Subscribe(OnTaskUpdatedEvent);
            this.eventAggregator.GetEvent<TaskSucceededEvent>().Subscribe(OnTaskSucceededEvent);
            this.eventAggregator.GetEvent<TaskFailedEvent>().Subscribe(OnTaskFailedEvent);
            RegisterCommands();
        }
        #endregion

        #region private methods
        private void RegisterCommands()
        {
            AddTaskCommand = new DelegateCommand(OnAddTaskCommand);
            SelectionChangedCommand = new DelegateCommand(OnSelectionChangedCommand);
        }

        private void OnSelectionChangedCommand()
        {

        }

        private void OnAddTaskCommand()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region event handlers
        private void OnTaskFailedEvent(Task obj)
        {
            throw new NotImplementedException();
        }

        private void OnTaskSucceededEvent(Task obj)
        {
            throw new NotImplementedException();
        }

        private void OnTaskUpdatedEvent(Task obj)
        {
            throw new NotImplementedException();
        }

        private void OnTaskAddedEvent(Task obj)
        {
            throw new NotImplementedException();
        }

        private void OnTaskDeletedEvent(Task obj)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
