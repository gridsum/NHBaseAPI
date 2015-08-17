using KJFramework.Net.Channels.Events;
using KJFramework.Net.Channels.Objects;

namespace NHBaseThrift.Network
{
    /// <summary>
    ///     Thrift网络协议内部所使用的内存片段
    /// </summary>
    internal class ThriftSegmentNode : SegmentNode
    {
        #region Constructor.

        /// <summary>
        ///     Thrift网络协议内部所使用的内存片段
        /// </summary>
        /// <param name="value">Socket接收到的内存片段</param>
        public ThriftSegmentNode(SegmentReceiveEventArgs value)
            : base(value)
        {
        }

        #endregion

        #region Members.

        /// <summary>
        ///     获取或设置上一个节点
        /// </summary>
        public SegmentNode Previous { get; set; }

        #endregion
    }
}