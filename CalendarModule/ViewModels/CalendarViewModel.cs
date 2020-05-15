using CalendarModule.Views;
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

namespace CalendarModule.ViewModels
{
    public class CalendarViewModel : ViewModelBase
    {
        #region private members
        private readonly ITasksRepository tasksRepository;
        private readonly IEmployeesRepository employeesRepository;
        private readonly IEventAggregator eventAggregator;
        #endregion

        #region properties
        private string title;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private DateTime selectedDate;
        public DateTime SelectedDate
        {
            get { return selectedDate; }
            set { SetProperty(ref selectedDate, value); }
        }

        private ObservableCollection<Task> tasks;
        public ObservableCollection<Task> Tasks
        {
            get { return tasks; }
            set { SetProperty(ref tasks, value); }
        }

        private ObservableCollection<Task> selectedDateTasks;
        public ObservableCollection<Task> SelectedDateTasks
        {
            get { return selectedDateTasks; }
            set { SetProperty(ref selectedDateTasks, value); }
        }

        private Task selectedTask;
        public Task SelectedTask
        {
            get { return selectedTask; }
            set { SetProperty(ref selectedTask, value); }
        }

        private bool successButtonState;
        public bool SuccessButtonState
        {
            get { return successButtonState; }
            set { SetProperty(ref successButtonState, value); }
        }
        #endregion

        #region commands
        public DelegateCommand SelectedDateChangedCommand { get; set; }
        public DelegateCommand SuccessTaskCommand { get; set; }
        public DelegateCommand FailTaskCommand { get; set; }
        public DelegateCommand SelectedTaskChangedCommand { get; set; }
        #endregion

        #region ctor
        public CalendarViewModel(
            IRegionManager regionManager,
            ITasksRepository tasksRepository,
            IEmployeesRepository employeesRepository,
            IEventAggregator eventAggregator) : base(regionManager, typeof(Calendar), typeof(CalendarRibbonTab))
        {
            this.tasksRepository = tasksRepository;
            this.employeesRepository = employeesRepository;
            this.eventAggregator = eventAggregator;
            this.eventAggregator.GetEvent<TaskAddedEvent>().Subscribe(OnTaskAdded);
            this.eventAggregator.GetEvent<TaskDeletedEvent>().Subscribe(OnTaskDeleted);
            this.eventAggregator.GetEvent<TaskUpdatedEvent>().Subscribe(OnTaskUpdated);
            Title = "Kalendarz";
            RegisterCommands();
        }
        #endregion

        #region private methods
        private void RegisterCommands()
        {
            SelectedDateChangedCommand = new DelegateCommand(OnSelectedDateChanged);
            SuccessTaskCommand = new DelegateCommand(OnSuccessTaskCommand);
            FailTaskCommand = new DelegateCommand(OnFailTaskCommand);
            SelectedTaskChangedCommand = new DelegateCommand(OnSelectedTaskChanged);
        }

        private void OnSelectedDateChanged()
        {
            SelectedDateTasks = new ObservableCollection<Task>(Tasks.Where(x => x.TaskDate.ToShortDateString() == SelectedDate.ToShortDateString()).ToList());
            SelectedTask = null;
        }

        private void OnFailTaskCommand()
        {
            tasksRepository.TaskFail(SelectedTask);
        }

        private void OnSuccessTaskCommand()
        {
            tasksRepository.TaskSuccess(SelectedTask);
        }

        private void OnSelectedTaskChanged()
        {
            if (SelectedTask != null)
                SuccessButtonState = true;
            else
                SuccessButtonState = false;
        }
        #endregion

        #region override
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            Tasks = new ObservableCollection<Task>(tasksRepository.Tasks);
            foreach (var task in Tasks)
            {
                var employee = employeesRepository.Employees.FirstOrDefault(x => x.Id == task.EmployeeId);
                task.Employee = $"{employee.FirstName} {employee.LastName}";
                eventAggregator.GetEvent<HighlightCalendarDateEvent>().Publish(task.TaskDate);
            }
        }
        #endregion

        #region event handlers
        private void OnTaskAdded(Task obj)
        {
            Tasks.Add(obj);
            if (SelectedDate.ToShortDateString() == obj.TaskDate.ToShortDateString())
            {
                SelectedDateTasks.Insert(0, obj);
            }

        }

        private void OnTaskDeleted(Task obj)
        {
            Tasks.Remove(obj);
            if (SelectedDate.ToShortDateString() == obj.TaskDate.ToShortDateString())
            {
                SelectedDateTasks.Remove(obj);
            }
        }

        private void OnTaskUpdated(Task obj)
        {
            for(int i = 0; i < Tasks.Count; i++)
            {
                if (Tasks[i].Id == obj.Id)
                    Tasks[i] = obj;
            }
            if(SelectedDate.ToShortDateString() == obj.TaskDate.ToShortDateString())
            {
                for (int i = 0; i < SelectedDateTasks.Count; i++)
                {
                    if (SelectedDateTasks[i].Id == obj.Id)
                        SelectedDateTasks[i] = obj;
                }
            }
        }
        #endregion
    }
}
