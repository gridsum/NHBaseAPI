using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Objects;

namespace Gridsum.NHBaseThrift.Messages
{
    /// <summary>
    ///		根据scanner获取一批数据请求
    /// </summary>
    public class ScannerGetListResponseMessage : ThriftMessage
    {
        #region Constructor.

        /// <summary>
        ///     Request mesasge for scaning a row list.
        /// </summary>
        public ScannerGetListResponseMessage()
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
        ///     获取或设置行信息
        /// </summary>
        [ThriftProperty(0, PropertyTypes.List)]
        public RowInfo[] RowInfos { get; set; }
        /// <summary>
        ///     获取或设置IO错误信息
        /// </summary>
        [ThriftProperty(1, PropertyTypes.String)]
        public IOError IOErrorMessage { get; set; }
        /// <summary>
        ///     获取或设置参数非法错误信息
        /// </summary>
        [ThriftProperty(2, PropertyTypes.String)]
        public IllegalArgumentError IllegalArgumentErrorMessage { get; set; }

        #endregion
    }
}
