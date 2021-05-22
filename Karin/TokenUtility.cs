using System;
using System.Collections.Generic;
using System.Text;

namespace Karin
{
    public class TokenUtility
    {
        /// <summary>
        /// 字句解析後チェック
        /// </summary>
        public static void Check(List<Token> tokens) {
            var isOperand = new Func<Token, bool>(t => {
                return t.Type == TokenType.String
                    || t.Type == TokenType.Number
                    || t.Type == TokenType.Function
                    || t.Type == TokenType.Variable;
            });

            int perCount = 0;
            int line = 0;
            string block = "";

            try {
                for (var i = 0; i < tokens.Count; i++) {
                    Token t = tokens[i];
                    line = t.Line;
                    block = t.Block;

                    if (t.Type == TokenType.Operator) {
                        if (i == 0 || i == tokens.Count - 1) {
                            throw new KarinException($"演算子の位置が不正です。'{t.Text}'");
                        } else if (tokens[i - 1].Type == TokenType.LParen) {
                            throw new KarinException($"演算子の位置が不正です。'{t.Text}'");
                        } else if (tokens[i - 1].Type == TokenType.Operator) {
                            throw new KarinException($"演算子が連続しています。'{t.Text}'");
                        }
                    } else if (t.Type == TokenType.Function && (t as FunctionToken).IsPipe) {
                        if (i == 0 || !(tokens[i - 1].Type == TokenType.RParen || isOperand(tokens[i - 1]))) {
                            throw new KarinException($"関数パイプの位置が不正です。'{t.Text}'");
                        }
                    } else if (isOperand(t)) {
                        if (i > 0 && (tokens[i - 1].Type == TokenType.RParen || isOperand(tokens[i - 1]))) {
                            throw new KarinException($"被演算子が連続しています。'{t.Text}'");
                        }
                    } else if (t.Type == TokenType.LParen) {
                        perCount++;
                        if (i > 0 && (tokens[i - 1].Type == TokenType.RParen || isOperand(tokens[i - 1]))) {
                            throw new KarinException($"被演算子が連続しています。'{t.Text}'");
                        }
                    } else if (t.Type == TokenType.RParen) {
                        perCount--;
                        if (i > 0 && tokens[i - 1].Type == TokenType.LParen) {
                            throw new KarinException("空の()です。");
                        }
                    } else if (t.Type == TokenType.Function) {
                        var token = t as FunctionToken;
                        foreach (var arg in token.Arguments) {
                            Check(arg);
                        }
                    } else if (t.Type == TokenType.ScriptBlockToken) {
                        var token = t as ScriptBlockToken;
                        Check(token.SubTokens);
                    } else if (t.Type == TokenType.ScriptFunction) {
                        var token = t as ScriptFunctionToken;
                        Check(token.SubTokens);
                    }
                }

                if (perCount != 0) {
                    throw new KarinException("()の対応がとれていません。");
                }
            } catch (KarinException ex) {
                if (ex.StackBlockName != block) {
                    ex.AddStackTrace(line, block);
                }
                throw;
            }
        }

        /// <summary>
        /// 逆ポーランド記法変換
        /// </summary>
        public static List<Token> ToRPN(List<Token> tokens) {
            var ret = new List<Token>(tokens.Count);  //結果格納
            var opeStack = new Stack<Token>();        //演算子スタック

            foreach (var token in tokens) {
                if (token.Type == TokenType.Operator) {
                    //要素は演算子
                    var to = (OperatorToken)token;

                    //スタック内の優先順位が以上の演算子をすべて結果に移動する
                    while (true) {
                        if (opeStack.Count == 0) { break; }
                        if (opeStack.Peek().Type == TokenType.LParen) { break; }
                        if ((opeStack.Peek() as OperatorToken).Operator.Priority < to.Operator.Priority) {
                            break;
                        }
                        ret.Add(opeStack.Pop());
                    }
                    opeStack.Push(token);
                } else if (token.Type == TokenType.LParen) {
                    //要素は左括弧
                    opeStack.Push(token);
                } else if (token.Type == TokenType.RParen) {
                    //要素は右括弧
                    //左括弧までの全演算子を結果に格納する
                    while (opeStack.Count > 0) {
                        var ope = opeStack.Pop();
                        if (ope.Type == TokenType.LParen) {
                            break;
                        } else {
                            ret.Add(ope);
                        }
                    }
                } else if (token.Type == TokenType.End) {
                    //要素は終端
                    //スタックに残った演算子をすべて結果に移動する
                    while (opeStack.Count > 0) {
                        ret.Add(opeStack.Pop());
                    }
                    ret.Add(token);
                } else {
                    //要素はその他の項目
                    ret.Add(token);

                    if(token.Type == TokenType.Function) {
                        var t = token as FunctionToken;
                        for(int i=0; i<t.Arguments.Length; i++){
                            t.Arguments[i] = ToRPN(t.Arguments[i]);
                        }
                    }else if(token.Type == TokenType.ScriptFunction) {
                        var t = token as ScriptFunctionToken;
                        t.SubTokens = ToRPN(t.SubTokens);
                    } else if (token.Type == TokenType.ScriptBlockToken) {
                        var t = token as ScriptBlockToken;
                        t.SubTokens = ToRPN(t.SubTokens);
                    }
                }
            }

            //最後にスタックに残った演算子をすべて結果に移動する
            while (opeStack.Count > 0) {
                ret.Add(opeStack.Pop());
            }

            return ret;
        }
        
    }
}
