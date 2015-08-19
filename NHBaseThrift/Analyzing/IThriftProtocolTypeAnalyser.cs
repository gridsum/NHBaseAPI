namespace Gridsum.NHBaseThrift.Analyzing
{
    /// <summary>
    ///     Thrift协议类型分析器元接口，提供了相关的基本操作。
    /// </summary>
    internal interface IThriftProtocolTypeAnalyser<out T, in K>
    {
        #region Methods.

        /// <summary>
        ///     分析一个类型中的所有Thrift成员属性
        /// </summary>
        /// <param name="type">要分析的类型</param>
        /// <returns>返回分析的结果</returns>
        T Analyse(K type);
        /// <summary>
        ///     清空当前所有的分析结果
        /// </summary>
        void Clear();

        #endregion
    }
}