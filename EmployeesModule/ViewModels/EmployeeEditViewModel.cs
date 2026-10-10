using Core.DataAccess;
using Core.Models;
using Prism.Navigation;
using Prism.Navigation.Regions;
using Presentation.Services;
using Presentation.ViewModelBases;
using System;
using System.Linq;

namespace EmployeesModule.ViewModels
{
    public class EmployeeEditViewModel : EditorViewModelBase<Employee>
    {
        private readonly IEmployeesRepository employeesRepository;
        protected override string ParameterName => "employeeId";
        public Employee Employee => Model;
        public string Title => Employee?.Id > 0 ? $"Edycja {Employee.FirstName} {Employee.LastName}" : "Nowy pracownik";

        public EmployeeEditViewModel(IWorkspaceService workspace, IEmployeesRepository employeesRepository, IUserDialogService dialogs)
            : base(workspace, dialogs) => this.employeesRepository = employeesRepository;

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            int id = navigationContext.Parameters.GetValue<int>(ParameterName);
            return id > 0 && Employee?.Id == id;
        }

        // Opening another employee leaves this draft in its own tab.
        public override bool CanNavigateTo(NavigationParameters parameters) => true;

        protected override Employee Load(int id) => id == 0 ? new Employee() :
            new Employee(employeesRepository.Employees.FirstOrDefault(x => x.Id == id) ??
                throw new InvalidOperationException("Pracownik został usunięty."));
        protected override void OnModelLoaded()
        {
            RaisePropertyChanged(nameof(Employee));
            RaisePropertyChanged(nameof(Title));
        }
        protected override bool CanSave() => Employee?.IsValid() == true;
        protected override void Save() => employeesRepository.Save(Employee);
    }
}
