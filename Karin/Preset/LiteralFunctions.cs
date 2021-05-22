using System;
using System.Collections.Generic;
using System.Text;

namespace Karin.Preset
{
    /// <summary>
    /// trueリテラル
    /// </summary>
    class KFunc_TRUE : IKarinFunction
    {
        public string Name { get { return "TRUE"; } }
        public object Execute(object[] args) { return true; }
    }

    /// <summary>
    /// falseリテラル
    /// </summary>
    class KFunc_FALSE : IKarinFunction
    {
        public string Name { get { return "FALSE"; } }
        public object Execute(object[] args) { return false; }
    }

    /// <summary>
    /// nullリテラル
    /// </summary>
    class KFunc_NULL : IKarinFunction
    {
        public string Name { get { return "NULL"; } }
        public object Execute(object[] args) { return null; }
    }
}
