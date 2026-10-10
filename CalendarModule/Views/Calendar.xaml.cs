using CalendarModule.ViewModels;
using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace CalendarModule.Views
{
    public partial class Calendar : UserControl
    {
        private CalendarViewModel viewModel;
        public Calendar()
        {
            InitializeComponent();
            Loaded += (_, _) => AttachViewModel();
            Unloaded += (_, _) => DetachViewModel();
            DataContextChanged += (_, _) => { if (IsLoaded) AttachViewModel(); };
        }

        private void AttachViewModel()
        {
            DetachViewModel();
            viewModel = DataContext as CalendarViewModel;
            if (viewModel != null) viewModel.SignificantDates.CollectionChanged += DatesChanged;
            RefreshButtons(this);
        }
        private void DetachViewModel()
        {
            if (viewModel != null) viewModel.SignificantDates.CollectionChanged -= DatesChanged;
            viewModel = null;
        }
        private void DatesChanged(object sender, NotifyCollectionChangedEventArgs e) => RefreshButtons(this);

        private void RefreshButtons(DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is CalendarDayButton button) HighlightDay(button);
                RefreshButtons(child);
            }
        }
        private void HighlightDay(CalendarDayButton button)
        {
            button.Background = button.DataContext is DateTime date && viewModel?.SignificantDates.Contains(date.Date) == true
                ? Brushes.OrangeRed : Brushes.Transparent;
        }
        private void calendarButton_Loaded(object sender, RoutedEventArgs e)
        {
            var button = (CalendarDayButton)sender;
            button.DataContextChanged -= calendarButton_DataContextChanged;
            button.DataContextChanged += calendarButton_DataContextChanged;
            HighlightDay(button);
        }
        private void calendarButton_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) =>
            HighlightDay((CalendarDayButton)sender);
    }
}