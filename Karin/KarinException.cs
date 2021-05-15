using System;
using System.Collections.Generic;

namespace Karin
{
    /// <summary>
    /// スクリプト実行時例外
    /// </summary>
    public class KarinException : Exception
    {
        /// <summary>
        /// スクリプト内スタックトレース
        /// </summary>
        public string ScriptStackTrace { get; private set; } = "";

        /// <summary>
        /// スタックトレースの最新ブロック名
        /// </summary>
        internal string StackBlockName = null;

        public KarinException(string message, Exception ex) : base(message, ex)
        {
        }

        public KarinException(string message) : this(message, null)
        {
        }

        internal void AddStackTrace(int line, string block)
        {
            var s = block + "/line:"+line;
            if(ScriptStackTrace != "") {
                s = Environment.NewLine + s;
            }
            ScriptStackTrace += s;
            StackBlockName = block;
        }
    }

    /// <summary>
    /// スクリプト中断要求例外
    /// </summary>
    public class KarinAbortException : KarinException
    {
        public KarinAbortException(string message) : base(message)
        {
        }
    }
}
