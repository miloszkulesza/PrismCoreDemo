using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.DataAccess
{
    public interface IEmployeesRepository
    {
        IReadOnlyList<Employee> Employees { get; }
        void Save(Employee employee);
        void Delete(Employee employee);
    }
}
