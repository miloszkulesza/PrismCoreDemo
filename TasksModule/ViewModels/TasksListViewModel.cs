using Infrastructure.ViewModelBases;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Text;
using TasksModule.Views;

namespace TasksModule.ViewModels
{
    public class TasksListViewModel : ViewModelBase
    {
        private readonly IEventAggregator eventAggregator;
        public TasksListViewModel(
            IRegionManager regionManager,
            IEventAggregator eventAggregator): base(regionManager, typeof(TasksList), typeof(TasksListRibbonTab))
        {
            this.eventAggregator = eventAggregator;
        }
    }
}
