using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Objects;

namespace Gridsum.NHBaseThrift.Messages
{
	/// <summary>
	///		插入行集合应答
	/// </summary>
	public class InsertNewRowsResponseMessage : ThriftMessage
	{
		#region Constructor.

        /// <summary>
		///     插入行集合应答
        /// </summary>
		public InsertNewRowsResponseMessage()
        {
            uint version = VERSION_1 | (uint)(MessageTypes.Reply);
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
		///     获取或设置IO错误信息
		/// </summary>
		[ThriftProperty(1, PropertyTypes.String)]
		public IOError IOErrorMessage { get; set; }
		/// <summary>
		///     获取或设置参数非法错误信息
		/// </summary>
		[ThriftProperty(2, PropertyTypes.String)]
		public IllegalArgumentError IllegalArgumentErrorMessage { get; set; }

        #endregion
	}
}
