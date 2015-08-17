using System.Collections.Generic;
using NHBaseThrift.Attributes;
using NHBaseThrift.Enums;
using NHBaseThrift.Objects;

namespace NHBaseThrift.Messages
{
	/// <summary>
	///		插入行集合请求
	/// </summary>
	public class InsertNewRowsRequestMessage : ThriftMessage
	{
		#region Constructor.

        /// <summary>
		///     插入行集合请求
        /// </summary>
		public InsertNewRowsRequestMessage()
        {
            uint version = VERSION_1 | (uint)(MessageTypes.Call);
            Identity = new MessageIdentity
            {
				Command = "mutateRows",
                Version = (int)version
            };
            Identity.CommandLength = (uint) Identity.Command.Length;
        }

        #endregion

        #region Members.

		/// <summary>
		///     获取或设置表名
		/// </summary>
		[ThriftProperty(1, PropertyTypes.String)]
		public string TableName { get; set; }

		/// <summary>
		///		获取或设置行集合
		/// </summary>
		[ThriftProperty(2, PropertyTypes.List)]
		public BatchMutation[] RowBatch { get; set; }

		/// <summary>
		///		获取或设置行集合属性
		/// </summary>
		
		[ThriftProperty(3, PropertyTypes.Map)]
		public Dictionary<string, string> Attributes { get; set; }

        #endregion
	}
}
