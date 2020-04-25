using Infrastructure.Consts;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Windows;
using System.Windows.Input;

namespace PrismCoreDemo.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;

        public DelegateCommand EmployeesCommand { get; set; }
        public DelegateCommand ExitCommand { get; set; }
        public DelegateCommand TasksCommand { get; set; }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
            this.regionManager.RegisterViewWithRegion(RegionNames.RibbonRegion, typeof(Controls.ShellRibbonTab));
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            EmployeesCommand = new DelegateCommand(OnEmployeesCommand);
            ExitCommand = new DelegateCommand(OnExitCommand);
            TasksCommand = new DelegateCommand(OnTasksCommand);
        }

        private void OnEmployeesCommand()
        {
            regionManager.RequestNavigate(RegionNames.ViewRegion, new Uri("EmployeesList", UriKind.Relative));
            regionManager.RequestNavigate(RegionNames.RibbonRegion, new Uri("EmployeesListRibbonTab", UriKind.Relative));
        }

        private void OnExitCommand()
        {
            Application.Current.Shutdown();
        }

        private void OnTasksCommand()
        {
            regionManager.RequestNavigate(RegionNames.ViewRegion, new Uri("TasksList", UriKind.Relative));
            regionManager.RequestNavigate(RegionNames.RibbonRegion, new Uri("TasksListRibbonTab", UriKind.Relative));
        }
    }
}
