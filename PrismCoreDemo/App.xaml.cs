using CalendarModule;
using Core.DataAccess;
using Infrastructure.DataAccess;
using Presentation.RegionAdapters;
using Presentation.Services;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation.Regions;
using Prism.Unity;
using PrismCoreDemo.Controls;
using PrismCoreDemo.Views;
using System;
using System.Windows;
using System.Windows.Controls.Ribbon;

namespace PrismCoreDemo
{
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IUserDialogService, UserDialogService>();
            containerRegistry.RegisterSingleton<IWorkspaceService, WorkspaceService>();
            containerRegistry.Register<Object, ShellRibbonTab>("ShellRibbonTab");
            containerRegistry.RegisterSingleton<IEmployeesRepository, EmployeesRepository>();
            containerRegistry.RegisterSingleton<ITasksRepository, TasksRepository>();
        }

        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            base.ConfigureRegionAdapterMappings(regionAdapterMappings);
            if (regionAdapterMappings != null)
            {
                regionAdapterMappings.RegisterMapping(typeof(Ribbon), this.Container.Resolve<RibbonRegionAdapter>());
            }
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);
            moduleCatalog.AddModule(typeof(EmployeesModule.EmployeesModule));
            moduleCatalog.AddModule(typeof(TasksModule.TasksModule));
            moduleCatalog.AddModule(typeof(CalendarModule.CalendarModule));
        }
    }
}
