using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Objects;

namespace Gridsum.NHBaseThrift.Messages
{
    /// <summary>
    ///     获取一个表所分布在HBase的区域范围应答消息
    /// </summary>
    public class GetTableRegionsResponseMessage : ThriftMessage
    {
        #region Constructor.

        /// <summary>
        ///     获取一个表所分布在HBase的区域范围应答消息
        /// </summary>
        public GetTableRegionsResponseMessage()
        {
            uint version = VERSION_1 | (uint)(MessageTypes.Reply);
            Identity = new MessageIdentity
            {
                Command = "getTableRegions",
                Version = (int)version
            };
            Identity.CommandLength = (uint) Identity.Command.Length;
        }

        #endregion

        #region Members.

        /// <summary>
        ///     获取或设置Region分布信息列表
        /// </summary>
        [ThriftProperty(0, PropertyTypes.List)]
        public Region[] Regions { get; set; }
        /// <summary>
        ///     获取或设置IO错误信息
        /// </summary>
        [ThriftProperty(1, PropertyTypes.String)]
        public IOError IOErrorMessage { get; set; }

        #endregion
    }
}