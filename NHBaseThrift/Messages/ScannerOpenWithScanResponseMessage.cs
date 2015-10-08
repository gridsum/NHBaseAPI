using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Objects;

namespace Gridsum.NHBaseThrift.Messages
{
	/// <summary>
	///		Response corresponding scanner ID to the ScannerOpenWithScan request
	/// </summary>
	class ScannerOpenWithScanResponseMessage : ThriftMessage
	{
		#region Constructor.

		/// <summary>
		///		Response corresponding scanner ID to the ScannerOpenWithScan request
		/// </summary>
		public ScannerOpenWithScanResponseMessage()
		{
			uint version = VERSION_1 | (uint)(MessageTypes.Reply);
			Identity = new MessageIdentity
			{
				Command = "scannerOpenWithScan",
				Version = (int)version
			};
			Identity.CommandLength = (uint)Identity.Command.Length;
		}

		#endregion

		#region Members.

		/// <summary>
		///     Set or get the table name
		/// </summary>
		[ThriftProperty(0, PropertyTypes.I32)]
		public int ScannerId { get; set; }

		/// <summary>
		///     Set or get IOError message
		/// </summary>
		[ThriftProperty(1, PropertyTypes.String)]
		public IOError IOErrorMessage { get; set; }

		#endregion
	}
}
