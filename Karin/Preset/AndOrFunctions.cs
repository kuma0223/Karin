using System;
using System.Collections.Generic;
using System.Text;

namespace Karin.Preset
{
    /// <summary>
    /// 論理演算系スーパークラス
    /// </summary>
    abstract class KFunc_AndOrBase : IKarinFunction
    {
        public abstract string Name { get; }

        public object Execute(object[] args) {
            if (args.Length != 2) { throw new KarinException(Name + "関数の引数の数が不正です。"); }
            if (!(args[0] is bool)) { throw new KarinException(Name + "関数で'" + args[0] + "'を評価できません。"); }
            if (!(args[1] is bool)) { throw new KarinException(Name + "関数で'" + args[1] + "'を評価できません。"); }

            return compare((bool)args[0], (bool)args[1]);
        }
        public abstract bool compare(bool arg1, bool arg2);
    }

    class KFunc_AND : KFunc_AndOrBase
    {
        public override string Name { get { return "AND"; } }
        public override bool compare(bool arg1, bool arg2) { return arg1 && arg2; }
    }

    class KFunc_OR : KFunc_AndOrBase
    {
        public override string Name { get { return "OR"; } }
        public override bool compare(bool arg1, bool arg2) { return arg1 || arg2; }
    }

    class KFunc_NOT : IKarinFunction
    {
        public string Name { get { return "NOT"; } }

        public object Execute(object[] args) {
            if (args.Length != 1) { throw new KarinException(Name + "関数の引数の数が不正です。"); }
            if (!(args[0] is bool)) { throw new KarinException(Name + "関数で'" + args[0] + "'を評価できません。"); }
            return !(bool)args[0];
        }
    }
}
