using Prism.Mvvm;
using System;

namespace Infrastructure.Models
{
    public class Task : BindableBase
    {
		public int Id { get; set; }

		private DateTime taskDate;

		public DateTime TaskDate
		{
			get { return taskDate; }
			set { SetProperty(ref taskDate, value); }
		}

		public int EmployeeId { get; set; }

		private string name;

		public string Name
		{
			get { return name; }
			set { SetProperty(ref name, value); }
		}

		private string description;
		public string Description
		{
			get { return description; }
			set { SetProperty(ref description, value); }
		}

		private string employee;
		public string Employee
		{
			get { return employee; }
			set { SetProperty(ref employee, value); }
		}

		private bool? isSucceeded;
		public bool? IsSucceeded
		{
			get { return isSucceeded; }
			set { SetProperty(ref isSucceeded, value); }
		}
	}
}
