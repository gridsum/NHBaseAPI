namespace Gridsum.NHBaseThrift.Network
{
    /// <summary>
    ///    网络数据内部检查结果
    /// </summary>
    internal struct NetworkDataCheckResult
    {
        #region Members.

        /// <summary>
        ///    获取指定数据长度所涉及的内存片段个数
        /// </summary>
        public int SegmentCount;

        #endregion
    }
}