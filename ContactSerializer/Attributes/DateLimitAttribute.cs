using System;

namespace ContactSerializer.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class DateLimitAttribute : Attribute
    {
        public DateLimitAttribute(int year, int month, int day)
        {
            Date = new DateTime(year, month, day);
        }

        public bool CheckData(DateTime dateTime)
        {
            return dateTime > Date ? true: false;
        }

        public DateTime Date { get; set; }
    }
}
