using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LexicalAnalysis2
{
    public class Formula
    {
        public Formula()
        {
 
        }
        public Formula(string type, string value, string label)
        {
            this.type = type;
            this.value = value;
            this.label = label;
        }
        public string type;
        public string value;
        public string label;
        public string convertToString()
        {
            return "(" + type + "," + value + ")" + "\r\n";
        }
    }
}
