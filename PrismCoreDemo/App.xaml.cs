using Infrastructure.RegionAdapters;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using PrismCoreDemo.Controls;
using PrismCoreDemo.Views;
using System;
using System.Windows;
using System.Windows.Controls.Ribbon;

namespace PrismCoreDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<Object, ShellRibbonTab>("ShellRibbonTab");
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
            moduleCatalog.AddModule(typeof(DocumentsModule.DocumentsModule));
        }
    }
}
