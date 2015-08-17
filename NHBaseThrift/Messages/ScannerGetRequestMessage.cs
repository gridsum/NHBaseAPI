using NHBaseThrift.Attributes;
using NHBaseThrift.Enums;
using NHBaseThrift.Objects;

namespace NHBaseThrift.Messages
{
    /// <summary>
    ///		根据scanner获取一批数据请求
    /// </summary>
    public class ScannerGetRequestMessage : ThriftMessage
    {

        #region Constructor.

        /// <summary>
        ///     Request mesasge for scaning a row list.
        /// </summary>
        public ScannerGetRequestMessage()
        {
            uint version = VERSION_1 | (uint)(MessageTypes.Call);
            Identity = new MessageIdentity
            {
                Command = "scannerGet",
                Version = (int)version
            };
            Identity.CommandLength = (uint)Identity.Command.Length;
        }

        #endregion

        #region Members.

        /// <summary>
        ///     Gets or sets scanner id you want to use.
        /// </summary>
        [ThriftProperty(1, PropertyTypes.I32)]
        public int Id { get; set; }

        #endregion
    }
}
