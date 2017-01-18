using System.Collections.Generic;
using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Objects;

namespace Gridsum.NHBaseThrift.Messages
{
	/// <summary>
	///		获取指定行指定列请求
	/// </summary>
	public class GetRowWithColumnsRequestMessage : ThriftMessage
	{
		#region Constructor.

        /// <summary>
        ///     Request mesasge for creating HBase table command.
        /// </summary>
		public GetRowWithColumnsRequestMessage()
        {
            uint version = VERSION_1 | (uint)(MessageTypes.Call);
            Identity = new MessageIdentity
            {
				Command = "getRowWithColumns",
                Version = (int)version
            };
            Identity.CommandLength = (uint) Identity.Command.Length;
        }

        #endregion

        #region Members.


		/// <summary>
		///     Gets or sets HBase table name you want to create.
		/// </summary>
		[ThriftProperty(1, PropertyTypes.String)]
		public string TableName { get; set; }
		/// <summary>
		///     Gets or sets HBase table name you want to create.
		/// </summary>
		[ThriftProperty(2, PropertyTypes.String)]
		public byte[] RowKey { get; set; }
		/// <summary>
		///		Gets or sets List of columns to return, null for all columns
		/// </summary>
		[ThriftProperty(3, PropertyTypes.List)]
		public string[] Columns { get; set; }
		/// <summary>
		///     Gets or sets HBase table's column families you want use.
		/// </summary>
		[ThriftProperty(4, PropertyTypes.Map)]
		public Dictionary<string, string> Attributes { get; set; }

		#endregion
	}
}
