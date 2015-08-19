using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Objects;

namespace Gridsum.NHBaseThrift.Messages
{
	/// <summary>
	///     关闭表操作应答
	/// </summary>
	public class DisableTableResponseMessage : ThriftMessage
	{
		#region Constructor.

        /// <summary>
        ///     关闭表操作应答
        /// </summary>
		public DisableTableResponseMessage()
        {
            uint version = VERSION_1 | (uint)(MessageTypes.Reply);
            Identity = new MessageIdentity
            {
				Command = "disableTable",
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
