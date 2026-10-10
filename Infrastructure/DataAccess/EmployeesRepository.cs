using Core.DataAccess;
using Core.Models;
using ApplicationCore.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.DataAccess
{
    public class EmployeesRepository : IEmployeesRepository
    {
        private readonly IEventAggregator events;
        private readonly ITasksRepository tasks;
        private readonly List<Employee> employees = new()
        {
            new Employee { Id = 1, FirstName = "Jan", LastName = "Kowalski", Age = 30, Position = "Programista", Email = "jkowalski@gmail.com" },
            new Employee { Id = 2, FirstName = "Adam", LastName = "Nowak", Age = 35, Position = "Doradca klienta", Email = "anowak@gmail.com" },
            new Employee { Id = 3, FirstName = "Anna", LastName = "Kowalska", Age = 25, Position = "Sekretarka", Email = "akowalska@gmail.com" }
        };
        private int nextId = 4;
        // Snapshots protect stored objects from changes made outside Save.
        public IReadOnlyList<Employee> Employees => employees.Select(x => new Employee(x)).ToList().AsReadOnly();
        public EmployeesRepository(IEventAggregator events, ITasksRepository tasks)
        {
            this.events = events;
            this.tasks = tasks;
        }

        public void Delete(Employee employee)
        {
            ArgumentNullException.ThrowIfNull(employee);
            var existing = employees.FirstOrDefault(x => x.Id == employee.Id);
            if (existing == null) return;
            if (tasks.Tasks.Any(x => x.EmployeeId == employee.Id))
                throw new InvalidOperationException("Nie można usunąć pracownika, który ma przypisane zadania. Najpierw usuń zadania lub przypisz je innej osobie.");
            employees.Remove(existing);
            events.GetEvent<EmployeeDeletedEvent>().Publish(new Employee(existing));
        }

        public void Save(Employee employee)
        {
            ArgumentNullException.ThrowIfNull(employee);
            if (!employee.IsValid()) throw new ArgumentException(employee.Error, nameof(employee));
            var index = employees.FindIndex(x => x.Id == employee.Id);
            if (index >= 0)
            {
                employees[index] = new Employee(employee);
                events.GetEvent<EmployeeUpdatedEvent>().Publish(new Employee(employee));
            }
            else
            {
                employee.Id = nextId++;
                employees.Add(new Employee(employee));
                events.GetEvent<EmployeeAddedEvent>().Publish(new Employee(employee));
            }
        }
    }
}