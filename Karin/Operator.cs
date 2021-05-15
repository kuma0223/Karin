using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Karin
{
    class Operator
    {
        public string Mark;
        public int Priority;
        public string Function;

        private static char[] Marks = { '+', '-', '*', '/', '%', '|', '&', '=', '!', '>', '<' };
        private static HashSet<char> MarkSet = new HashSet<char>(Marks);

        public static Operator[] Operators = {
            new Operator(){ Mark="*",  Priority=5, Function="MUL"},
            new Operator(){ Mark="/",  Priority=5, Function="DIV"},
            new Operator(){ Mark="%",  Priority=5, Function="MOD"},
            new Operator(){ Mark="+",  Priority=4, Function="ADD"},
            new Operator(){ Mark="-",  Priority=4, Function="SUB"},

            new Operator(){ Mark="&",  Priority=3, Function="CAT"},

            new Operator(){ Mark=">",  Priority=2, Function="OVER"},
            new Operator(){ Mark=">=", Priority=2, Function="EOVER"},
            new Operator(){ Mark="<",  Priority=2, Function="UNDER"},
            new Operator(){ Mark="<=", Priority=2, Function="EUNDER"},
            new Operator(){ Mark="==", Priority=1, Function="EQUAL"},
            new Operator(){ Mark="!=", Priority=1, Function="NEQUAL"},

            new Operator(){ Mark="&&", Priority=0, Function="AND"},
            new Operator(){ Mark="||", Priority=0, Function="OR"},
                                                 
            new Operator(){ Mark="=", Priority=-1},
        };

        public static Operator Parse(string str)
        {
            return Operators.FirstOrDefault(x => x.Mark == str);
        }

        public static bool IsOeratorChar(char c)
        {
            return MarkSet.Contains(c);
        }
    }
}
