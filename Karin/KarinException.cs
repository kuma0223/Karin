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

        public KarinException(string message, Exception ex, int line, string block) : base(message, ex)
        {
            if(line > 0) {            
                AddStackTrace(line, block);
            }
        }
        
        public KarinException(string message, Exception ex)
            : this(message, ex, 0, null)
        { }
        public KarinException(string message)
            : this(message, null)
        { }
        //public KarinException(string message, int line, string block)
        //    : this(message, null, line, block)
        //{ }

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
