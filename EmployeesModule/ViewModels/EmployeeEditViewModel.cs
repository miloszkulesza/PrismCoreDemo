using EmployeesModule.Views;
using Infrastructure.DataAccess;
using Infrastructure.Models;
using Infrastructure.ViewModelBases;
using Prism.Commands;
using Prism.Regions;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace EmployeesModule.ViewModels
{
    public class EmployeeEditViewModel : ViewModelBase 
    {
        #region private members
        private readonly IEmployeesRepository employeesRepository;
        #endregion

        #region commands
        public DelegateCommand SaveAndCloseCommand { get; set; }
        public DelegateCommand CancelAndCloseViewCommand { get; set; }
        #endregion

        #region properties
        private string title;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private Employee employee;
        public Employee Employee
        {
            get { return employee; }
            set { SetProperty(ref employee, value); }
        }

        private bool saveButtonState;
        public bool SaveButtonState
        {
            get { return saveButtonState; }
            set { SetProperty(ref saveButtonState, value); }
        }
        #endregion

        #region ctor
        public EmployeeEditViewModel(
            IRegionManager regionManager,
            IEmployeesRepository employeesRepository): base(regionManager, typeof(EmployeeEdit), typeof(EmployeeEditRibbonTab))
        {
            this.employeesRepository = employeesRepository;
            RegisterCommands();
        }
        #endregion

        #region private methods
        private void RegisterCommands()
        {
            SaveAndCloseCommand = new DelegateCommand(OnSaveAndClose);
            CancelAndCloseViewCommand = new DelegateCommand(OnCancelAndCloseViewCommand);
        }

        private void OnSaveAndClose()
        {
            employeesRepository.Save(employee);
            CloseViewCommand.Execute();
        }

        private bool CanAddEmployee()
        {
            return Employee.IsValid();
        }

        private void OnCancelAndCloseViewCommand()
        {
            var result = MessageBox.Show("Czy na pewno chcesz anulować zmiany i zamknąć?", "Anuluj i zamknij", MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    var employee = employeesRepository.Employees.FirstOrDefault(x => x.Id == Employee.Id);
                    Employee = employee;
                    base.OnCloseView();
                    break;
                case MessageBoxResult.No:
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region event handlers
        private void Employee_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (CanAddEmployee())
                SaveButtonState = true;
            else
                SaveButtonState = false;
        }
        #endregion

        #region override
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            if(navigationContext.Parameters.Count > 0)
            {
                int id = navigationContext.Parameters.GetValue<int>("employeeId");
                Employee = new Employee(employeesRepository.Employees.FirstOrDefault(x => x.Id == id));
                Title = $"Edycja {Employee.FirstName} {Employee.LastName}";
                SaveButtonState = true;
            }
            else
            {
                Title = "Nowy pracownik";
                Employee = new Employee();
                SaveButtonState = false;
            }
            Employee.PropertyChanged += Employee_PropertyChanged;
        }
        #endregion
    }
}
