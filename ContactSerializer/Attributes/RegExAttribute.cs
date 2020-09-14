using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
