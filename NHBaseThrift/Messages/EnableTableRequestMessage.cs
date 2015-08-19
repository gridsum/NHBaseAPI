using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Objects;

namespace Gridsum.NHBaseThrift.Messages
{
    /// <summary>
    ///     Request mesasge for enable HBase table command.
    /// </summary>
    public class EnableTableRequestMessage : ThriftMessage
    {
        #region Constructor.

        /// <summary>
        ///     Request mesasge for enable HBase table command.
        /// </summary>
        public EnableTableRequestMessage()
        {
            uint version = VERSION_1 | (uint)(MessageTypes.Call);
            Identity = new MessageIdentity
            {
                Command = "enableTable",
                Version = (int)version
            };
            Identity.CommandLength = (uint)Identity.Command.Length;
        }

        #endregion

        #region Members.

        /// <summary>
        ///     Gets or sets HBase table name you want to disable.
        /// </summary>
        [ThriftProperty(1, PropertyTypes.String)]
        public string TableName { get; set; }

        #endregion
    }
}