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

        private static char[] Marks = { '+', '-', '*', '/', '%', '|', '&', '=', '!', '>', '<', /*';'*/ };
        private static HashSet<char> MarkSet = new HashSet<char>(Marks);

        public static Operator[] Operators = {
            new Operator(){ Mark="*",  Priority=5, Function=PresetFunctions.mul.name},
            new Operator(){ Mark="/",  Priority=5, Function=PresetFunctions.div.name},
            new Operator(){ Mark="%",  Priority=5, Function=PresetFunctions.mod.name},
            new Operator(){ Mark="+",  Priority=4, Function=PresetFunctions.add.name},
            new Operator(){ Mark="-",  Priority=4, Function=PresetFunctions.sub.name},

            new Operator(){ Mark="&",  Priority=3, Function=PresetFunctions.cat.name},

            new Operator(){ Mark=">",  Priority=2, Function=PresetFunctions.over.name},
            new Operator(){ Mark=">=", Priority=2, Function=PresetFunctions.eover.name},
            new Operator(){ Mark="<",  Priority=2, Function=PresetFunctions.under.name},
            new Operator(){ Mark="<=", Priority=2, Function=PresetFunctions.eunder.name},
            new Operator(){ Mark="==", Priority=1, Function=PresetFunctions.equal.name},
            new Operator(){ Mark="!=", Priority=1, Function=PresetFunctions.nequal.name},

            new Operator(){ Mark="&&", Priority=0, Function=PresetFunctions.and.name},
            new Operator(){ Mark="||", Priority=0, Function=PresetFunctions.or.name},
                                                 
            new Operator(){ Mark="=", Priority=-1},

            //;は直列実行演算子。左辺, 右辺と評価され、右辺の値を結果値として返す。
            //全ての演算子より優先度が低いが、括弧()よりは先に判定される。
            //new Operator(){ Mark=";", Priority=-99, Function=PresetFunctions.executionAll.name}
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
