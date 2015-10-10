using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Contracts;
using Gridsum.NHBaseThrift.Enums;

namespace Gridsum.NHBaseThrift.Objects
{
	/// <summary>
	///		A Scan object is used to specify scanner parameters when opening a scanner.
	/// </summary>
	public class TScan : ThriftObject
	{
		#region Members.

		/// <summary>
		///     Get or set start row
		/// </summary>
		[ThriftProperty(1, PropertyTypes.String)]
		public byte[] StartRow { get; set; }

		/// <summary>
		///     Get or set stop row
		/// </summary>
		[ThriftProperty(2, PropertyTypes.String)]
		public byte[] StopRow { get; set; }

		/// <summary>
		///     Get or set timeStamp
		/// </summary>
		[ThriftProperty(3, PropertyTypes.I64)]
		public long? TimeStamp { get; set; }

		/// <summary>
		///		Get or set columns
		/// </summary>
		[ThriftProperty(4, PropertyTypes.List)]
		public string[] Columns { get; set; }

		/// <summary>
		///		Get or set caching
		/// </summary>
		[ThriftProperty(5, PropertyTypes.I32)]
		public int? Caching { get; set; }

		/// <summary>
		///		Get or set filter string
		/// </summary>
		[ThriftProperty(6, PropertyTypes.String)]
		public string FilterString { get; set; }

		/// <summary>
		///		Get or set batch size
		/// </summary>
		[ThriftProperty(7, PropertyTypes.I32)]
		public int? BatchSize { get; set; }

		/// <summary>
		///		Get or set sortColumns
		/// </summary>
		[ThriftProperty(8, PropertyTypes.Bool)]
		public bool? SortColumns { get; set; }

		/// <summary>
		///		Get or set reversed
		/// </summary>
		[ThriftProperty(9, PropertyTypes.Bool)]
		public bool? Reversed { get; set; }

		#endregion
	}
}
