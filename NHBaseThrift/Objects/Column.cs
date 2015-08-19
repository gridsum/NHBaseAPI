using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Contracts;
using Gridsum.NHBaseThrift.Enums;

namespace Gridsum.NHBaseThrift.Objects
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