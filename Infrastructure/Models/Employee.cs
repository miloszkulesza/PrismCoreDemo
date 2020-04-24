using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Infrastructure.Models
{
    public class Employee : BindableBase, IDataErrorInfo
    {
        public int Id { get; set; }

        private string firstName;
        public string FirstName
        {
            get { return firstName; }
            set { SetProperty(ref firstName, value); }
        }

        private string lastName;
        public string LastName
        {
            get { return lastName; }
            set { SetProperty(ref lastName, value); }
        }

        private int age;
        public int Age
        {
            get { return age; }
            set { SetProperty(ref age, value); }
        }

        private string email;
        public string Email
        {
            get { return email; }
            set { SetProperty(ref email, value); }
        }

        private string position;
        public string Position
        {
            get { return position; }
            set { SetProperty(ref position, value); }
        }

        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string columnName]
        {
            get
            {
                string result = null;

                switch (columnName)
                {
                    case "FirstName":
                        result = FirstNameValidation();
                        break;

                    case "LastName":
                        result = LastNameValidation();
                        break;

                    case "Age":
                        result = AgeValidation();
                        break;

                    case "Email":
                        result = EmailValidation();
                        break;
                    case "Position":
                        result = PositionValidation();
                        break;

                    default:
                        break;
                }

                return result;
            }
        }

        public void Reset()
        {
            FirstName = default(string);
            LastName = default(string);
            Age = default(int);
            Email = default(string);
            Position = default(string);
        }

        public bool IsValid()
        {
            var firstNameValid = FirstNameValidation();
            var lastNameValid = LastNameValidation();
            var ageValid = AgeValidation();
            var emailValid = EmailValidation();
            var positionValid = PositionValidation();

            var result = firstNameValid == null && lastNameValid == null && ageValid == null && emailValid == null && positionValid == null;
            return result;
        }

        private string FirstNameValidation()
        {
            string result = null;

            if (string.IsNullOrEmpty(FirstName))
                result = "Wprowadź imię";

            return result;
        }

        private string LastNameValidation()
        {
            string result = null;

            if (string.IsNullOrEmpty(LastName))
                result = "Wprowadź nazwisko";

            return result;
        }

        private string AgeValidation()
        {
            string result = null;

            if (Age <= 0 || Age >= 150)
                result = "Wprowadź prawidłowy wiek";

            return result;
        }

        private string EmailValidation()
        {
            string result = null;

            if (string.IsNullOrEmpty(Email))
                result = "Wprowadź email";

            return result;
        }

        private string PositionValidation()
        {
            string result = null;

            if (string.IsNullOrEmpty(Position))
                result = "Wprowadź stanowisko";

            return result;
        }
    }
}
