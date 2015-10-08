using System.Collections.Generic;
using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Objects;

namespace Gridsum.NHBaseThrift.Messages
{
	/// <summary>
	///		Get a scanner on the current table, using the filter string
	/// </summary>
	public class ScannerOpenWithScanRequestMessage : ThriftMessage
	{
		#region Constructor.

		/// <summary>
		///		Get a scanner on the current table, using the filter string
		/// </summary>
		public ScannerOpenWithScanRequestMessage()
		{
			uint version = VERSION_1 | (uint)(MessageTypes.Call);
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
		///		获取或设置表名
		/// </summary>
		[ThriftProperty(1, PropertyTypes.String)]
		public string TableName { get; set; }

		/// <summary>
		///		获取或设置表名
		/// </summary>
		[ThriftProperty(2, PropertyTypes.Struct)]
		public TScan Scan { get; set; }
		
		/// <summary>
		///		获取或设置Atrribute
		/// </summary>
		[ThriftProperty(3, PropertyTypes.Map)]
		public Dictionary<string, string> Attribute { get; set; } 

		#endregion

	}
}
