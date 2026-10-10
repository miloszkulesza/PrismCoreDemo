using Presentation.Consts;
using Presentation.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;

namespace PrismCoreDemo.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public DelegateCommand EmployeesCommand { get; }
        public DelegateCommand ExitCommand { get; }
        public DelegateCommand TasksCommand { get; }
        public DelegateCommand CalendarCommand { get; }

        public MainWindowViewModel(IRegionManager regionManager, IWorkspaceService workspace)
        {
            regionManager.RegisterViewWithRegion(RegionNames.RibbonRegion, typeof(Controls.ShellRibbonTab));
            EmployeesCommand = new DelegateCommand(() => workspace.Open("EmployeesList", "EmployeesListRibbonTab"));
            TasksCommand = new DelegateCommand(() => workspace.Open("TasksList", "TasksListRibbonTab"));
            CalendarCommand = new DelegateCommand(() => workspace.Open("Calendar", "CalendarRibbonTab"));
            ExitCommand = new DelegateCommand(() => System.Windows.Application.Current.Shutdown());
        }
    }
}
