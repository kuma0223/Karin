using System;
using System.Collections.Generic;
using System.Text;

namespace Karin.Preset
{
    /// <summary>
    /// 演算系スーパークラス
    /// </summary>
    abstract class KFunc_MathBase : IKarinFunction
    {
        public abstract string Name { get; }

        private double ToDouble(object v) {
            if (v is int) return (int)v;
            if (v is long) return (long)v;
            return (double)v;
        }
        private long ToLong(object v) {
            if (v is double) return (long)(double)v;
            if (v is int) return (int)v;
            return (long)v;
        }

        public object Execute(object[] args) {
            if (args.Length != 2) {
                throw new KarinException(Name + "関数の引数の数が不正です。");
            }

            foreach (var x in args) {
                if (x is int) continue;
                if (x is long) continue;
                if (x is double) continue;
                throw new KarinException($"{Name}の引数({x})が数値ではありません。");
            }

            if (args[0] is double || args[1] is double) {
                return calcDouble(ToDouble(args[0]), ToDouble(args[1]));
            } else if (args[0] is long || args[1] is long) {
                return calcLong(ToLong(args[0]), ToLong(args[1]));
            } else {
                return calcInt((int)args[0], (int)args[1]);
            }
        }
        public abstract object calcInt(int arg1, int arg2);
        public abstract object calcLong(long args1, long args2);
        public abstract object calcDouble(double arg1, double arg2);
    }

    class KFunc_ADD : KFunc_MathBase
    {
        public override string Name { get { return "ADD"; } }

        public override object calcInt(int arg1, int arg2) { return arg1 + arg2; }
        public override object calcLong(long arg1, long arg2) { return arg1 + arg2; }
        public override object calcDouble(double arg1, double arg2) { return arg1 + arg2; }

    }

    class KFunc_SUB : KFunc_MathBase
    {
        public override string Name { get { return "SUB"; } }

        public override object calcInt(int arg1, int arg2) { return arg1 - arg2; }
        public override object calcLong(long arg1, long arg2) { return arg1 - arg2; }
        public override object calcDouble(double arg1, double arg2) { return arg1 - arg2; }
    }

    class KFunc_MUL : KFunc_MathBase
    {
        public override string Name { get { return "MUL"; } }

        public override object calcInt(int arg1, int arg2) { return arg1 * arg2; }
        public override object calcLong(long arg1, long arg2) { return arg1 * arg2; }
        public override object calcDouble(double arg1, double arg2) { return arg1 * arg2; }
    }

    class KFunc_DIV : KFunc_MathBase
    {
        public override string Name { get { return "DIV"; } }

        public override object calcInt(int arg1, int arg2) {
            if (arg2 == 0) { throw new KarinException("0除算が発生しました。"); }
            return arg1 / arg2;
        }
        public override object calcLong(long arg1, long arg2) {
            if (arg2 == 0) { throw new KarinException("0除算が発生しました。"); }
            return arg1 / arg2;
        }
        public override object calcDouble(double arg1, double arg2) {
            if (arg2 == 0.0) { throw new KarinException("0除算が発生しました。"); }
            return arg1 / arg2;
        }
    }

    class KFunc_MOD : KFunc_MathBase
    {
        public override string Name { get { return "MOD"; } }

        public override object calcInt(int arg1, int arg2) {
            if (arg2 == 0) { throw new KarinException("0除算（剰余）が発生しました。"); }
            return arg1 % arg2;
        }
        public override object calcLong(long arg1, long arg2) {
            if (arg2 == 0) { throw new KarinException("0除算（剰余）が発生しました。"); }
            return arg1 % arg2;
        }
        public override object calcDouble(double arg1, double arg2) {
            if (arg2 == 0.0) { throw new KarinException("0除算（剰余）が発生しました。"); }
            return arg1 % arg2;
        }
    }

    class KFunc_POW : KFunc_MathBase
    {
        public override string Name { get { return "POW"; } }

        public override object calcInt(int arg1, int arg2) { return Math.Pow(arg1, arg2); }
        public override object calcLong(long arg1, long arg2) { return Math.Pow(arg1, arg2); }
        public override object calcDouble(double arg1, double arg2) { return Math.Pow(arg1, arg2); }
    }
}
