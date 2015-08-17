using NHBaseThrift.Attributes;
using NHBaseThrift.Enums;
using NHBaseThrift.Objects;

namespace NHBaseThrift.Messages
{
	/// <summary>
	///		获取一行数据应答
	/// </summary>
	public class GetRowResponseMessage : ThriftMessage
	{
        
        #region Constructor.

        /// <summary>
        ///     Request mesasge for inserting a new row into specified HBase table.
        /// </summary>
        public GetRowResponseMessage()
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
        ///     获取或设置行信息
        /// </summary>
        [ThriftProperty(0, PropertyTypes.List)]
        public RowInfo[] RowInfos { get; set; }
        /// <summary>
        ///     获取或设置IO错误信息
        /// </summary>
        [ThriftProperty(1, PropertyTypes.String)]
        public IOError IOErrorMessage { get; set; }

        #endregion
	}
}
