using System;
using ContactSerializer.Attributes;
using ContactSerializer.Enums;
using NonSerializedAttribute = ContactSerializer.Attributes.NonSerializedAttribute;

namespace ContactSerializer.Models
{
    [Description("Contact")]
    [Serializable]
    public sealed class Contact : ICloneable
    {
        [StringLengthLimit()]
        public string LastName
        {
            get; set;
        }
        [StringLengthLimit()]
        public string FirstName
        {
            get; set;
        }
        [StringLengthLimit()]
        public string MiddleName
        {
            get; set;
        }
        public Gender Gender
        {
            get; set;
        }
        [DateLimit(year: 1999, month: 2, day: 22)]
        public DateTime Birthday
        {
            get; set;
        }
        public string ITN
        {
            get; set;
        }
        [RegEx(@"^\+?7(9\d{9})$")]
        public string PhoneNumber
        {
            get; set;
        }

        public Address Address
        {
            get; set;
        }

        [NonSerialized()]
        public int Age 
        {
            get => DateTime.Today.Year - Birthday.Year;
        }
        public Contact() { }

        public Contact(string lastName, string firstName, string middleName,
            Gender gender, DateTime birthday, string iTN, string phoneNumber, Address address)
        {
            LastName = lastName;
            FirstName = firstName;
            MiddleName = middleName;
            Gender = gender;
            Birthday = birthday;
            ITN = iTN;
            PhoneNumber = phoneNumber;
            Address = address;
        }

        public override string ToString()
        {
            return $"Contact with info {FirstName} {LastName} {MiddleName}";
        }
        public override bool Equals(object obj)
        {
            return obj is Contact contact &&
                   LastName.Equals(contact.LastName, StringComparison.OrdinalIgnoreCase) &&
                   FirstName.Equals(contact.FirstName, StringComparison.OrdinalIgnoreCase) &&
                   MiddleName.Equals(contact.MiddleName, StringComparison.OrdinalIgnoreCase) &&
                   Gender == contact.Gender &&
                   Birthday == contact.Birthday &&
                   ITN.Equals(contact.ITN, StringComparison.OrdinalIgnoreCase) &&
                   PhoneNumber.Equals(contact.PhoneNumber, StringComparison.OrdinalIgnoreCase) &&
                   Age == contact.Age;
        }

        public override int GetHashCode()
        {
            int hashCode = -2058314865;
            hashCode = hashCode * -1521134295 + LastName.GetHashCode();
            hashCode = hashCode * -1521134295 + FirstName.GetHashCode();
            hashCode = hashCode * -1521134295 + MiddleName.GetHashCode();
            hashCode = hashCode * -1521134295 + Gender.GetHashCode();
            hashCode = hashCode * -1521134295 + Birthday.GetHashCode();
            hashCode = hashCode * -1521134295 + ITN.GetHashCode();
            hashCode = hashCode * -1521134295 + PhoneNumber.GetHashCode();
            hashCode = hashCode * -1521134295 + Age.GetHashCode();
            return hashCode;
        }

        #region ICloneable

        public object Clone()
        {
            return new Contact
            {
                LastName = LastName,
                FirstName = FirstName,
                MiddleName = MiddleName,
                Gender = Gender,
                Birthday = Birthday,
                ITN = ITN,
                PhoneNumber = PhoneNumber
            };
        }

        #endregion
    }
}
