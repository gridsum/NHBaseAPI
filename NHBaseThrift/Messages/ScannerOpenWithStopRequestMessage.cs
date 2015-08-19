using System.Collections.Generic;
using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Objects;

namespace Gridsum.NHBaseThrift.Messages
{
	/// <summary>
	///		Get a scanner on the current table starting and stopping at the specified rows.
	///		ending at the last row in the table. Return the specified columns.
	/// </summary>
	public class ScannerOpenWithStopRequestMessage : ThriftMessage
	{
		#region Constructor.

		/// <summary>
		///		Get a scanner on the current table starting and stopping at the specified rows.
		///		ending at the last row in the table. Return the specified columns.
		/// </summary>
		public ScannerOpenWithStopRequestMessage()
		{
			uint version = VERSION_1 | (uint)(MessageTypes.Call);
			Identity = new MessageIdentity
			{
				Command = "scannerOpenWithStop",
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
		///		获取或设置开始RowKey
		/// </summary>
		[ThriftProperty(2, PropertyTypes.String)]
		public byte[] StartRow { get; set; }

		/// <summary>
		///		获取或设置终止RowKey
		/// </summary>
		[ThriftProperty(3, PropertyTypes.String)]
		public byte[] EndRow { get; set; }

		/// <summary>
		///		获取或设置可选列名
		/// </summary>
		[ThriftProperty(4, PropertyTypes.List)]
		public string[] Columns { get; set; }

		/// <summary>
		///		获取或设置Atrribute
		/// </summary>
		[ThriftProperty(5, PropertyTypes.Map)]
		public Dictionary<string, string> Attribute { get; set; } 

        #endregion
	}
}
