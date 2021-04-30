using System;
using System.Collections.Generic;
using System.Text;

namespace Karin
{
    /// <summary>
    /// 構文の最小単位
    /// </summary>
    public class Token
    {
        public TokenType Type;
        public string Text;
        public int Line;

        public Token(TokenType type, string text, int line)
        {
            this.Type = type;
            this.Text = text;
            this.Line = line;
        }
        public override string ToString()
        {
            return $"({Line})[{Type}]{Text}";
        }
    }

    public enum TokenType
    {
        Number,
        String,
        Function,
        Operator,
        LParen,
        RParen,
        Variable,
        ScriptFunction,
        ScriptBlockToken,
        End,
        Other,
    }

    class OperatorToken : Token
    {
        public Operator Operator;

        public OperatorToken(string text, int line, Operator targetOperator)
            : base(TokenType.Operator, text, line)
        {
            this.Operator = targetOperator;
        }
    }

    class VariableToken : Token
    {
        public string Name;
        public bool IsGlobal = false;

        public VariableToken(string text, int line, string varName, bool isGlobal)
            : base(TokenType.Variable, text, line)
        {
            this.Name = varName;
            this.IsGlobal = isGlobal;
        }
    }

    class FunctionToken : Token
    {
        public string Name;
        public List<List<Token>> Arguments;

        public bool IsPipe;

        public FunctionToken(string text, int line, string funcName, List<List<Token>> arguments, bool isPipe)
            : base(TokenType.Function, text, line)
        {
            this.Name = funcName;
            this.Arguments = arguments;
            this.IsPipe = isPipe;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{base.ToString()}");

            if (Arguments != null) {
                foreach(var args in Arguments) {
                    foreach (var arg in args) {
                        foreach(var s in arg.ToString().Split('\n', '\r')) {
                            if(s=="") continue;
                            sb.AppendLine();
                            sb.Append($"  {s}");
                        }
                    }
                    sb.AppendLine();
                    sb.Append("  ---");
                }
            }
            return sb.ToString();
        }
    }

    class ScriptFunctionToken : Token
    {
        public string Name;
        public List<Token> SubTokens;

        public ScriptFunctionToken(string text, int line, string funcName, List<Token> subTokens)
            : base(TokenType.ScriptFunction, text, line)
        {
            this.Name = funcName;
            this.SubTokens = subTokens;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{base.ToString()}");

            foreach (var token in SubTokens) {
                foreach (var s in token.ToString().Split('\n', '\r')) {
                    if (s == "") continue;
                    sb.AppendLine();
                    sb.Append($"  {s}");
                }
            }
            return sb.ToString();
        }
    }

    class ScriptBlockToken : Token
    {
        public List<Token> SubTokens;

        public ScriptBlockToken(string text, int line, List<Token> subTokens)
            : base(TokenType.ScriptBlockToken, text, line)
        {
            this.SubTokens = subTokens;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{base.ToString()}");

            foreach (var token in SubTokens) {
                foreach (var s in token.ToString().Split('\n', '\r')) {
                    if (s == "") continue;
                    sb.AppendLine();
                    sb.Append($"  {s}");
                }
            }
            return sb.ToString();
        }
    }
}
