using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
