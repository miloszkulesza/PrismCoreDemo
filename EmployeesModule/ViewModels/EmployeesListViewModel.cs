using Core.DataAccess;
using Core.Models;
using ApplicationCore.Events;
using Presentation.Services;
using Presentation.ViewModelBases;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace EmployeesModule.ViewModels
{
    public class EmployeesListViewModel : ViewModelBase
    {
        public string Title => "Lista pracowników";
        private readonly IEmployeesRepository repository;
        private readonly IUserDialogService dialogs;
        public ObservableCollection<Employee> Employees { get; } = new();
        private Employee selectedEmployee;
        public Employee SelectedEmployee
        {
            get => selectedEmployee;
            set { if (SetProperty(ref selectedEmployee, value)) OnSelectedItemChanged(); }
        }
        public bool DeleteButtonState => SelectedEmployee != null;
        public DelegateCommand SelectionChangedCommand { get; }
        public DelegateCommand RemoveEmployeeCommand { get; }
        public DelegateCommand AddEmployeeCommand { get; }
        public DelegateCommand EditEmployeeCommand { get; }

        public EmployeesListViewModel(IWorkspaceService workspace, IEmployeesRepository repository,
            IEventAggregator events, IUserDialogService dialogs) : base(workspace)
        {
            this.repository = repository;
            this.dialogs = dialogs;
            SelectionChangedCommand = new DelegateCommand(OnSelectedItemChanged);
            RemoveEmployeeCommand = new DelegateCommand(Remove, () => SelectedEmployee != null);
            AddEmployeeCommand = new DelegateCommand(() => workspace.Open("EmployeeEdit", "EmployeeEditRibbonTab"));
            EditEmployeeCommand = new DelegateCommand(Edit, () => SelectedEmployee != null);
            events.GetEvent<EmployeeAddedEvent>().Subscribe(_ => Refresh());
            events.GetEvent<EmployeeUpdatedEvent>().Subscribe(_ => Refresh());
            events.GetEvent<EmployeeDeletedEvent>().Subscribe(_ => Refresh());
            Refresh();
        }
        private void OnSelectedItemChanged()
        {
            RaisePropertyChanged(nameof(DeleteButtonState));
            RemoveEmployeeCommand.RaiseCanExecuteChanged();
            EditEmployeeCommand.RaiseCanExecuteChanged();
        }
        private void Remove()
        {
            if (SelectedEmployee == null) return;
            try { repository.Delete(SelectedEmployee); }
            catch (InvalidOperationException e) { dialogs.ShowError(e.Message); }
        }
        private void Edit()
        {
            if (SelectedEmployee == null) return;
            workspace.Open("EmployeeEdit", "EmployeeEditRibbonTab", new NavigationParameters { { "employeeId", SelectedEmployee.Id } });
        }
        private void Refresh()
        {
            var id = SelectedEmployee?.Id;
            Employees.Clear();
            foreach (var employee in repository.Employees) Employees.Add(employee);
            SelectedEmployee = Employees.FirstOrDefault(x => x.Id == id);
        }
    }
}
