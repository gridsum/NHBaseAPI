namespace Gridsum.NHBaseThrift.Objects
{
    /// <summary>
    ///    新数据列信息对象
    /// </summary>
    public class ColumnInfo
    {
        #region Members.

        /// <summary>
        ///    获取或设置列簇信息
        /// </summary>
        public string ColumnFamily { get; set; }
        /// <summary>
        ///    获取或设置列名
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        ///    获取或设置列值
        /// </summary>
        public byte[] Value { get; set; }

        #endregion
    }
}