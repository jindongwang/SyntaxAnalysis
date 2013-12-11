using System;
using System.Collections.Generic;
using System.Linq;

namespace LexicalAnalysis2
{
    class PublicVariables
    {
        //二维表
        static public int[,] matrix = new int[8, 8]{{1,1,-1,-1,-1,1,-1,1},{1,1,-1,-1,-1,1,-1,1},{1,1,1,1,-1,1,-1,1},{1,1,1,1,-1,1,-1,1},     
             {-1,-1,-1,-1,-1,0,-1,-2},{1,1,1,1,-2,1,-2,1},{1,1,1,1,-2,1,-2,1},{-1,-1,-1,-1,-1,-2,-1,0}};

        static public int Char2Int(char c)                    //字符转成表里的下标
        {
            switch (c)
            {
                case '+':
                    return 0;
                case '-':   
                    return 1;
                case '*':   
                    return 2;
                case '/':   
                    return 3;
                case '(':   
                    return 4;
                case ')':   
                    return 5;
                case 'i':    
                    return 6;
                case '#':  
                    return 7;
                default:   
                    return 0;
            }
        }    
    }
}
