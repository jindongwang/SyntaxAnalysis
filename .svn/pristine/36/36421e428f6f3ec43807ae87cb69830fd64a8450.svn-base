using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LexicalAnalysis2
{
    class JudgeChar
    {
        public JudgeChar()
        {
 
        }
        public bool isAlphabet(char c)
        {
            if (c >= 65 && c <= 90)
                return true;
            else if (c >= 97 && c <= 122)
                return true;
            return false;
        }
        public bool isDigit(char c)
        {
            if (c >= 48 && c <= 57)
                return true;
            return false;
        }
        public bool isAlphaOrDigit(char c)
        {
            if (isAlphabet(c) || isDigit(c))
            {
                return true;
            }
            return false;
        }
    }
}
