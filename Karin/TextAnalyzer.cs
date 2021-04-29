using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karin
{
    public class TextAnalyzer
    {
        public string BlockName{
            private set;
            get;
        }
        
        public List<Token> Tokens {
            get { return tokens; }
        }

        private string src;
        private int position;
        private int line = 1;
        private List<Token> tokens = new List<Token>();

        //エスケープ対象文字
        private static Dictionary<char, char> EscTable = new Dictionary<char, char>(){
            { '"', '"' }, { '\\', '\\' }, { 'n', '\n' }, { 'r', '\r' }, { 't', '\t' }
        };

        //--------------------
        //Constructor

        public TextAnalyzer(string src, string blockName, int position=0) {
            this.src = src;
            this.BlockName = blockName;
            this.position = position;
        }

        public void Analyze() {
            Token t;
            while((t = Next()) != null) {
                if(t.Type == TokenType.Other) {
                    throw new KarinException("不明なトークンの出現です。", line, BlockName);
                }
            }

            if(tokens.Any() && tokens.Last().Type != TokenType.End) {
                tokens.Add(new Token(TokenType.End, "", line));
            }
        }

        //--------------------
        //Methods

        private Token Next() {            
            while(position < src.Length) {
                var c = src[position];

                if(IsSpace(c)) {
                    position++;
                    continue;
                }
                if (IsBreak(c)) {
                    position++;
                    if (!IsLFofCRLF(position-1)) {
                        line++;
                        if (tokens.Count>0 && tokens.Last().Type != TokenType.End) {
                            tokens.Add(new Token(TokenType.End, "", line));
                            return tokens.Last();
                        }
                    }
                    continue;
                }
                if(c == ';') {
                    position++;
                    if (tokens.Count > 0 && tokens.Last().Type != TokenType.End) {
                        tokens.Add(new Token(TokenType.End, "", line));
                        return tokens.Last();
                    }
                    continue;
                }

                if (_ANA_LineComment()) continue;

                Token token = null;
                if (token == null) token = _ANA_HereDocument();
                if (token == null) token = _ANA_String();
                if (token == null) token = _ANA_Number();
                if (token == null) token = _ANA_Operator();
                if (token == null) token = _ANA_Paren();
                if (token == null) token = _ANA_Variable();
                if (token == null) token = _ANA_Function();

                if (token == null) {
                    position++;
                    token = new Token(TokenType.Other, c.ToString(), line);
                }

                tokens.Add(token);
                return token;
            }
            return null;
        }
        
        //--------------------

        private char At(int i) {
            if(i < 0) return '\0';
            if(i >= src.Length) return '\0';
            return src[i];
        }

        private bool IsNumber(char c) {
            return '0' <= c && c <= '9';
        }

        private bool IsSpace(char c) {
            return c == ' ' || c == '\t';
        }

        private bool IsVariableName(char c) {
            return ('a' <= c && c <= 'z')
                || ('A' <= c && c <= 'Z')
                || ('1' <= c && c <= '9')
                || c == '_' || c == '.';
        }

        private bool IsFunctionName(char c) {
            return ('a' <= c && c <= 'z')
                || ('A' <= c && c <= 'Z')
                || ('1' <= c && c <= '9')
                || c == '_' || c == '.';
        }

        private bool IsBreak(char c) {
            return c=='\r' || c=='\n' || c=='\0';
        }

        private bool IsLFofCRLF(int i) {
            return (At(i) == '\n' && At(i-1) == '\r');
        }

        //--------------------
        //取り出し-行コメント
        private bool _ANA_LineComment() {
            if(At(position) == '/' && At(position+1) == '/') {
                while(!IsBreak(At(position))){
                    position++;
                }
                line++;
                return true;
            }
            return false;
        }
        
        //--------------------
        //取り出し-ヒアドキュメント
        private Token _ANA_HereDocument() {
            int i = position;
            var yes = At(i)=='$' && At(i+1)=='"' && IsBreak(At(i+2));
            if(!yes) return null;

            line++;            
            i += 3;
            if(IsLFofCRLF(i+1)) i++;
            
            var sb = new StringBuilder();
            while (true) {
                var c = At(i);
                if(c == '\0') {
                    throw new KarinException("ヒアドキュメントが閉じられていません。", line, BlockName);
                } else if(IsBreak(c) && At(i+1)=='"' && At(i+2)=='$'){
                    i+=3;
                    break;
                } else{
                    sb.Append(c);
                    if (IsBreak(c) && !IsLFofCRLF(i)) {
                        line++;
                    }
                }
                i++;
            }
            position = i;

            return new Token(TokenType.String, sb.ToString(), line);
        }
        
        //--------------------
        //取り出し-文字列
        private Token _ANA_String() {
            if(At(position) != '"') {
                return null;
            }
            position++;
            var buf = new StringBuilder();

            while(true){
                var c = At(position);

                if (IsBreak(c)) {
                    throw new KarinException("文字列が閉じられていません。", line, BlockName);
                } else if (c == '"') {
                    //エスケープは飛ばしてるので考慮不要
                    position++;
                    break;
                } else if (c == '\\') {
                    var cc = At(position+1);
                    if (!EscTable.ContainsKey(cc)) {
                        throw new KarinException($"不明なエスケープ文字'\\{cc}'です。", line, BlockName);
                    }
                    buf.Append(EscTable[cc]);
                    position+=2;
                } else {
                    buf.Append(c);
                    position++;
                }
            }

            return new Token(TokenType.String, buf.ToString(), line);
        }
        
        //--------------------
        //取り出し-数値
        private Token _ANA_Number() {
            var c1 = At(position);
            var c2 = At(position);
            var yes = (IsNumber(c1) || (c1 == '-' && IsNumber(c2)));
            if(!yes) return null;

            var sb = new StringBuilder();
            if(c1 == '-') {
                sb.Append(c1);
                position++;
            }
            
            var dec = false;
            
            while (true) {
                var c = At(position);
                if(c == '.') {
                    if (dec) {
                        throw new KarinException("不正な数値です。", line, BlockName);
                    }
                    dec = true;
                }else if (!IsNumber(c)) {
                    break;
                }
                sb.Append(c);
                position++;
            }
            return new Token(TokenType.Number, sb.ToString(), line);
        }
        
        //--------------------
        //取り出し-演算子
        private Token _ANA_Operator() {
            var c = At(position);
            if(!Operator.IsOeratorChar(c)) {
                return null;
            }

            string str;
            if (Operator.IsOeratorChar(At(position + 1))){
                str = src.Substring(position, 2);
                position+=2;
            } else {
                str = c.ToString();
                position+=1;
            }
            var ope = Operator.Parse(str);
            if(ope == null) {
                throw new KarinException($"不明な演算子'{str}'です。", line, BlockName);
            }
            return new OperatorToken(str, line, ope);
        }
        
        //--------------------
        //取り出し-括弧
        private Token _ANA_Paren() {
            var c = At(position);
            if (c == '(') {
                position++;
                return new Token(TokenType.LParen, "(", line);
            }
            if(c == ')') {
                position++;
                return new Token(TokenType.RParen, ")", line);
            }
            return null;
        }
        
        //--------------------
        //取り出し-変数
        private Token _ANA_Variable() {
            var c = At(position);
            if(c != '$') return null;
            position++;

            var global = false;
            if(At(position) == '$') {
                global = true;
                position++;
            }

            var sb = new StringBuilder();
            while (true) {
                c = At(position);
                if (!IsVariableName(c)) {
                    break;
                }
                position++;
                sb.Append(c);
            }
            if (sb.Length == 0) {
                throw new KarinException("変数名がありません。", line, BlockName);
            }

            var name = sb.ToString();
            var text = global ? "$$"+name : "$"+name;
            return new VariableToken(text, line, name, global);
        }
        
        //--------------------
        //取り出し-関数
        private Token _ANA_Function() {
            var c = At(position);
            var yes = (c == '[' || c == '{' || c == ':' || IsFunctionName(c));
            if(!yes) return null;

            bool isPipe = false;
            if(c == ':') {
                isPipe = true;
                position++;
                while(IsSpace(At(position)))
                    position++;
            }

            var sb = new StringBuilder();
            while (true) {
                c = At(position);
                if(!IsFunctionName(c)) break;

                sb.Append(c);
                position++;
            }

            var fname = sb.ToString();

            while (IsSpace(At(position)))
                position++;

            c = At(position);

            if (c == '{') {
                //ユーザ関数定義 | スクリプトブロック
                if (isPipe) {
                    throw new Exception("関数宣言にパイプを繋げることはできません。");
                }
                position++;
                
                if(sb.Length == 0) {
                    var subs = _ANA_Function_ReadBlock("script block");
                    return new ScriptBlockToken("", line, subs);
                } else {
                    var subs = _ANA_Function_ReadBlock(fname);
                    return new ScriptFunctionToken(fname, line, fname, subs);
                }
            }
            else if (c == '[') {
                //呼び出し
                position++;
                var args = _ANA_Function_ReadArgs();
                return new FunctionToken(fname, line, fname, args, isPipe);
            }
            else {
                //引数省略呼び出し
                return new FunctionToken(fname, line, fname, new List<List<Token>>(), isPipe);
            }
        }
        
        private List<Token> _ANA_Function_ReadBlock(string subBlockName) {
            var ana = new TextAnalyzer(src, subBlockName, position);
            var subs = new List<Token>();

            while (true) {
                Token t;
                try { 
                    t = ana.Next();
                }catch(KarinException ex) {
                    ex.AddStackTrace(line, BlockName);
                    throw;
                }

                if (t == null) {
                    throw new KarinException("ブロックが閉じられていません。", line, BlockName);
                }
                if (t.Type == TokenType.Other && t.Text == "}") {
                    break;
                }
                subs.Add(t);
            }
            position = ana.position;
            return subs;
        }

        private List<List<Token>> _ANA_Function_ReadArgs() {
            var args = new List<List<Token>>();

            Token t;
            while (true) {
                var ana = new TextAnalyzer(src, BlockName, position);
                var arg = new List<Token>();
                while (true) {
                    try {
                        t = ana.Next();
                    } catch (KarinException ex) {
                        ex.AddStackTrace(line, BlockName);
                        throw;
                    }

                    if (t == null) {
                        throw new KarinException("引数が閉じられていません。", line, BlockName);
                    }
                    if (t.Type == TokenType.Other && (t.Text == "," || t.Text == "]")) {
                        break;
                    }
                    arg.Add(t);
                }
                position = ana.position;
                args.Add(arg);
                if (t.Text == "]") {
                    break;
                }
            }
            return args;
        }
        
        /// <summary>
        /// 逆ポーランド記法変換
        /// </summary>
        public void ToRPN() {
            this.tokens = ToRPN(tokens);
        }

        /// <summary>
        /// 逆ポーランド記法変換
        /// </summary>
        private List<Token> ToRPN(List<Token> tokens) {
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
                        for(int i=0; i<t.Arguments.Count; i++){
                            t.Arguments[i] = ToRPN(t.Arguments[i]);
                        }
                    }else if(token.Type == TokenType.ScriptFunction) {
                        var t = token as ScriptFunctionToken;
                        t.SubTokens = ToRPN(t.SubTokens);
                    } else if (token.Type == TokenType.Subscript) {
                        var t = token as ScriptBlockToken;
                        t.SubTokens = ToRPN(t.SubTokens);
                    }
                }
            }
            return ret;
        }

    }
}
