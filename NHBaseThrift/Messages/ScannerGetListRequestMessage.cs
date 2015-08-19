using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Objects;

namespace Gridsum.NHBaseThrift.Messages
{
    /// <summary>
    ///		根据scanner获取一批数据请求
    /// </summary>
    public class ScannerGetListRequestMessage : ThriftMessage
    {

        #region Constructor.

        /// <summary>
        ///     Request mesasge for scaning a row list.
        /// </summary>
        public ScannerGetListRequestMessage()
        {
            uint version = VERSION_1 | (uint)(MessageTypes.Call);
            Identity = new MessageIdentity
            {
                Command = "scannerGetList",
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
        /// <summary>
        ///     Gets or sets rows count you want to get.
        /// </summary>
        [ThriftProperty(2, PropertyTypes.I32)]
        public int RowsCount { get; set; }

        #endregion
    }
}
