using Prism.Mvvm;
using System;

namespace Infrastructure.Models
{
    public class Task : BindableBase
    {
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

	}
}
