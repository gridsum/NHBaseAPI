using NHBaseThrift.Attributes;
using NHBaseThrift.Enums;
using NHBaseThrift.Objects;

namespace NHBaseThrift.Messages
{
    /// <summary>
    ///     获取一个表所分布在HBase的区域范围请求消息
    /// </summary>
    public class GetTableRegionsRequestMessage : ThriftMessage
    {
        #region Constructor.

        /// <summary>
        ///     获取一个表所分布在HBase的区域范围请求消息
        /// </summary>
        public GetTableRegionsRequestMessage()
        {
            uint version = VERSION_1 | (uint)(MessageTypes.Call);
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
        ///     获取或设置表名
        /// </summary>
        [ThriftProperty(1, PropertyTypes.String)]
        public string TableName { get; set; }

        #endregion
    }
}