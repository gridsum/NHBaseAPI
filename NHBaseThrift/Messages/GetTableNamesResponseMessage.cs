using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Objects;

namespace Gridsum.NHBaseThrift.Messages
{
    /// <summary>
    ///     获取目前HBase内部所有的表名应答消息
    /// </summary>
    public class GetTableNamesResponseMessage : ThriftMessage
    {
        #region Constructor.

        /// <summary>
        ///     获取目前HBase内部所有的表名请求消息
        /// </summary>
        public GetTableNamesResponseMessage()
        {
            uint version = VERSION_1 | (uint)(MessageTypes.Call);
            Identity = new MessageIdentity
            {
                Command = "getTableNames",
                Version = (int)version
            };
            Identity.CommandLength = (uint) Identity.Command.Length;
        }

        #endregion

        #region Members.

        /// <summary>
        ///     获取或设置HBase中所有的表名
        /// </summary>
        [ThriftProperty(0, PropertyTypes.List)]
        public string[] Tables { get; set; }
        /// <summary>
        ///     获取或设置IO错误信息
        /// </summary>
        [ThriftProperty(1, PropertyTypes.String)]
        public IOError IOErrorMessage { get; set; }

        #endregion
    }
}