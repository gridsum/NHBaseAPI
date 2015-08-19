using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Objects;

namespace Gridsum.NHBaseThrift.Messages
{
    /// <summary>
    ///     Request mesasge for deleting HBase table command.
    /// </summary>
    public class DeleteTableRequestMessage : ThriftMessage
    {
        #region Constructor.

        /// <summary>
        ///     Request mesasge for deleting HBase table command.
        /// </summary>
        public DeleteTableRequestMessage()
        {
            uint version = VERSION_1 | (uint)(MessageTypes.Call);
            Identity = new MessageIdentity
            {
                Command = "deleteTable",
                Version = (int)version
            };
            Identity.CommandLength = (uint) Identity.Command.Length;
        }

        #endregion

        #region Members.

        /// <summary>
        ///     Gets or sets HBase table name you want to delete.
        /// </summary>
        [ThriftProperty(1, PropertyTypes.String)]
        public string TableName { get; set; }

        #endregion
    }
}