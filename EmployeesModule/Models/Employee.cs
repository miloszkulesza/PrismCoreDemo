using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeesModule.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Position { get; set; }
        public string Email { get; set; }
    }
}
