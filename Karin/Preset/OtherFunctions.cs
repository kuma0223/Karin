using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Karin.Preset
{
    /// <summary>
    /// 文字列結合
    /// </summary>
    class KFunc_CAT : IKarinFunction
    {
        public string Name { get { return "CAT"; } }

        public object Execute(object[] args) {
            StringBuilder bil = new StringBuilder();
            foreach (var arg in args) {
                bil.Append(arg == null ? "" : arg.ToString());
            }
            return bil.ToString();
        }
    }
    
    /// <summary>
    /// 文字列フォーマット
    /// </summary>
    class KFunc_FORMAT : IKarinFunction
    {
        public string Name { get { return "FORMAT"; } }

        public object Execute(object[] args) {
            if (args.Length < 1) {
                throw new KarinException(Name + "関数の引数の数が不正です。");
            }
            if (!(args[0] is string)) {
                throw new KarinException(Name + "関数の第一引数が文字列ではありません。");
            }

            var objs = new object[args.Length - 1];
            for (int i = 1; i < args.Length; i++) { objs[i - 1] = args[i]; }
            try {
                return String.Format((string)args[0], objs);
            } catch (FormatException ex) {
                throw new KarinException(ex.Message);
            }
        }
    }

    /// <summary>
    /// 整数変換
    /// </summary>
    class KFunc_INT : IKarinFunction
    {
        public string Name { get { return "INT"; } }

        public object Execute(object[] args) {
            if (args.Length < 1) {
                return 0;
            }

            object obj = args[0];
            try {
                if (obj is string) {
                    var s = (string)obj;
                    if (s.StartsWith("0x")) {
                        return Convert.ToInt32(s.Substring(2), 16);
                    } else {
                        return Convert.ToInt32(s);
                    }
                } else if (obj is double) {
                    return (int)(double)obj;
                } else if (obj is float) {
                    return (int)(float)obj;
                } else if (obj is long) {
                    return (int)(long)obj;
                }
                return Convert.ToInt32(args[0]);
            } catch (FormatException) {
                throw new KarinException($"'{obj}'を整数へ変換できません。");
            }
        }
    }

    /// <summary>
    /// 実数変換
    /// </summary>
    class KFunc_DOUBLE : IKarinFunction
    {
        public string Name { get { return "DOUBLE"; } }

        public object Execute(object[] args) {
            if (args.Length < 1) {
                return 0.0;
            }

            object obj = args[0];
            try {
                if (obj is string){
                    return Convert.ToDouble((string)obj);
                }
                return Convert.ToDouble(args[0]);
            } catch (FormatException) {
                throw new KarinException($"'{obj}'を整数へ変換できません。");
            }
        }
    }

    /// <summary>
    /// 日時取得
    /// </summary>
    class KFunc_DATE : IKarinFunction
    {
        public string Name { get { return "DATE"; } }

        public object Execute(object[] args) {
            if (args.Length < 1) {
                return DateTime.Now;
            }

            DateTime d;
            object obj = args[0];

            if (obj is string && DateTime.TryParse((string)obj, out d)) {
                return d;
            }
            throw new KarinException($"'{obj}'を日時へ変更できません。");
        }
    }


    /// <summary>
    /// クラスメソッド呼び出し
    /// </summary>
    class KFunc_METHOD : IKarinFunction
    {
        public string Name { get { return "METHOD"; } }

        public object Execute(object[] args) {
            if (args.Length < 2) {
                throw new KarinException($"{this.Name}関数の引数が不足しています。");
            }

            var cls = args[0].GetType();
            var name = "" + args[1];

            System.Reflection.MethodInfo method = null;
            object[] param = null;

            if (args.Length > 2) {
                param = args.Where((obj, idx) => { return idx >= 2; }).ToArray();
                Type[] types = (from x in param select x.GetType()).ToArray();
                method = cls.GetMethod(name, types);
                if (method == null) method = cls.GetMethod(name);
            } else {
                method = cls.GetMethod(name, new Type[0]);
            }

            if (method == null) throw new KarinException("メソッド'" + name + "'が見つかりません。");
            return method.Invoke(args[0], param);
        }
    }

    /// <summary>
    /// クラスプロパティ呼び出し
    /// </summary>
    class KFunc_PROPERTY : IKarinFunction
    {
        public string Name { get { return "PROPERTY"; } }

        public object Execute(object[] args) {
            if (args.Length < 2) {
                throw new KarinException($"{this.Name}関数の引数が不足しています。");
            }

            var cls = args[0].GetType();
            var name = "" + args[1];

            var prop = cls.GetProperty(name);
            if (prop == null) throw new KarinException("プロパティ'" + name + "'が見つかりません。");

            if (args.Length > 2) {
                prop.SetValue(args[0], args[2]);
                return null;
            } else {
                return prop.GetValue(args[0]);
            }
        }
    }
}
