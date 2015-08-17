using NHBaseThrift.Attributes;
using NHBaseThrift.Enums;
using NHBaseThrift.Objects;

namespace NHBaseThrift.Messages
{
    /// <summary>
    ///     Request mesasge for creating HBase table command.
    /// </summary>
    public class CreateTableRequestMessage : ThriftMessage
    {
        #region Constructor.

        /// <summary>
        ///     Request mesasge for creating HBase table command.
        /// </summary>
        public CreateTableRequestMessage()
        {
            uint version = VERSION_1 | (uint)(MessageTypes.Call);
            Identity = new MessageIdentity
            {
                Command = "createTable",
                Version = (int)version
            };
            Identity.CommandLength = (uint) Identity.Command.Length;
        }

        #endregion

        #region Members.

        /// <summary>
        ///     Gets or sets HBase table name you want to create.
        /// </summary>
        [ThriftProperty(1, PropertyTypes.String)]
        public string TableName { get; set; }
        /// <summary>
        ///     Gets or sets HBase table's column families you want use.
        /// </summary>
        [ThriftProperty(2, PropertyTypes.List)]
        public ColumnDescriptor[] ColumnFamilies { get; set; }

        #endregion
    }
}