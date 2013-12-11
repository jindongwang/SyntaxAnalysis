using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LexicalAnalysis2
{
    class SystemSymbol
    {

        public string[] key;
        public string[] opt;
        public string[] prept;
        public string[] reln;
        public bool searchWord(int type, string word)
        {
            switch (type)
            {
                case 1: return this.key.Contains(word);
                case 2: return this.opt.Contains(word); 
                case 3: return this.prept.Contains(word); 
                case 4: return this.reln.Contains(word);
                default: return false;
            }
        }
    }
}
