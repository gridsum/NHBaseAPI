using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Contracts;
using Gridsum.NHBaseThrift.Enums;

namespace Gridsum.NHBaseThrift.Objects
{
    /// <summary>
    ///     BatchMutation
    /// </summary>
    public class BatchMutation : ThriftObject
    {
        #region Members.

        /// <summary>
        ///     Gets or sets
        /// </summary>
        [ThriftProperty(1, PropertyTypes.String)]
        public byte[] RowKey { get; set; }
        /// <summary>
        ///     Gets or sets
        /// </summary>
        [ThriftProperty(2, PropertyTypes.List)]
        public Mutation[] Mutations { get; set; }

        #endregion
    }
}