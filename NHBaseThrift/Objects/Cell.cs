using NHBaseThrift.Attributes;
using NHBaseThrift.Contracts;
using NHBaseThrift.Enums;

namespace NHBaseThrift.Objects
{
    /// <summary>
    ///     Cell
    /// </summary>
    public class Cell : ThriftObject
    {
        #region Members.
        
        /// <summary>
        ///     Gets or sets specified cell value.
        /// </summary>
        [ThriftProperty(1, PropertyTypes.String)]
        public byte[] Value { get; set; }
        /// <summary>
        ///     Gets or sets timestamp.
        /// </summary>
        [ThriftProperty(2, PropertyTypes.I64)]
        public long Timestamp { get; set; }

        #endregion
    }
}