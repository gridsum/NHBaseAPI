using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Objects;

namespace Gridsum.NHBaseThrift.Messages
{
	/// <summary>
	///		删除HBase表的应答消息
	/// </summary>
	public class DeleteTableResponseMessage : ThriftMessage
	{
		#region Constructor.

        /// <summary>
        ///     删除HBase表的应答消息
        /// </summary>
		public DeleteTableResponseMessage()
        {
            uint version = VERSION_1 | (uint)(MessageTypes.Reply);
            Identity = new MessageIdentity
            {
				Command = "deleteTable",
                Version = (int)version
            };
            Identity.CommandLength = (uint) Identity.Command.Length;
        }

        #endregion

        #region Members.

        /// <summary>
        ///     获取或设置IO错误信息
        /// </summary>
        [ThriftProperty(1, PropertyTypes.String)]
        public IOError IOErrorMessage { get; set; }

		#endregion
	}
}
