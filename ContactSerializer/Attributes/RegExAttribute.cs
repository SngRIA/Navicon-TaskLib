using System;
using System.Text.RegularExpressions;

namespace ContactSerializer.Attributes
{
    public class RegExAttribute : Attribute
    {
        public RegExAttribute(string expression)
        {
            Expression = expression;
        }

        public bool CheckExp(string value)
        {
            return Regex.IsMatch(value, Expression) ? true : false;
        }

        public string Expression { get; set; }
    }
}
