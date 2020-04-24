using EmployeesModule.Views;
using Infrastructure.DataAccess;
using Infrastructure.Models;
using Infrastructure.ViewModelBases;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EmployeesModule.ViewModels
{
    public class EmployeeEditViewModel : ViewModelBase 
    {
        #region private members
        private readonly IEmployeesRepository employeesRepository;
        #endregion

        #region commands
        public DelegateCommand SaveAndCloseCommand { get; set; }
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
                Title = "Edycja pracownika";
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
