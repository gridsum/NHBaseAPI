using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Objects;

namespace Gridsum.NHBaseThrift.Messages
{
    /// <summary>
    ///     获取目前HBase内部所有的表名请求消息
    /// </summary>
    public class GetTableNamesRequestMessage : ThriftMessage
    {
        #region Constructor.

        /// <summary>
        ///     获取目前HBase内部所有的表名请求消息
        /// </summary>
        public GetTableNamesRequestMessage()
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
    }
}