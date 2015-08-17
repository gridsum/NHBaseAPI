using System.Collections.Generic;
using NHBaseThrift.Attributes;
using NHBaseThrift.Enums;
using NHBaseThrift.Objects;

namespace NHBaseThrift.Messages
{
	/// <summary>
	///		获取指定行指定列应答
	/// </summary>
	public class GetRowWithColumnsResponseMessage : ThriftMessage
	{
				#region Constructor.

        /// <summary>
        ///     Request mesasge for creating HBase table command.
        /// </summary>
		public GetRowWithColumnsResponseMessage()
        {
            uint version = VERSION_1 | (uint)(MessageTypes.Reply);
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
