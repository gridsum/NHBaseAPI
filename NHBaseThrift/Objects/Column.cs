using NHBaseThrift.Attributes;
using NHBaseThrift.Contracts;
using NHBaseThrift.Enums;

namespace NHBaseThrift.Objects
{
    /// <summary>
    ///     Column
    /// </summary>
    public class Column : ThriftObject
    {
        #region Members.

        /// <summary>
        ///     Gets or sets column's name.
        /// </summary>
        [ThriftProperty(1, PropertyTypes.String)]
        public string Name { get; set; }
        /// <summary>
        ///     Gets or sets cell information.
        /// </summary>
        [ThriftProperty(2, PropertyTypes.Struct)]
        public Cell Cell { get; set; }

        #endregion
    }
}