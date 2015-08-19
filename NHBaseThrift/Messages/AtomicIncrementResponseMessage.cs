using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Objects;

namespace Gridsum.NHBaseThrift.Messages
{
	/// <summary>
	///		Atomically increment the column value specified.  Returns the next value post increment.
	/// </summary>
	public class AtomicIncrementResponseMessage : ThriftMessage
	{
		 #region Constructor.

        /// <summary>
		///     Atomically increment the column value specified.  Returns the next value post increment.
        /// </summary>
		public AtomicIncrementResponseMessage()
        {
            uint version = VERSION_1 | (uint)(MessageTypes.Reply);
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
		///		Get or set the next value post increment
		/// </summary>
		[ThriftProperty(0, PropertyTypes.I64)]
		public long Success { get; set; }
        /// <summary>
		///     Get or set IOError
        /// </summary>
        [ThriftProperty(1, PropertyTypes.String)]
        public IOError IOErrorMessage { get; set; }
        /// <summary>
		///     Get or set IllegalArgumentError
        /// </summary>
        [ThriftProperty(2, PropertyTypes.String)]
        public IllegalArgumentError IllegalArgumentErrorMessage { get; set; }
        
        #endregion
	}
}
