using Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace Presentation.Models
{
    public class TaskItem : Task
    {
        public string Employee { get; }

        public TaskItem(Task task, IReadOnlyList<Employee> employees) : base(task)
        {
            Employee = employees.FirstOrDefault(x => x.Id == task.EmployeeId)?.ToString() ?? "Brak pracownika";
        }
    }
}
