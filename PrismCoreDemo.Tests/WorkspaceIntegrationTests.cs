using Core.DataAccess;
using EmployeesModule.ViewModels;
using Presentation.Consts;
using Presentation.Services;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Navigation.Regions;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Media;
using System.Windows.Threading;
using Xunit;

namespace PrismCoreDemo.Tests
{
    public class WorkspaceIntegrationTests
    {
        private sealed class Dialogs : IUserDialogService
        {
            public bool Discard;
            public string Error;
            public bool ConfirmDiscardChanges() => Discard;
            public void ShowError(string message) => Error = message;
        }
        private sealed class TestApp : App
        {
            public readonly Dialogs Dialogs = new();
            public void Start()
            {
                ConfigureViewModelLocator();
                Initialize();
            }
            protected override void RegisterTypes(IContainerRegistry registry)
            {
                base.RegisterTypes(registry);
                registry.RegisterInstance<IUserDialogService>(Dialogs);
            }
        }

        [Fact]
        public void RealPrismNavigationSharesContextProtectsDraftAndSynchronizesRibbon()
        {
            RegressionTests.OnSta(() =>
            {
                var app = new TestApp();
                try
                {
                    app.Start();
                    // Realize templates in an invisible window outside the desktop work area.
                    app.MainWindow.Opacity = 0;
                    app.MainWindow.ShowInTaskbar = false;
                    app.MainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                    app.MainWindow.Left = -32000;
                    app.MainWindow.Top = -32000;
                    app.MainWindow.Show();
                    Pump();
                    app.MainWindow.UpdateLayout();
                    RegionManager.UpdateRegions();
                    var regions = app.Container.Resolve<IRegionManager>();
                    var workspace = app.Container.Resolve<IWorkspaceService>();
                    var viewRegion = regions.Regions[RegionNames.ViewRegion];
                    var ribbonRegion = regions.Regions[RegionNames.RibbonRegion];
                    var ribbon = ((Grid)app.MainWindow.Content).Children.OfType<Ribbon>().Single();
                    Assert.Empty(((Grid)app.MainWindow.Content).Children.OfType<TabControl>());
                    Assert.Same(ribbonRegion, RegionManager.GetObservableRegion(ribbon).Value);
                    Assert.IsType<SingleActiveRegion>(ribbonRegion);

                    workspace.Open("EmployeeEdit", "EmployeeEditRibbonTab", new NavigationParameters { { "employeeId", 1 } });
                    Pump();
                    Assert.Null(app.Dialogs.Error);
                    var view = Assert.IsAssignableFrom<FrameworkElement>(viewRegion.ActiveViews.Single());
                    var tab = Assert.IsAssignableFrom<RibbonTab>(ribbonRegion.ActiveViews.Single());
                    var editor = Assert.IsType<EmployeeEditViewModel>(view.DataContext);
                    Assert.Same(editor, tab.DataContext);
                    Assert.Equal(1, editor.Employee.Id);
                    Assert.Contains(tab, ribbon.Items.Cast<object>());
                    Assert.Same(tab, ribbon.SelectedItem);
                    editor.Employee.FirstName = "Niezapisane";

                    workspace.Open("EmployeesList", "EmployeesListRibbonTab");
                    var listView = viewRegion.ActiveViews.Single();
                    workspace.Open("EmployeeEdit", "EmployeeEditRibbonTab", new NavigationParameters { { "employeeId", 2 } });
                    var secondView = Assert.IsAssignableFrom<FrameworkElement>(viewRegion.ActiveViews.Single());
                    var secondEditor = Assert.IsType<EmployeeEditViewModel>(secondView.DataContext);
                    var secondTab = Assert.IsAssignableFrom<RibbonTab>(ribbonRegion.ActiveViews.Single());
                    Assert.NotSame(view, secondView);
                    Assert.NotSame(editor, secondEditor);
                    Assert.Same(secondEditor, secondTab.DataContext);
                    Assert.Equal(2, secondEditor.Employee.Id);
                    Assert.Equal("Niezapisane", editor.Employee.FirstName);

                    workspace.Open("EmployeeEdit", "EmployeeEditRibbonTab", new NavigationParameters { { "employeeId", 1 } });
                    Assert.Same(view, viewRegion.ActiveViews.Single());
                    Assert.Same(tab, ribbon.SelectedItem);
                    Assert.Equal("Niezapisane", editor.Employee.FirstName);

                    workspace.Open("EmployeeEdit", "EmployeeEditRibbonTab");
                    var newView = Assert.IsAssignableFrom<FrameworkElement>(viewRegion.ActiveViews.Single());
                    var newEditor = Assert.IsType<EmployeeEditViewModel>(newView.DataContext);
                    newEditor.Employee.FirstName = "Pierwszy nowy";
                    workspace.Open("EmployeeEdit", "EmployeeEditRibbonTab");
                    var anotherNewView = Assert.IsAssignableFrom<FrameworkElement>(viewRegion.ActiveViews.Single());
                    var anotherNewEditor = Assert.IsType<EmployeeEditViewModel>(anotherNewView.DataContext);
                    Assert.NotSame(newView, anotherNewView);
                    Assert.NotSame(newEditor, anotherNewEditor);
                    Assert.Equal("Pierwszy nowy", newEditor.Employee.FirstName);
                    Assert.True(string.IsNullOrEmpty(anotherNewEditor.Employee.FirstName));
                    Assert.Equal(4, viewRegion.Views.Cast<FrameworkElement>().Count(x => x.DataContext is EmployeeEditViewModel));
                    Assert.Equal(5, ribbonRegion.Views.OfType<RibbonTab>().Count(x => Equals(x.ContextualTabGroupHeader, "Pracownicy")));
                    anotherNewEditor.CloseViewCommand.Execute();
                    newEditor.CloseViewCommand.Execute();
                    Assert.Contains(newView, viewRegion.Views);
                    app.Dialogs.Discard = true;
                    newEditor.CloseViewCommand.Execute();
                    Assert.DoesNotContain(newView, viewRegion.Views);

                    var listTab = ribbonRegion.Views.OfType<RibbonTab>().Single(x => x.DataContext is EmployeesListViewModel);
                    ribbon.SelectedItem = listTab;
                    Assert.Same(listTab, ribbonRegion.ActiveViews.Single());
                    Assert.Same(listView, viewRegion.ActiveViews.Single());
                    ribbonRegion.Activate(tab);
                    Assert.Same(view, viewRegion.ActiveViews.Single());
                    ribbon.SelectedItem = listTab;
                    editor.CloseViewCommand.Execute();
                    Assert.Same(listView, viewRegion.ActiveViews.Single());
                    Assert.DoesNotContain(view, viewRegion.Views);
                    Assert.Contains(secondView, viewRegion.Views);
                    Assert.Equal(2, secondEditor.Employee.Id);

                    workspace.Open("TasksList", "TasksListRibbonTab");
                    workspace.Open("TaskEdit", "TaskEditRibbonTab", new NavigationParameters { { "taskId", 1 } });
                    var taskView = Assert.IsAssignableFrom<FrameworkElement>(viewRegion.ActiveViews.Single());
                    var taskEditor = Assert.IsType<TasksModule.ViewModels.TaskEditViewModel>(taskView.DataContext);
                    Assert.Equal(1, taskEditor.Task.Id);
                    Assert.Same(taskEditor, ((FrameworkElement)ribbonRegion.ActiveViews.Single()).DataContext);
                    taskEditor.Task.Description = "Zmieniony opis";
                    workspace.Open("TaskEdit", "TaskEditRibbonTab", new NavigationParameters { { "taskId", 2 } });
                    var secondTaskView = Assert.IsAssignableFrom<FrameworkElement>(viewRegion.ActiveViews.Single());
                    Assert.NotSame(taskView, secondTaskView);
                    workspace.Open("TaskEdit", "TaskEditRibbonTab");
                    var newTaskView = viewRegion.ActiveViews.Single();
                    workspace.Open("TaskEdit", "TaskEditRibbonTab");
                    Assert.NotSame(newTaskView, viewRegion.ActiveViews.Single());
                    Assert.Equal(5, ribbonRegion.Views.OfType<RibbonTab>().Count(x => Equals(x.ContextualTabGroupHeader, "Zadania")));
                    ribbon.SelectedItem = listTab;
                    Assert.Same(listView, viewRegion.ActiveViews.Single());
                    ribbon.SelectedItem = ribbonRegion.Views.OfType<RibbonTab>().Single(x => ReferenceEquals(x.DataContext, taskEditor));
                    Assert.Same(taskView, viewRegion.ActiveViews.Single());
                    workspace.Open("TasksList", "TasksListRibbonTab");
                    workspace.Open("TaskEdit", "TaskEditRibbonTab", new NavigationParameters { { "taskId", 1 } });
                    Assert.Equal("Zmieniony opis", taskEditor.Task.Description);
                    taskEditor.SaveAndCloseCommand.Execute();
                    Assert.Equal("Zmieniony opis", app.Container.Resolve<ITasksRepository>().Tasks.First().Description);
                    Assert.DoesNotContain(taskView, viewRegion.Views);
                    Assert.Equal("Zadania", Assert.IsAssignableFrom<RibbonTab>(ribbonRegion.ActiveViews.Single()).ContextualTabGroupHeader);

                    workspace.Open("Calendar", "CalendarRibbonTab");
                    var calendarView = Assert.IsType<CalendarModule.Views.Calendar>(viewRegion.ActiveViews.Single());
                    var calendar = Assert.IsType<CalendarModule.ViewModels.CalendarViewModel>(calendarView.DataContext);
                    Assert.Equal(3, calendar.Tasks.Count);
                    // Verify visible day buttons respond to date changes without switching months.
                    var control = ((Grid)calendarView.Content).Children.OfType<System.Windows.Controls.Calendar>().Single();
                    var task = app.Container.Resolve<ITasksRepository>().Tasks.First();
                    control.DisplayDate = task.TaskDate;
                    calendarView.Measure(new Size(800, 450));
                    calendarView.Arrange(new Rect(0, 0, 800, 450));
                    calendarView.UpdateLayout();
                    calendarView.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));
                    var day = FindDay(calendarView, task.TaskDate.Date);
                    Assert.NotNull(day);
                    day.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));
                    Assert.Same(Brushes.OrangeRed, day.Background);
                    app.Container.Resolve<ITasksRepository>().Delete(task);
                    Assert.Same(Brushes.Transparent, day.Background);
                    Assert.Null(app.Dialogs.Error);

                    calendar.CloseViewCommand.Execute();
                    var remaining = viewRegion.Views.Cast<FrameworkElement>().ToList();
                    foreach (var screenView in remaining)
                        ((Presentation.ViewModelBases.ViewModelBase)screenView.DataContext).CloseViewCommand.Execute();
                    Assert.Empty(viewRegion.Views);
                    Assert.Single(ribbonRegion.Views);
                }
                finally
                {
                    app.MainWindow?.Close();
                    app.Shutdown();
                }
            });
        }

        private static CalendarDayButton FindDay(DependencyObject root, DateTime date)
        {
            if (root is CalendarDayButton match && match.DataContext is DateTime day && day.Date == date) return match;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
                if (FindDay(VisualTreeHelper.GetChild(root, i), date) is CalendarDayButton child) return child;
            return null;
        }

        private static void Pump()
        {
            var frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => frame.Continue = false));
            Dispatcher.PushFrame(frame);
        }
    }
}
