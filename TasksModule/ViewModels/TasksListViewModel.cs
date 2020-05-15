using Infrastructure.Consts;
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
using System.Windows;
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
        public DelegateCommand SelectionChangedCommand { get; set; }

        public DelegateCommand AddTaskCommand { get; set; }
        public DelegateCommand EditTaskCommand { get; set; }
        public DelegateCommand RemoveTaskCommand { get; set; }
        #endregion

        #region properties
        public ObservableCollection<Task> Tasks { get; set; }

        private Task selectedTask;
        public Task SelectedTask
        {
            get { return selectedTask; }
            set { SetProperty(ref selectedTask, value); }
        }

        private bool deleteButtonState;
        public bool DeleteButtonState
        {
            get { return deleteButtonState; }
            set { SetProperty(ref deleteButtonState, value); }
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
            Tasks = new ObservableCollection<Task>(tasks.OrderByDescending(x => x.TaskDate));
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
            EditTaskCommand = new DelegateCommand(OnEditTaskCommand);
            RemoveTaskCommand = new DelegateCommand(OnRemoveTaskCommand);
        }

        private void OnRemoveTaskCommand()
        {
            tasksRepository.Delete(SelectedTask);
        }

        private void OnEditTaskCommand()
        {
            regionManager.RequestNavigate(RegionNames.ViewRegion, new Uri("TaskEdit", UriKind.Relative));
            regionManager.RequestNavigate(RegionNames.RibbonRegion, new Uri("TaskEditRibbonTab", UriKind.Relative), new NavigationParameters($"?taskId={SelectedTask.Id}"));
        }

        private void OnSelectionChangedCommand()
        {
            if (SelectedTask != null)
                DeleteButtonState = true;
            else
                DeleteButtonState = false;
        }

        private void OnAddTaskCommand()
        {
            regionManager.RequestNavigate(RegionNames.ViewRegion, new Uri("TaskEdit", UriKind.Relative));
            regionManager.RequestNavigate(RegionNames.RibbonRegion, new Uri("TaskEditRibbonTab", UriKind.Relative));
        }
        #endregion

        #region event handlers
        private void OnTaskFailedEvent(Task obj)
        {
            Tasks.FirstOrDefault(x => x.Id == obj.Id).IsSucceeded = false;
        }

        private void OnTaskSucceededEvent(Task obj)
        {
            Tasks.FirstOrDefault(x => x.Id == obj.Id).IsSucceeded = true;
        }

        private void OnTaskUpdatedEvent(Task obj)
        {
            for (int i = 0; i < Tasks.Count; i++)
            {
                if (Tasks[i].Id == obj.Id)
                {
                    Tasks[i] = obj;
                    var employee = employeesRepository.Employees.FirstOrDefault(x => x.Id == Tasks[i].EmployeeId);
                    Tasks[i].Employee = $"{employee.FirstName} {employee.LastName}";
                }
            }
        }

        private void OnTaskAddedEvent(Task obj)
        {
            var employee = employeesRepository.Employees.FirstOrDefault(x => x.Id == obj.EmployeeId);
            obj.Employee = $"{employee.FirstName} {employee.LastName}";
            Tasks.Insert(0, obj);
        }

        private void OnTaskDeletedEvent(Task obj)
        {
            Tasks.Remove(obj);
        }
        #endregion
    }
}
