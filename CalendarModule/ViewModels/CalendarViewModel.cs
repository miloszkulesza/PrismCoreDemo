using CalendarModule.Views;
using Infrastructure.ViewModelBases;
using Prism.Regions;

namespace CalendarModule.ViewModels
{
    public class CalendarViewModel : ViewModelBase
    {
        private string title;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public CalendarViewModel(IRegionManager regionManager): base(regionManager, typeof(Calendar), typeof(CalendarRibbonTab))
        {
            Title = "Kalendarz";
        }
    }
}
