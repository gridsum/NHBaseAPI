using System.Collections.Generic;
using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Contracts;
using Gridsum.NHBaseThrift.Enums;

namespace Gridsum.NHBaseThrift.Objects
{
    /// <summary>
    ///    Row info
    /// </summary>
    public class RowInfo : ThriftObject
    {
        #region Members.

        /// <summary>
        ///    获取或设置行键
        /// </summary>
		[ThriftProperty(1, PropertyTypes.String)]
        public byte[] RowKey { get; set; }
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