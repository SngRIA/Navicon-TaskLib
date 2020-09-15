using System;

namespace ContactSerializer.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class StringLengthLimitAttribute : Attribute
    {
        public StringLengthLimitAttribute(int length = 15)
        {
            Length = length;
        }

        public bool CheckLength(int length)
        {
            return Length > length ? true : false;
        }

        public int Length { get; set; }
    }
}
