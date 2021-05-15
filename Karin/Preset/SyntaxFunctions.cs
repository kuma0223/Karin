using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Karin.Preset
{
    /// <summary>
    /// 制御構文用の関数インタフェース
    /// </summary>
    interface IKarinSyntaxFunction : IKarinFunction
    {
        /// <summary>
        /// パイプ呼び出しが可能か否かを返します。
        /// </summary>
        bool PipeAcceptable { get; }

        /// <summary>
        /// 関数を呼び出し結果を返します。
        /// </summary>
        /// <param name="karin">実行エンジン</param>
        /// <param name="token">トークン</param>
        /// <param name="pipedObj">パイプオブジェクト</param>
        /// <returns>結果値</returns>
        object Execute(KarinEngine engine, FunctionToken token, object pipedObj);

    }

    /// <summary>
    /// IF構文関数
    /// </summary>
    class KFunc_IF : IKarinSyntaxFunction
    {
        public string Name {
            get { return "IF"; }
        }

        public bool PipeAcceptable {
            get { return false; }
        }

        public object Execute(object[] args) {
            throw new NotImplementedException();
        }

        public object Execute(KarinEngine engine, FunctionToken token, object pipedObj) {
            var args = token.Arguments;

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
        public string Name {
            get { return "REPEAT"; }
        }

        public bool PipeAcceptable {
            get { return false; }
        }

        public object Execute(object[] args) {
            throw new NotImplementedException();
        }

        public object Execute(KarinEngine engine, FunctionToken token, object pipedObj) {
            var args = token.Arguments;
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
        public string Name {
            get { return "WHILE"; }
        }

        public bool PipeAcceptable {
            get { return false; }
        }

        public object Execute(object[] args) {
            throw new NotImplementedException();
        }

        public object Execute(KarinEngine engine, FunctionToken token, object pipedObj) {
            var args = token.Arguments;

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
    /// スクリプト取得
    /// </summary>
    class KFunc_TOSCRIPT : IKarinSyntaxFunction
    {   
        public string Name {
            get { return "TOSCRIPT"; }
        }

        public bool PipeAcceptable {
            get { return false; }
        }

        public object Execute(object[] args) {
            throw new NotImplementedException();
        }

        public object Execute(KarinEngine engine, FunctionToken token, object pipedObj) {
            if(token.Arguments.Length != 1) {
                throw new KarinException($"{Name}関数の引数の数が不正です。");
            }

            var ptn = new System.Text.RegularExpressions.Regex($@"{token.Name}\s*\[([\s\S]*)\]");
            var mc = ptn.Match(token.Text);
            return mc.Groups[1].Value;
        }
    }
    /// <summary>
    /// スクリプト実行
    /// </summary>
    class KFunc_DOSCRIPT : IKarinSyntaxFunction
    {
        public string Name {
            get { return "DOSCRIPT"; }
        }

        public bool PipeAcceptable {
            get { return true; }
        }

        public object Execute(object[] args) {
            throw new NotImplementedException();
        }

        public object Execute(KarinEngine engine, FunctionToken token, object pipedObj) {
            if(!((token.IsPipe && token.Arguments.Length==0)
                || (!token.IsPipe && token.Arguments.Length == 1))) {
                throw new KarinException($"{Name}関数の引数の数が不正です。");
            }
            
            var s = token.IsPipe ? pipedObj : engine.Ride(token.Arguments[0]);
            if(!(s is string)) {
                throw new KarinException($"構文関数'{token.Name}'を実行できません。");
            }
            return engine.Eval((string)s);
        }
    }

    /// <summary>
    /// 文字列に変数を展開する
    /// </summary>
    class KFunc_DEPLOY : IKarinSyntaxFunction
    {
        public string Name {
            get { return "DEPLOY"; }
        }

        public bool PipeAcceptable {
            get { return true; }
        }

        public object Execute(object[] args) {
            throw new NotImplementedException();
        }

        public object Execute(KarinEngine engine, FunctionToken token, object pipedObj) {
            if(!((token.IsPipe && token.Arguments.Length==0)
                || (!token.IsPipe && token.Arguments.Length == 1))) {
                throw new KarinException($"{Name}関数の引数の数が不正です。");
            }
            
            var arg = token.IsPipe ? pipedObj : engine.Ride(token.Arguments[0]);

            if(!(arg is string)){
                throw new KarinException($"{Name}関数の引数が文字列ではありません。");
            }

            var s = (string)arg;
            var sb = new StringBuilder(s.Length);
            int pre = 0;
            var ptn = new Regex(@"{\s*(\$\$?[^\s\$\{\}]+)\s*}");

            foreach (Match m in ptn.Matches(s)) {
                var val = engine.Eval(m.Value);
                sb.Append(s.Substring(pre, m.Index-pre));
                sb.Append(val);
                pre = m.Index + m.Length;
            }
            sb.Append(s.Substring(pre, s.Length-pre));
            return sb.ToString();
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
}
