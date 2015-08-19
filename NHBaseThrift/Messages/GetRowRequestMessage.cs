using System.Collections.Generic;
using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Objects;

namespace Gridsum.NHBaseThrift.Messages
{
	/// <summary>
	///		获取一行数据请求
	/// </summary>
	public class GetRowRequestMessage : ThriftMessage
	{
        
        #region Constructor.

        /// <summary>
        ///     Request mesasge for inserting a new row into specified HBase table.
        /// </summary>
        public GetRowRequestMessage()
        {
            uint version = VERSION_1 | (uint)(MessageTypes.Call);
            Identity = new MessageIdentity
            {
                Command = "getRow",
                Version = (int)version
            };
            Identity.CommandLength = (uint)Identity.Command.Length;
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
        ///     Gets or sets HBase table's column families you want use.
        /// </summary>
        [ThriftProperty(3, PropertyTypes.Map)]
        public Dictionary<string, string> Attributes { get; set; }

        #endregion
	}
}
