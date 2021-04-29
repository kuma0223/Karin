using System;
using System.Collections.Generic;
using System.Text;

namespace Karin
{
    /// <summary>
    /// 変数用クラス
    /// </summary>
    class Variable
    {
        public VariableToken token;
        
        public Variable(VariableToken token) {
            this.token = token;
        }
    }
}
