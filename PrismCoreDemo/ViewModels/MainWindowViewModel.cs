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
        public DelegateCommand DocumentsCommand { get; set; }
        public DelegateCommand ExitCommand { get; set; }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
            this.regionManager.RegisterViewWithRegion(RegionNames.RibbonRegion, typeof(Controls.ShellRibbonTab));
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            EmployeesCommand = new DelegateCommand(OnEmployeesCommand);
            DocumentsCommand = new DelegateCommand(OnDocumentsCommand);
            ExitCommand = new DelegateCommand(OnExitCommand);
        }

        private void OnEmployeesCommand()
        {
            regionManager.RequestNavigate(RegionNames.ViewRegion, new Uri("EmployeesList", UriKind.Relative));
            regionManager.RequestNavigate(RegionNames.RibbonRegion, new Uri("EmployeesListRibbonTab", UriKind.Relative));
        }

        private void OnDocumentsCommand()
        {
            regionManager.RequestNavigate(RegionNames.ViewRegion, new Uri("DocumentsList", UriKind.Relative));
            regionManager.RequestNavigate(RegionNames.RibbonRegion, "DocumentsListRibbonTab");
        }

        private void OnExitCommand()
        {
            Application.Current.Shutdown();
        }
    }
}
