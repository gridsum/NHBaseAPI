using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Objects;

namespace Gridsum.NHBaseThrift.Messages
{
	/// <summary>
	///		Atomically increment the column value specified.  Returns the next value post increment.
	/// </summary>
	public class AtomicIncrementRequestMessage : ThriftMessage
	{
		#region Constructor.

        /// <summary>
		///     Atomically increment the column value specified.  Returns the next value post increment.
        /// </summary>
		public AtomicIncrementRequestMessage()
        {
            uint version = VERSION_1 | (uint)(MessageTypes.Call);
            Identity = new MessageIdentity
            {
				Command = "atomicIncrement",
                Version = (int)version
            };
            Identity.CommandLength = (uint) Identity.Command.Length;
        }

        #endregion

        #region Members.

        /// <summary>
        ///     Gets or sets table name.
        /// </summary>
        [ThriftProperty(1, PropertyTypes.String)]
        public string TableName { get; set; }
		/// <summary>
		///     Gets or sets Rowkey.
		/// </summary>
		[ThriftProperty(2, PropertyTypes.String)]
		public byte[] RowKey { get; set; }
		/// <summary>
		///     Gets or sets Column.
		/// </summary>
		[ThriftProperty(3, PropertyTypes.String)]
		public string Column { get; set; }
		/// <summary>
		///     Gets or sets Value.
		/// </summary>
		[ThriftProperty(4, PropertyTypes.I64)]
		public long Value { get; set; }

        #endregion
	}
}
