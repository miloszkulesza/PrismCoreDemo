using System.Linq;
using System;
using System.ComponentModel;

namespace Core.Models
{
    public class Task : ObservableModel, IDataErrorInfo
	{
		public int Id { get; set; }

		private DateTime taskDate;

		public DateTime TaskDate
		{
			get { return taskDate; }
			set { SetProperty(ref taskDate, value); }
		}

		private int employeeId;
        public int EmployeeId
        {
            get => employeeId;
            set => SetProperty(ref employeeId, value);
        }

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
		private bool? isSucceeded;
		public bool? IsSucceeded
		{
			get { return isSucceeded; }
			set { SetProperty(ref isSucceeded, value); }
		}

		public Task()
		{

		}

		public Task(Task task)
		{
			Id = task.Id;
			Name = task.Name;
			Description = task.Description;
			EmployeeId = task.EmployeeId;
			IsSucceeded = task.IsSucceeded;
			TaskDate = task.TaskDate;
		}

		public string Error
		{
			get { return string.Join(Environment.NewLine, new[] { NameValidation(), DescriptionValidation() }.Where(x => x != null)); }
		}

		public string this[string columnName]
		{
			get
			{
				string result = null;

				switch (columnName)
				{
					case "Name":
						result = NameValidation();
						break;

					case "Description":
						result = DescriptionValidation();
						break;

					default:
						break;
				}

				return result;
			}
		}

		public void Reset()
		{
			Name = default(string);
			Description = default(string);
		}

		public bool IsValid()
		{
			var nameValid = NameValidation();
			var descriptionValid = DescriptionValidation();

			var result = nameValid == null && descriptionValid == null;
			return result;
		}

		private string NameValidation()
		{
			string result = null;

			if (string.IsNullOrWhiteSpace(Name))
				result = "Wprowadź nazwę zadania";

			return result;
		}

		private string DescriptionValidation()
		{
			string result = null;

			if (string.IsNullOrWhiteSpace(Description))
				result = "Wprowadź opis zadania";

			return result;
		}

	}
}
