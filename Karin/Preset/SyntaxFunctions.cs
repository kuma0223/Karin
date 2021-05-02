using System;
using System.Collections.Generic;
using System.Text;

namespace Karin.Preset
{
    /// <summary>
    /// IF構文関数
    /// </summary>
    class KFunc_IF : IKarinSyntaxFunction
    {
        public string Name { get { return "IF"; } }

        public object Execute(object[] args) {
            throw new NotImplementedException();
        }

        public object Execute(KarinEngine engine, List<Token>[] args) {
            if (!(2 <= args.Length || args.Length <= 3)) {
                throw new KarinException($"{Name}関数の引数の数が不正です。");
            }

            //条件を演算
            var sw = engine.Ride(args[0]);

            if (!(sw is bool)) {
                throw new KarinException($"{Name}関数の条件を評価できません。");
            }

            //どちらかを実行
            if ((bool)sw) {
                return engine.Ride(args[1]);
            } else {
                return args.Length == 3 ? engine.Ride(args[2]) : null;
            }
        }
    }

    /// <summary>
    /// REPEAT構文関数
    /// </summary>
    class KFunc_REPEAT : IKarinSyntaxFunction
    {
        public string Name { get { return "REPEAT"; } }

        public object Execute(object[] args) {
            throw new NotImplementedException();
        }

        public object Execute(KarinEngine engine, List<Token>[] args) {
            if (args.Length != 2) {
                throw new KarinException($"{Name}関数の引数の数が不正です。");
            }

            //回数条件
            var co = engine.Ride(args[0]);
            
            //繰り返し実行
            object ret = null;
            for (var c = 0; c < (int)co; c++) {
                ret = engine.Ride(args[1]);
                if (ret is ReturnedObject) break;
            }
            return ret;
        }
    }

    /// <summary>
    /// WHILE構文関数
    /// </summary>
    class KFunc_WHILE : IKarinSyntaxFunction
    {
        public string Name { get { return "WHILE"; } }

        public object Execute(object[] args) {
            throw new NotImplementedException();
        }

        public object Execute(KarinEngine engine, List<Token>[] args) {
            if (args.Length != 2) {
                throw new KarinException($"{Name}関数の引数の数が不正です。");
            }

            object ret = null;
            while (true) {
                object loop = engine.Ride(args[0]);
                if (!(loop is bool)) {
                    throw new KarinException($"{Name}関数の条件を評価できません。");
                }
                if ((bool)loop == false) {
                    break;
                }

                ret = engine.Ride(args[1]);
                if (ret is ReturnedObject) break;
            }
            return ret;
        }
    }

    /// <summary>
    /// RETRUN関数
    /// </summary>
    class KFunc_RETURN : IKarinFunction
    {
        public string Name { get { return "RETURN"; } }

        public object Execute(object[] args) {
            if (args.Length == 0){
                return new ReturnedObject(null);
            }
            if(args.Length == 1) {
                if (args[0] is ReturnedObject) {
                    return args[0];
                } else {
                    return new ReturnedObject(args[0]);
                }
            }
            throw new KarinException($"{Name}関数の引数の数が不正です。");
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
    {   
        public string Name { get { return "TOSCRIPT"; } }

        public object Execute(object[] args) {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// スクリプト実行（特別関数：実装はEngine参照）
    /// </summary>
    class KFunc_DOSCRIPT : IKarinFunction
    {
        public string Name { get { return "DOSCRIPT"; } }

        public object Execute(object[] args) {
            throw new NotImplementedException();
        }
    }
}
