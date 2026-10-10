using Core.DataAccess;
using ApplicationCore.Events;
using Presentation.Models;
using Presentation.Services;
using Presentation.ViewModelBases;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Linq;

namespace TasksModule.ViewModels
{
    public class TasksListViewModel : ViewModelBase
    {
        public string Title => "Lista zadań";
        private readonly ITasksRepository tasks;
        private readonly IEmployeesRepository employees;
        public ObservableCollection<TaskItem> Tasks { get; } = new();
        private TaskItem selectedTask;
        public TaskItem SelectedTask
        {
            get => selectedTask;
            set { if (SetProperty(ref selectedTask, value)) OnSelectionChanged(); }
        }
        public bool DeleteButtonState => SelectedTask != null;
        public DelegateCommand SelectionChangedCommand { get; }
        public DelegateCommand AddTaskCommand { get; }
        public DelegateCommand EditTaskCommand { get; }
        public DelegateCommand RemoveTaskCommand { get; }

        public TasksListViewModel(IWorkspaceService workspace, IEventAggregator events,
            ITasksRepository tasks, IEmployeesRepository employees) : base(workspace)
        {
            this.tasks = tasks;
            this.employees = employees;
            SelectionChangedCommand = new DelegateCommand(OnSelectionChanged);
            AddTaskCommand = new DelegateCommand(() => workspace.Open("TaskEdit", "TaskEditRibbonTab"));
            EditTaskCommand = new DelegateCommand(Edit, () => SelectedTask != null);
            RemoveTaskCommand = new DelegateCommand(() => { if (SelectedTask != null) tasks.Delete(SelectedTask); }, () => SelectedTask != null);
            events.GetEvent<TaskAddedEvent>().Subscribe(_ => Refresh());
            events.GetEvent<TaskUpdatedEvent>().Subscribe(_ => Refresh());
            events.GetEvent<TaskDeletedEvent>().Subscribe(_ => Refresh());
            events.GetEvent<TaskSucceededEvent>().Subscribe(_ => Refresh());
            events.GetEvent<TaskFailedEvent>().Subscribe(_ => Refresh());
            events.GetEvent<EmployeeUpdatedEvent>().Subscribe(_ => Refresh());
            events.GetEvent<EmployeeDeletedEvent>().Subscribe(_ => Refresh());
            events.GetEvent<EmployeeAddedEvent>().Subscribe(_ => Refresh());
            Refresh();
        }
        private void OnSelectionChanged()
        {
            RaisePropertyChanged(nameof(DeleteButtonState));
            EditTaskCommand.RaiseCanExecuteChanged();
            RemoveTaskCommand.RaiseCanExecuteChanged();
        }
        private void Edit()
        {
            if (SelectedTask == null) return;
            workspace.Open("TaskEdit", "TaskEditRibbonTab", new NavigationParameters { { "taskId", SelectedTask.Id } });
        }
        private void Refresh()
        {
            var id = SelectedTask?.Id;
            var people = employees.Employees;
            Tasks.Clear();
            foreach (var task in tasks.Tasks.OrderByDescending(x => x.TaskDate))
                Tasks.Add(new TaskItem(task, people));
            SelectedTask = Tasks.FirstOrDefault(x => x.Id == id);
        }
    }
}
