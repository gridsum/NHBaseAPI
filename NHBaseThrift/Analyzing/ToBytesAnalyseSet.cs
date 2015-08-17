namespace NHBaseThrift.Analyzing
{
    /// <summary>
    ///     序列化分析结果集
    /// </summary>
    internal sealed class ToBytesAnalyseSet
    {
        #region Constructor

        /// <summary>
        ///     序列化分析结果集
        /// </summary>
        /// <param name="initSize">初始化长度</param>
        public ToBytesAnalyseSet(int initSize)
        {
            InitializeSize = initSize;
        }

        #endregion

        #region Members

        /// <summary>
        ///     预计算智能字段存档
        /// </summary>
        public ToBytesAnalyseResult[] AnalyseProperties { get; set; }
        /// <summary>
        ///     获取或设置指定类型参与运算的初始化长度
        ///     <para>* 这个长度是当前类型所有可预算字段长度的总和再外加上一些辅助数据</para>
        /// </summary>
        public int InitializeSize { get; private set; }

        #endregion
    }
}