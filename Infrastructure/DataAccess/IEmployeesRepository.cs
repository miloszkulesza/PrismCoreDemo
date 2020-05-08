using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.DataAccess
{
    public interface IEmployeesRepository
    {
        IList<Employee> Employees { get; }
        void Save(Employee employee);
        void Delete(Employee employee);
    }
}
