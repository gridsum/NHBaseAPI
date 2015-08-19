using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Objects;

namespace Gridsum.NHBaseThrift.Messages
{
	/// <summary>
	///		按照RowKey范围查询数据应答
	/// </summary>
	public class ScannerOpenWithStopResponseMessage : ThriftMessage
	{
		#region Constructor.

        /// <summary>
		///     按照RowKey范围查询数据应答
        /// </summary>
		public ScannerOpenWithStopResponseMessage()
        {
            uint version = VERSION_1 | (uint)(MessageTypes.Reply);
            Identity = new MessageIdentity
            {
				Command = "scannerOpenWithStop",
                Version = (int)version
            };
            Identity.CommandLength = (uint) Identity.Command.Length;
        }

        #endregion

        #region Members.


		/// <summary>
		///     获取或设置表名
		/// </summary>
		[ThriftProperty(0, PropertyTypes.I32)]
		public int ScannerId { get; set; }
		/// <summary>
		///     获取或设置IO错误信息
		/// </summary>
		[ThriftProperty(1, PropertyTypes.String)]
		public IOError IOErrorMessage { get; set; }

        #endregion
	}
}
