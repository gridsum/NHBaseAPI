using System.Collections.Generic;
using NHBaseThrift.Attributes;
using NHBaseThrift.Contracts;
using NHBaseThrift.Enums;

namespace NHBaseThrift.Objects
{
    /// <summary>
    ///    HBase数据行信息
    /// </summary>
    public class RowInfo : ThriftObject
    {
        #region Members.

        /// <summary>
        ///    获取或设置行键
        /// </summary>
		[ThriftProperty(1, PropertyTypes.String)]
        public string RowKey { get; set; }
        /// <summary>
        ///    获取或设置列信息
        /// </summary>
		[ThriftProperty(2, PropertyTypes.Map)]
        public Dictionary<string, Cell> Columns { get; set; }
        /// <summary>
        ///    获取或设置有序列信息
        /// </summary>
		[ThriftProperty(3, PropertyTypes.List)]
		public Column[] SortedColumns { get; set; }

        #endregion
    }
}