using System.Collections.Generic;
using NHBaseThrift.Attributes;
using NHBaseThrift.Enums;
using NHBaseThrift.Objects;

namespace NHBaseThrift.Messages
{
    /// <summary>
    ///     Request mesasge for inserting a new row into specified HBase table.
    /// </summary>
    public class InsertNewRowRequestMessage : ThriftMessage
    {
        #region Constructor.

        /// <summary>
        ///     Request mesasge for inserting a new row into specified HBase table.
        /// </summary>
        public InsertNewRowRequestMessage()
        {
            uint version = VERSION_1 | (uint)(MessageTypes.Call);
            Identity = new MessageIdentity
            {
                Command = "mutateRow",
                Version = (int)version
            };
            Identity.CommandLength = (uint)Identity.Command.Length;
        }

        #endregion

        #region Members.

        /// <summary>
        ///     Gets or sets HBase table name you want to create.
        /// </summary>
        [ThriftProperty(1, PropertyTypes.String)]
        public string TableName { get; set; }
        /// <summary>
        ///     Gets or sets HBase table name you want to create.
        /// </summary>
        [ThriftProperty(2, PropertyTypes.String)]
        public byte[] RowKey { get; set; }
        /// <summary>
        ///     Gets or sets HBase table's column families you want use.
        /// </summary>
        [ThriftProperty(3, PropertyTypes.List)]
        public Mutation[] Mutations { get; set; }
        /// <summary>
        ///     Gets or sets HBase table's column families you want use.
        /// </summary>
        [ThriftProperty(4, PropertyTypes.Map)]
        public Dictionary<string, string> Attributes { get; set; }

        #endregion
    }
}