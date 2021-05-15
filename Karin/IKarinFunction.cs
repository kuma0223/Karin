using System;
using System.Collections.Generic;

namespace Karin
{
    /// <summary>
    /// 関数インタフェース
    /// </summary>
    public interface IKarinFunction
    {
        /// <summary>
        /// 関数名を取得します。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 関数を呼び出し結果を返します。
        /// </summary>
        /// <param name="args">引数</param>
        /// <returns>結果値</returns>
        object Execute(object[] args);
    }

    /// <summary>
    /// 簡易IKarinFunction実装クラス
    /// </summary>
    public class KarinFunction : IKarinFunction
    {
        public string Name { private set; get; }
        private Func<object[], object> logic;

        /// <summary>
        /// 関数名と実装式を指定してインスタンスを生成します。
        /// </summary>
        /// <param name="name">関数名称</param>
        /// <param name="logic">関数実装</param>
        public KarinFunction(string name, Func<object[], object> logic)
        {
            this.Name = name;
            this.logic = logic;
        }

        /// <summary>
        /// 関数を呼び出し結果を返します。
        /// </summary>
        /// <param name="args">引数</param>
        /// <returns>結果値</returns>
        public object Execute(object[] args)
        {
            return logic(args);
        }
    }

    /// <summary>
    /// スクリプト内定義関数用クラス
    /// </summary>
    class ScriptFunction : IKarinFunction
    {
        public string Name { private set; get; }
        public List<Token> Tokens { private set; get; }

        public ScriptFunction(string name, List<Token> tokens)
        {
            this.Name = name;
            this.Tokens = tokens;
        }
        public object Execute(object[] args)
        {
            throw new KarinException("Can't call script function direct.");
        }
    }
}
