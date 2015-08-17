using NHBaseThrift.Attributes;
using NHBaseThrift.Enums;
using NHBaseThrift.Objects;

namespace NHBaseThrift.Messages
{
	/// <summary>
	///		回收Scanner资源请求
	/// </summary>
	public class ScannerCloseRequestMessage : ThriftMessage
	{
		#region Constructor.

        /// <summary>
		///     Request mesasge for close scanner command.
        /// </summary>
		public ScannerCloseRequestMessage()
        {
            uint version = VERSION_1 | (uint)(MessageTypes.Call);
            Identity = new MessageIdentity
            {
				Command = "scannerClose",
                Version = (int)version
            };
            Identity.CommandLength = (uint)Identity.Command.Length;
        }

        #endregion

        #region Members.

		/// <summary>
		///		获取或设置ScannerID
		/// </summary>
		[ThriftProperty(1, PropertyTypes.I32)]
		public int ScannerId { get; set; }

		#endregion
	}
}
