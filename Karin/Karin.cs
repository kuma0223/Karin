using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Karin
{
    public class Karin
    {
        /// <summary>
        /// スクリプト実行中か否か
        /// </summary>
        public bool IsRunning { private set; get; }

        /// <summary>
        /// 関数テーブル
        /// </summary>
        private Dictionary<string, IKarinFunction> FunctionTable
            = new Dictionary<string, IKarinFunction>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 変数テーブル（グローバル）
        /// </summary>
        public Dictionary<string, object> Variables = new Dictionary<string, object>();

        /// <summary>
        /// 変数テーブル（スコープ）
        /// </summary>
        private Dictionary<string, object> ScopedVariables = new Dictionary<string, object>();

        /// <summary>
        /// 停止要求フラグ
        /// </summary>
        private bool StopFlag = false;

        /// <summary>
        /// 同時にひとつのスクリプトを実行できるインスタンスを生成します。
        /// </summary>
        public Karin() {
            //setFunction(PresetFunctions.executionAll);
            SetFunction(PresetFunctions.cat);
            SetFunction(PresetFunctions.add);
            SetFunction(PresetFunctions.sub);
            SetFunction(PresetFunctions.mul);
            SetFunction(PresetFunctions.div);
            SetFunction(PresetFunctions.mod);
            SetFunction(PresetFunctions.pow);
            SetFunction(PresetFunctions.equal);
            SetFunction(PresetFunctions.nequal);
            SetFunction(PresetFunctions.over);
            SetFunction(PresetFunctions.eover);
            SetFunction(PresetFunctions.under);
            SetFunction(PresetFunctions.eunder);
            SetFunction(PresetFunctions.and);
            SetFunction(PresetFunctions.or);
            SetFunction(PresetFunctions.true_);
            SetFunction(PresetFunctions.false_);
            SetFunction(PresetFunctions.null_);
            SetFunction(PresetFunctions.not);
            SetFunction(PresetFunctions.return_);
            SetFunction(PresetFunctions.toscript);
            SetFunction(PresetFunctions.date);
            SetFunction(PresetFunctions.format);
            SetFunction(PresetFunctions.int_);
            SetFunction(PresetFunctions.double_);
            SetFunction(PresetFunctions.method);
            SetFunction(PresetFunctions.property);
            /*
            //文字列をスクリプトとして実行する関数
            setFunction(new KourinFunction("DOSCRIPT", (args) => {
                if (args.Length == 0) throw new KourinException("DOSCRIPT関数の引数が不足しています。");
                return this.rideScript(args[0].ToString(), "DOSCRIPT");
            }));
            */
            SetFunction(PresetFunctions.if_);
            SetFunction(PresetFunctions.while_);
            SetFunction(PresetFunctions.repeat);
        }

        /// <summary>
        /// 拡張関数を設定します。
        /// </summary>
        public void SetFunction(IKarinFunction function) {
            FunctionTable[function.name] = function;
        }

        /// <summary>
        /// 関数を削除します。
        /// </summary>
        /// <param name="name">関数名</param>
        public void ReleaseFunction(string name) {
            if (FunctionTable.ContainsKey(name))
                FunctionTable.Remove(name);
        }

        /// <summary>
        /// スクリプトを停止します。
        /// このメソッドを呼んだ時点で実行されている処理ブロックが
        /// 完了した時点でKourinAbortExeptionが発生します。
        /// スクリプトが既に停止している場合、何も起りません。
        /// </summary>
        public void Stop() {
            StopFlag = true;
        }


        /// <summary>
        /// スクリプトを実行します。
        /// </summary>
        /// <param name="reader">リーダ</param>
        /// <returns>結果値</returns>
        public object Execute(TextReader reader) {
            return Execute(reader.ReadToEnd());
        }

        /// <summary>
        /// スクリプトを実行します。
        /// </summary>
        public object Execute(string script) {
            if (IsRunning) {
                throw new KarinException("既に実行中です。");
            }

            try {
                StopFlag = false;
                IsRunning = true;

                var ana = new TextAnalyzer(script, "root");
                ana.Analyze();

                var ret = Ride(ana.Tokens);

                if (ret is ReturnedObject) {
                    ret = (ret as ReturnedObject).value;
                }
                return ret;
            } finally {
                ScopedVariables.Clear();
                IsRunning = false;
            }
        }

        private object Ride(List<Token> tokens) {
            return null;
        }
    }
}
