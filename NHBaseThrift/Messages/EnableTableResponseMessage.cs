using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Objects;

namespace Gridsum.NHBaseThrift.Messages
{
	/// <summary>
	///		Response mesasge for enable HBase table command.
	/// </summary>
	public class EnableTableResponseMessage : ThriftMessage
	{
		#region Constructor.

        /// <summary>
		///     Response mesasge for enable HBase table command.
        /// </summary>
		public EnableTableResponseMessage()
        {
            uint version = VERSION_1 | (uint)(MessageTypes.Reply);
            Identity = new MessageIdentity
            {
				Command = "enableTable",
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
        public string IOErrorMessage { get; set; }

		#endregion
	}
}
