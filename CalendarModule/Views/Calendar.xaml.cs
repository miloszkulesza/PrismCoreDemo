using Infrastructure.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace CalendarModule.Views
{
    /// <summary>
    /// Logika interakcji dla klasy CalendarView.xaml
    /// </summary>
    public partial class Calendar : UserControl
    {
        private readonly IEventAggregator eventAggregator;
        private ObservableCollection<DateTime> significantDates;

        public Calendar(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            this.eventAggregator = eventAggregator;
            this.eventAggregator.GetEvent<HighlightCalendarDateEvent>().Subscribe(OnHighlightCalendarDate);
            this.eventAggregator.GetEvent<RemoveHighlightCalendarDateEvent>().Subscribe(OnRemoveHighlightCalendarDate);
            significantDates = new ObservableCollection<DateTime>();
        }

        private void OnRemoveHighlightCalendarDate(DateTime obj)
        {
            if (significantDates.Any(x => x.ToShortDateString() == obj.ToShortDateString()))
                significantDates.Remove(obj);
        }

        private void OnHighlightCalendarDate(DateTime obj)
        {
            if(!significantDates.Any(x => x.ToShortDateString() == obj.ToShortDateString()))
                significantDates.Add(obj);
        }

        private void calendarButton_Loaded(object sender, EventArgs e)
        {
            CalendarDayButton button = (CalendarDayButton)sender;
            DateTime date = (DateTime)button.DataContext;
            HighlightDay(button, date);
            button.DataContextChanged += new DependencyPropertyChangedEventHandler(calendarButton_DataContextChanged);
        }

        private void HighlightDay(CalendarDayButton button, DateTime date)
        {
            if (significantDates.Contains(date))
                button.Background = Brushes.OrangeRed;
            else
                button.Background = Brushes.Transparent;
        }

        private void calendarButton_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CalendarDayButton button = (CalendarDayButton)sender;
            DateTime date = (DateTime)button.DataContext;
            HighlightDay(button, date);
        }
    }
}
