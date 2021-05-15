using System.Collections.Generic;

namespace Karin
{
    /// <summary>
    /// 関数プラグインインタフェース
    /// </summary>
    public interface IPluginFunctionFactory
    {
        /// <summary>
        /// プラグインを判別するキーワードです。
        /// </summary>
        string KeyName { get; }

        /// <summary>
        /// エンジンに登録する追加関数クラスのインスタンスを返します。
        /// </summary>
        IEnumerable<IKarinFunction> GetPluginFunctions();
    }
}
