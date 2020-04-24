using Infrastructure.Events;
using Infrastructure.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.DataAccess
{
    public class EmployeesRepository : IEmployeesRepository
    {
        private readonly IEventAggregator eventAggregator;

        public IList<Employee> Employees { get; } = new List<Employee>()
        {
             new Employee
            {
                Id = 1,
                FirstName = "Jan",
                LastName = "Kowalski",
                Age = 30,
                Position = "Programista",
                Email = "jkowalski@gmail.com"
            },
            new Employee
            {
                Id = 2,
                FirstName = "Adam",
                LastName = "Nowak",
                Age = 35,
                Position = "Doradca klienta",
                Email = "anowak@gmail.com"
            },
            new Employee
            {
                Id = 3,
                FirstName = "Anna",
                LastName = "Kowalska",
                Age = 25,
                Position = "Sekretarka",
                Email = "akowalska@gmail.com"
            }
        };

        public EmployeesRepository(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public void Delete(Employee employee)
        {
            Employees.Remove(employee);
            eventAggregator.GetEvent<EmployeeDeletedEvent>().Publish(employee);
        }

        public void Save(Employee employee)
        {
            if (Employees.Any(x => x.Id == employee.Id))
            {
                for (int i = 0; i < Employees.Count; i++)
                {
                    if (Employees[i].Id == employee.Id)
                        Employees[i] = employee;
                }
                eventAggregator.GetEvent<EmployeeUpdatedEvent>().Publish(employee);
            }
            else
            {
                employee.Id = Employees.LastOrDefault().Id + 1;
                Employees.Add(employee);
                eventAggregator.GetEvent<EmployeeAddedEvent>().Publish(employee);
            }
        }
    }
}
