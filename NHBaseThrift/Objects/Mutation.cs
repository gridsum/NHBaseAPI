using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Contracts;
using Gridsum.NHBaseThrift.Enums;

namespace Gridsum.NHBaseThrift.Objects
{
    /// <summary>
    ///     列对象
    /// </summary>
    public class Mutation : ThriftObject
    {
        #region Constructor.

        /// <summary>
        ///     Mutation
        /// </summary>
        public Mutation()
        {
            WriteToWAL = true;
        }

        #endregion

        #region Members.

        /// <summary>
        ///     Gets or sets IsDelete
        /// </summary>
        [ThriftProperty(1, PropertyTypes.Bool)]
        public bool IsDelete { get; set; }
        /// <summary>
        ///     Gets or sets ColumnName
        /// </summary>
        [ThriftProperty(2, PropertyTypes.String)]
        public string ColumnName { get; set; }
        /// <summary>
        ///     Gets or sets Value
        /// </summary>
        [ThriftProperty(3, PropertyTypes.String)]
        public byte[] Value { get; set; }
        /// <summary>
        ///     Gets or sets WAL
        /// </summary>
        [ThriftProperty(4, PropertyTypes.Bool)]
        public bool WriteToWAL { get; set; }

        #endregion
    }
}