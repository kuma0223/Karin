using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Karin.Preset;
using System.Linq;

namespace Karin
{
    public class KarinEngine
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

        //◆━━━━━━━━━━━━━━━━━━━━━━━━━━━━◆

        /// <summary>
        /// 同時にひとつのスクリプトを実行できるインスタンスを生成します。
        /// </summary>
        public KarinEngine() {
            foreach(var f in PresetFunctions.Create()) {
                SetFunction(f);
            }
            /*
            //文字列をスクリプトとして実行する関数
            setFunction(new KourinFunction("DOSCRIPT", (args) => {
                if (args.Length == 0) throw new KourinException("DOSCRIPT関数の引数が不足しています。");
                return this.rideScript(args[0].ToString(), "DOSCRIPT");
            }));
            */
        }

        /// <summary>
        /// 拡張関数を設定します。
        /// </summary>
        public void SetFunction(IKarinFunction function) {
            FunctionTable[function.Name] = function;
        }

        /// <summary>
        /// 関数を削除します。
        /// </summary>
        /// <param name="name">関数名</param>
        public void RemoveFunction(string name) {
            if (FunctionTable.ContainsKey(name))
                FunctionTable.Remove(name);
        }

        /// <summary>
        /// スクリプトを停止します。
        /// このメソッドを呼んだ時点で実行されている処理ブロックが
        /// 完了した時点でKarinAbortExeptionが発生します。
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

                //字句解析
                var ana = new TextAnalyzer(script, "script root");
                ana.Analyze();

                TokenUtility.Check(ana.Tokens, "script root");
                var rpn = TokenUtility.ToRPN(ana.Tokens);

                //実行
                var ret = Ride(rpn, "script root");

                if (ret is ReturnedObject) {
                    ret = (ret as ReturnedObject).Value;
                }
                return ret;
            } finally {
                ScopedVariables.Clear();
                IsRunning = false;
            }
        }

        private object Ride(List<Token> tokens, string blockName) {

            //引数が変数なら変数テーブルから値を取得
            //そうでなければそのまま返す
            Func<object, object> getVarIf = (arg) => {
                if (arg is Variable) {
                    var token = ((Variable)arg).token;
                    var table = token.IsGlobal ? Variables : ScopedVariables;
                    if (!table.ContainsKey(token.Name)) {
                        throw new KarinException($"変数'{token.Name}'が初期化されていません。");
                    }
                    return table[token.Name];
                } else {
                    return arg;
                }
            };

            var stack = new Stack<object>();
            object last = null;

            //計算実行
            foreach (var token in tokens) {
                if (token.Type == TokenType.Number) {
                    //数値リテラル
                    object box;
                    if (!ToNumber(token.Text, out box)) {
                        throw new KarinException($"'{token}'を数値へ変換できません。");
                    }
                    stack.Push(box);
                }
                else if (token.Type == TokenType.String) {
                    //文字列リテラル
                    stack.Push(token.Text);
                }
                else if (token.Type == TokenType.Function) {
                    //関数呼び出し
                    var ft = token as FunctionToken;
                    if (ft.IsPipe){
                        var pipeobj = getVarIf(stack.Pop());
                        stack.Push(CallFunction(ft, pipeobj));
                    } else {
                        stack.Push(CallFunction(ft, null));
                    } 
                }
                else if (token.Type == TokenType.Operator) {
                    //演算子
                    var t = (OperatorToken)token;
                    var args = new object[2];

                    args[1] = stack.Pop(); //後ろの数の方が後入れされている
                    args[0] = stack.Pop();

                    if (t.Operator.Mark == "=") {
                        //代入
                        var va = (Variable)args[0];
                        var tbl = va.token.IsGlobal ? Variables : ScopedVariables;
                        tbl[va.token.Name] = getVarIf(args[1]);
                        stack.Push(null);
                    } else {
                        //代入以外
                        //対応する関数を呼ぶ
                        args[0] = getVarIf(args[0]);
                        args[1] = getVarIf(args[1]);
                        stack.Push(CallFunction(t.Operator.Function, args));
                    }
                }
                else if (token.Type == TokenType.Variable) {
                    //変数
                    //変数オブジェクトを作ってスタック
                    //（出現時点では代入か取得か不明）
                    stack.Push(new Variable((VariableToken)token));
                }
                else if (token.Type == TokenType.ScriptBlockToken) {
                    //スクリプトブロック
                    //すぐ実行して結果をスタック
                    var t = (ScriptBlockToken)token;
                    var r = Ride(t.SubTokens, "script block");
                    stack.Push(r);
                }
                else if(token.Type == TokenType.ScriptFunction) {
                    //ユーザー関数宣言
                    var t = (ScriptFunctionToken)token;
                    var fd = FunctionTable;
                    if (fd.ContainsKey(t.Name) && !(fd[t.Name] is ScriptFunction)) {
                        throw new KarinException($"静的登録関数({t.Name})は上書きできません。");
                    }
                    SetFunction(new ScriptFunction(t.Name, t.SubTokens));
                    stack.Push(null);
                }
                else if(token.Type == TokenType.End) {
                    //終端
                    //結果値を取り出してスタックをクリア
                    if (stack.Count > 1) {
                        throw new KarinException("単一の演算結果が得られませんでした。");
                    }
                    last = stack.Any() ? stack.Pop() : null;
                    stack.Clear();
                }
            }

            return getVarIf(last);
        }
        
        /// <summary>
        /// 関数呼び出し
        /// </summary>
        private object CallFunction(FunctionToken token, object pipedObj)
        {
            return null;
        }
        
        /// <summary>
        /// 関数呼び出し
        /// </summary>
        private object CallFunction(string funcName, object[] args) {
            if (!FunctionTable.ContainsKey(funcName)) {
                throw new KarinException($"関数'{funcName}'が見つかりません。");
            }

            var func = FunctionTable[funcName];
            object ret;
            if (func is ScriptFunction) {
                //スクリプト関数

                //現在のスコープ変数を退避
                //新規にスコープを作成して引数を追加
                var table = ScopedVariables;
                ScopedVariables = new Dictionary<string, object>();
                for (var i = 0; i < args.Length; i++){
                    ScopedVariables["args" + i] = args[i];
                }

                //実行
                ret = Ride((func as ScriptFunction).Tokens, func.Name);

                //スコープ変数を戻す
                ScopedVariables = table;

                //RETURNオブジェクト場合は値を復元する
                if (ret is ReturnedObject){
                    ret = ((ReturnedObject)ret).Value;
                }
            } else {
                //静的登録関数
                ret = func.Execute(args);
            }
            return ret;
        }

        /// <summary>
        /// 数値リテラル生成
        /// </summary>
        private bool ToNumber(string txt, out object box) {
            if (txt.IndexOf(".") > 0) {
                double d;
                var ret = double.TryParse(txt, out d);
                box = d;
                return ret;
            } else if (txt.StartsWith("0x")) {
                int i;
                var ret = int.TryParse(txt.Substring(2)
                    , System.Globalization.NumberStyles.HexNumber
                    , null, out i);
                box = i;
                return ret;
            } else {
                int i;
                var ret = int.TryParse(txt, out i);
                box = i;
                return ret;
            }
        }
    }
}
