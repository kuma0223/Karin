using System;
using System.Collections.Generic;
using System.Text;

namespace Karin.Preset
{
    /// <summary>
    /// IF関数
    /// </summary>
    class KFunc_IF : IKarinSyntaxFunction
    {
        public string Name { get { return "IF"; } }

        public object Execute(object[] args) {
            throw new NotImplementedException();
        }

        public object Execute(KarinEngine karin, object[] args) {
            if (args.Length < 2) {
                throw new KarinException(Name + "関数の引数の数が不正です。");
            }

            if (!(args[0] is bool)) {
                throw new KarinException(Name + "関数で'" + args[0] + "'を評価できません。");
            }
            if ((bool)args[0]) { return args[1]; } else { return args.Length < 3 ? "NULL" : args[2]; }
        }
    }

    /// <summary>
    /// REPEAT関数（特別関数：実装はEngine参照）
    /// </summary>
    class KFunc_REPEAT : IKarinSyntaxFunction
    {
        public string Name { get { return "REPEAT"; } }

        public object Execute(object[] args) {
            throw new NotImplementedException();
        }

        public object Execute(KarinEngine karin, object[] args) {
            if (args.Length != 2) {
                throw new KarinException(Name + "関数の引数の数が不正です。");
            }
            if (!(args[0] is int)) {
                throw new KarinException(Name + "関数で'" + args[0] + "'を評価できません。");
            }
            return null;
        }
    }

    /// <summary>
    /// WHILE関数（特別関数：実装はEngine参照）
    /// </summary>
    class KFunc_WHILE : IKarinSyntaxFunction
    {
        public string Name { get { return "WHILE"; } }

        public object Execute(object[] args) {
            throw new NotImplementedException();
        }

        public object Execute(KarinEngine karin, object[] args) {
            if (args.Length != 2) {
                throw new KarinException(Name + "関数の引数の数が不正です。");
            }
            return null;
        }
    }

    /// <summary>
    /// RETRUN関数
    /// </summary>
    class KFunc_RETURN : IKarinSyntaxFunction
    {
        public string Name { get { return "RETURN"; } }

        public object Execute(object[] args) {
            throw new NotImplementedException();
        }

        public object Execute(KarinEngine karin, object[] args) {
            if (args.Length == 0) return new ReturnedObject(null);
            else {
                if (args[0] is ReturnedObject) { 
                    throw new KarinException(Name + "関数の引数にReturnedObjectを指定できません。");
                }
                return new ReturnedObject(args[0]);
            }
        }
    }

    /// <summary>
    /// リターン用オブジェクト
    /// </summary>
    class ReturnedObject
    {
        public object Value { get; set; }

        public ReturnedObject(object value) {
            this.Value = value;
        }
    }

    /// <summary>
    /// スクリプト化（特別関数：実装はEngine参照）
    /// </summary>
    class KFunc_TOSCRIPT : IKarinFunction
    {   //関数名チェック用ダミー
        public string Name { get { return "TOSCRIPT"; } }
        public object Execute(object[] args) { return null; }
    }
}
