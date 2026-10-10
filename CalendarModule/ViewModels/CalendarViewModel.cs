using Core.DataAccess;
using ApplicationCore.Events;
using Presentation.Models;
using Presentation.Services;
using Presentation.ViewModelBases;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation.Regions;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace CalendarModule.ViewModels
{
    public class CalendarViewModel : ViewModelBase
    {
        private readonly ITasksRepository tasks;
        private readonly IEmployeesRepository employees;
        public string Title => "Kalendarz";
        private DateTime selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get => selectedDate;
            set { if (SetProperty(ref selectedDate, value.Date)) RefreshSelectedDate(); }
        }
        public ObservableCollection<TaskItem> Tasks { get; } = new();
        public ObservableCollection<TaskItem> SelectedDateTasks { get; } = new();
        public ObservableCollection<DateTime> SignificantDates { get; } = new();
        private TaskItem selectedTask;
        public TaskItem SelectedTask
        {
            get => selectedTask;
            set { if (SetProperty(ref selectedTask, value)) OnSelectedTaskChanged(); }
        }
        public bool SuccessButtonState => SelectedTask != null;
        public DelegateCommand SelectedDateChangedCommand { get; }
        public DelegateCommand SuccessTaskCommand { get; }
        public DelegateCommand FailTaskCommand { get; }
        public DelegateCommand SelectedTaskChangedCommand { get; }

        public CalendarViewModel(IWorkspaceService workspace, ITasksRepository tasks,
            IEmployeesRepository employees, IEventAggregator events) : base(workspace)
        {
            this.tasks = tasks;
            this.employees = employees;
            SelectedDateChangedCommand = new DelegateCommand(RefreshSelectedDate);
            SuccessTaskCommand = new DelegateCommand(() => { if (SelectedTask != null) tasks.TaskSuccess(SelectedTask); }, () => SelectedTask != null);
            FailTaskCommand = new DelegateCommand(() => { if (SelectedTask != null) tasks.TaskFail(SelectedTask); }, () => SelectedTask != null);
            SelectedTaskChangedCommand = new DelegateCommand(OnSelectedTaskChanged);
            events.GetEvent<TaskAddedEvent>().Subscribe(_ => Refresh());
            events.GetEvent<TaskDeletedEvent>().Subscribe(_ => Refresh());
            events.GetEvent<TaskUpdatedEvent>().Subscribe(_ => Refresh());
            events.GetEvent<TaskSucceededEvent>().Subscribe(_ => Refresh());
            events.GetEvent<TaskFailedEvent>().Subscribe(_ => Refresh());
            events.GetEvent<EmployeeUpdatedEvent>().Subscribe(_ => Refresh());
            events.GetEvent<EmployeeDeletedEvent>().Subscribe(_ => Refresh());
            events.GetEvent<EmployeeAddedEvent>().Subscribe(_ => Refresh());
            Refresh();
        }

        public override void OnNavigatedTo(NavigationContext navigationContext) => Refresh();

        private void Refresh()
        {
            var people = employees.Employees;
            Tasks.Clear();
            foreach (var task in tasks.Tasks.OrderByDescending(x => x.TaskDate))
                Tasks.Add(new TaskItem(task, people));
            SignificantDates.Clear();
            foreach (var date in Tasks.Select(x => x.TaskDate.Date).Distinct()) SignificantDates.Add(date);
            RefreshSelectedDate();
        }

        private void RefreshSelectedDate()
        {
            var id = SelectedTask?.Id;
            SelectedDateTasks.Clear();
            foreach (var task in Tasks.Where(x => x.TaskDate.Date == SelectedDate.Date)) SelectedDateTasks.Add(task);
            SelectedTask = SelectedDateTasks.FirstOrDefault(x => x.Id == id);
        }

        private void OnSelectedTaskChanged()
        {
            RaisePropertyChanged(nameof(SuccessButtonState));
            SuccessTaskCommand.RaiseCanExecuteChanged();
            FailTaskCommand.RaiseCanExecuteChanged();
        }
    }
}