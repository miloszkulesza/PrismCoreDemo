using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int EmployeeId { get; set; }
    }
}
