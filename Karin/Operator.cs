﻿using System;
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
