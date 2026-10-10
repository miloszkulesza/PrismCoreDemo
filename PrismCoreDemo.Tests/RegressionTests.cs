using CalendarModule.ViewModels;
using Core.Models;
using EmployeesModule.ViewModels;
using Infrastructure.DataAccess;
using Presentation.Services;
using Prism.Events;
using Prism.Navigation;
using Prism.Navigation.Regions;
using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using TasksModule.ViewModels;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace PrismCoreDemo.Tests
{
    public class RegressionTests
    {
        private sealed class Dialogs : IUserDialogService
        {
            public bool Discard;
            public int Confirmations;
            public string Error;
            public bool ConfirmDiscardChanges() { Confirmations++; return Discard; }
            public void ShowError(string message) => Error = message;
        }
        private sealed class Workspace : IWorkspaceService
        {
            public int Closed;
            public void Open(string view, string ribbon, NavigationParameters parameters = null) { }
            public void Activate(object screen) { }
            public void Close(object screen) { Closed++; (screen as IWorkspaceScreen)?.OnClosed(); }
        }
        private sealed class Data
        {
            public readonly EventAggregator Events = new();
            public readonly TasksRepository Tasks;
            public readonly EmployeesRepository Employees;
            public readonly Workspace Workspace = new();
            public readonly Dialogs Dialogs = new();
            public Data()
            {
                Tasks = new TasksRepository(Events);
                Employees = new EmployeesRepository(Events, Tasks);
            }
        }
        private static NavigationContext Navigate(string key = null, int id = 0)
        {
            var context = new NavigationContext(null, new Uri("Edit", UriKind.Relative));
            if (key != null) context.Parameters.Add(key, id);
            return context;
        }

        [Fact]
        public void AddingAfterDeletingAllDataUsesUniqueIds()
        {
            var d = new Data();
            foreach (var task in d.Tasks.Tasks) d.Tasks.Delete(task);
            foreach (var employee in d.Employees.Employees) d.Employees.Delete(employee);
            var person = new Employee { FirstName = "A", LastName = "B", Age = 30, Email = "a@b.pl", Position = "Developer" };
            d.Employees.Save(person);
            Assert.True(person.Id > 3);
            var taskNew = new Core.Models.Task { Name = "Test", Description = "Opis", EmployeeId = person.Id, TaskDate = DateTime.Today };
            d.Tasks.Save(taskNew);
            Assert.True(taskNew.Id > 3);
        }

        [Fact]
        public void AssignedEmployeeCannotBeDeletedAndListShowsReason()
        {
            var d = new Data();
            var list = new EmployeesListViewModel(d.Workspace, d.Employees, d.Events, d.Dialogs);
            list.SelectedEmployee = list.Employees.First();
            list.RemoveEmployeeCommand.Execute();
            Assert.Equal(3, d.Employees.Employees.Count);
            Assert.Contains("przypisane zadania", d.Dialogs.Error);
        }

        [Fact]
        public void RepositorySnapshotsCannotModifyStoredDataOrBypassValidation()
        {
            var d = new Data();
            d.Employees.Employees.First().FirstName = "Changed outside Save";
            d.Tasks.Tasks.First().Name = "Changed outside Save";
            Assert.Equal("Jan", d.Employees.Employees.First().FirstName);
            Assert.Equal("Zadanie", d.Tasks.Tasks.First().Name);
            Assert.Throws<ArgumentException>(() => d.Employees.Save(new Employee()));
            Assert.Throws<ArgumentException>(() => d.Tasks.Save(new Core.Models.Task()));
        }

        [Fact]
        public void CalendarRefreshesMovedTasksAndNormalizesHighlightDates()
        {
            var d = new Data();
            var calendar = new CalendarViewModel(d.Workspace, d.Tasks, d.Employees, d.Events);
            var task = d.Tasks.Tasks.First();
            calendar.SelectedDate = task.TaskDate;
            calendar.SelectedTask = calendar.SelectedDateTasks.Single();
            var previous = task.TaskDate.Date;
            task.TaskDate = previous.AddDays(1).AddHours(12);
            d.Tasks.Save(task);
            Assert.Empty(calendar.SelectedDateTasks);
            Assert.Null(calendar.SelectedTask);
            Assert.DoesNotContain(previous, calendar.SignificantDates);
            Assert.Contains(task.TaskDate.Date, calendar.SignificantDates);
            calendar.SelectedDate = previous;
            task.TaskDate = previous.AddHours(17);
            d.Tasks.Save(task);
            Assert.Single(calendar.SelectedDateTasks);
            Assert.Contains(previous, calendar.SignificantDates);
            d.Tasks.Delete(task);
            Assert.Empty(calendar.SelectedDateTasks);
            Assert.DoesNotContain(previous, calendar.SignificantDates);
        }

        [Fact]
        public void CalendarKeepsDayHighlightedWhileAnotherTaskRemains()
        {
            var d = new Data();
            var calendar = new CalendarViewModel(d.Workspace, d.Tasks, d.Employees, d.Events);
            var first = d.Tasks.Tasks.First();
            var another = new Core.Models.Task(first) { Id = 0, TaskDate = first.TaskDate.AddHours(18) };
            d.Tasks.Save(another);
            d.Tasks.Delete(first);
            Assert.Contains(first.TaskDate.Date, calendar.SignificantDates);
        }

        [Fact]
        public void EmployeeChangesRefreshTaskListAndCalendar()
        {
            var d = new Data();
            var list = new TasksListViewModel(d.Workspace, d.Events, d.Tasks, d.Employees);
            var calendar = new CalendarViewModel(d.Workspace, d.Tasks, d.Employees, d.Events);
            var employee = d.Employees.Employees.First();
            employee.LastName = "Nowe nazwisko";
            d.Employees.Save(employee);
            Assert.Contains("Nowe nazwisko", list.Tasks.Single(x => x.EmployeeId == employee.Id).Employee);
            Assert.Contains("Nowe nazwisko", calendar.Tasks.Single(x => x.EmployeeId == employee.Id).Employee);
        }

        [Fact]
        public void EmptyEmployeesDisableTaskSaveAndAddingEmployeeEnablesIt()
        {
            var d = new Data();
            foreach (var task in d.Tasks.Tasks) d.Tasks.Delete(task);
            foreach (var employee in d.Employees.Employees) d.Employees.Delete(employee);
            var editor = new TaskEditViewModel(d.Workspace, d.Tasks, d.Employees, d.Dialogs, d.Events);
            editor.OnNavigatedTo(Navigate());
            editor.Task.Name = "Nazwa";
            editor.Task.Description = "Opis";
            Assert.Null(editor.Employee);
            Assert.False(editor.SaveAndCloseCommand.CanExecute());
            editor.SaveAndCloseCommand.Execute();
            Assert.Empty(d.Tasks.Tasks);
            var person = new Employee { FirstName = "A", LastName = "B", Age = 30, Email = "a@b.pl", Position = "Dev" };
            d.Employees.Save(person);
            Assert.True(editor.SaveAndCloseCommand.CanExecute());
        }

        [Fact]
        public void EmployeeEditorMatchesItsRecordPreservesDraftAndConfirmsBeforeClosing()
        {
            var d = new Data();
            var editor = new EmployeeEditViewModel(d.Workspace, d.Employees, d.Dialogs);
            editor.OnNavigatedTo(Navigate("employeeId", 1));
            var draft = editor.Employee;
            draft.FirstName = "Niezapisane";
            editor.OnNavigatedTo(Navigate("employeeId", 1));
            Assert.Same(draft, editor.Employee);
            Assert.True(editor.CanNavigateTo(new NavigationParameters { { "employeeId", 1 } }));
            Assert.True(editor.CanNavigateTo(new NavigationParameters { { "employeeId", 2 } }));
            Assert.True(editor.IsNavigationTarget(Navigate("employeeId", 1)));
            Assert.False(editor.IsNavigationTarget(Navigate("employeeId", 2)));
            Assert.False(editor.IsNavigationTarget(Navigate()));
            editor.CloseViewCommand.Execute();
            Assert.Equal(0, d.Workspace.Closed);
            Assert.Equal("Jan", d.Employees.Employees.First().FirstName);
            d.Dialogs.Discard = true;
            editor.CloseViewCommand.Execute();
            Assert.Equal(1, d.Workspace.Closed);
            draft.FirstName = "Old detached draft";
            Assert.False(editor.HasChanges);
            editor.OnNavigatedTo(Navigate("employeeId", 1));
            Assert.Equal("Jan", editor.Employee.FirstName);
        }

        [Fact]
        public void TaskEditorTracksEmployeeChangeAndSaveClosesWithoutDiscardPrompt()
        {
            var d = new Data();
            var editor = new TaskEditViewModel(d.Workspace, d.Tasks, d.Employees, d.Dialogs, d.Events);
            editor.OnNavigatedTo(Navigate("taskId", 1));
            editor.Employee = editor.Employees.Single(x => x.Id == 2);
            Assert.True(editor.HasChanges);
            Assert.True(editor.CanNavigateTo(new NavigationParameters { { "taskId", 2 } }));
            Assert.True(editor.IsNavigationTarget(Navigate("taskId", 1)));
            Assert.False(editor.IsNavigationTarget(Navigate("taskId", 2)));
            Assert.False(editor.IsNavigationTarget(Navigate()));
            editor.SaveAndCloseCommand.Execute();
            Assert.Equal(2, d.Tasks.Tasks.First().EmployeeId);
            Assert.Equal(1, d.Workspace.Closed);
            Assert.Equal(0, d.Dialogs.Confirmations);
        }

        [Fact]
        public void ValidationSummaryDoesNotThrowAndRejectsWhitespace()
        {
            Assert.NotEmpty(new Employee().Error);
            Assert.NotEmpty(new Core.Models.Task().Error);
            Assert.False(new Core.Models.Task { Name = " ", Description = " " }.IsValid());
        }

        internal static void OnSta(Action action)
        {
            Exception error = null;
            var thread = new Thread(() => { try { action(); } catch (Exception e) { error = e; } });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            if (error != null) ExceptionDispatchInfo.Capture(error).Throw();
        }
    }
}
