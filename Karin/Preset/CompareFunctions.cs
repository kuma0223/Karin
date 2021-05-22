using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Karin.Preset
{
    /// <summary>
    /// 比較系スーパークラス
    /// </summary>
    abstract class KFunc_CompareBase : IKarinFunction
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

            if(args[0] is string && args[1] is string) {
                return compareString((string)args[0], (string)args[1]);
            } else if (args[0] is bool || args[1] is bool) {
                return compareBool((bool)args[0], (bool)args[1]);
            } else if (args[0] is double || args[1] is double) {
                return compareDouble(ToDouble(args[0]), ToDouble(args[1]));
            } else if (args[0] is long || args[1] is long) {
                return compareLong(ToLong(args[0]), ToLong(args[1]));
            } else if (args[0] is int || args[1] is int) {
                return compareInt((int)args[0], (int)args[1]);
            } else {
                return compareOther(args[0], args[1]);
            }
        }

        public abstract bool compareInt(int arg1, int arg2);
        public abstract bool compareLong(long arg1, long arg2);
        public abstract bool compareDouble(double arg1, double arg2);
        public abstract bool compareString(string arg1, string arg2);
        public abstract bool compareBool(bool arg1, bool arg2);
        public abstract bool compareOther(object arg1, object arg2);
    }

    class KFunc_OVER : KFunc_CompareBase
    {
        public override string Name { get { return "OVER"; } }
        public override bool compareInt(int arg1, int arg2) { return arg1 > arg2; }
        public override bool compareLong(long arg1, long arg2) { return arg1 > arg2; }
        public override bool compareDouble(double arg1, double arg2) { return arg1 > arg2; }
        public override bool compareString(string arg1, string arg2) { throw new Exception(); }
        public override bool compareBool(bool arg1, bool arg2) { throw new Exception(); }
        public override bool compareOther(object arg1, object arg2) { throw new Exception(); }
    }

    class KFunc_EOVER : KFunc_CompareBase
    {
        public override string Name { get { return "EOVER"; } }
        public override bool compareInt(int arg1, int arg2) { return arg1 >= arg2; }
        public override bool compareLong(long arg1, long arg2) { return arg1 >= arg2; }
        public override bool compareDouble(double arg1, double arg2) { return arg1 >= arg2; }
        public override bool compareString(string arg1, string arg2) { throw new Exception(); }
        public override bool compareBool(bool arg1, bool arg2) { throw new Exception(); }
        public override bool compareOther(object arg1, object arg2) { throw new Exception(); }
    }

    class KFunc_UNDER : KFunc_CompareBase
    {
        public override string Name { get { return "UNDER"; } }
        public override bool compareInt(int arg1, int arg2) { return arg1 < arg2; }
        public override bool compareLong(long arg1, long arg2) { return arg1 < arg2; }
        public override bool compareDouble(double arg1, double arg2) { return arg1 < arg2; }
        public override bool compareString(string arg1, string arg2) { throw new Exception(); }
        public override bool compareBool(bool arg1, bool arg2) { throw new Exception(); }
        public override bool compareOther(object arg1, object arg2) { throw new Exception(); }
    }

    class KFunc_EUNDER : KFunc_CompareBase
    {
        public override string Name { get { return "EUNDER"; } }
        public override bool compareInt(int arg1, int arg2) { return arg1 <= arg2; }
        public override bool compareLong(long arg1, long arg2) { return arg1 <= arg2; }
        public override bool compareDouble(double arg1, double arg2) { return arg1 <= arg2; }
        public override bool compareString(string arg1, string arg2) { throw new Exception(); }
        public override bool compareBool(bool arg1, bool arg2) { throw new Exception(); }
        public override bool compareOther(object arg1, object arg2) { throw new Exception(); }
    }

    class KFunc_EQUAL : KFunc_CompareBase
    {
        public override string Name { get { return "EQUAL"; } }
        public override bool compareInt(int arg1, int arg2) { return arg1 == arg2; }
        public override bool compareLong(long arg1, long arg2) { return arg1 == arg2; }
        public override bool compareDouble(double arg1, double arg2) { return arg1 == arg2; }
        public override bool compareString(string arg1, string arg2) { return arg1 == arg2; }
        public override bool compareBool(bool arg1, bool arg2) { return arg1 == arg2; }
        public override bool compareOther(object arg1, object arg2) { return arg1 == arg2; }
    }

    class KFunc_NEQUAL : KFunc_CompareBase
    {
        public override string Name { get { return "NEQUAL"; } }
        public override bool compareInt(int arg1, int arg2) { return arg1 != arg2; }
        public override bool compareLong(long arg1, long arg2) { return arg1 != arg2; }
        public override bool compareDouble(double arg1, double arg2) { return arg1 != arg2; }
        public override bool compareString(string arg1, string arg2) { return arg1 != arg2; }
        public override bool compareBool(bool arg1, bool arg2) { return arg1 != arg2; }
        public override bool compareOther(object arg1, object arg2) { return arg1 != arg2; }
    }
}
