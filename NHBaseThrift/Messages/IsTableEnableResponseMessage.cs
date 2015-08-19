using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Objects;

namespace Gridsum.NHBaseThrift.Messages
{
    /// <summary>
    ///     获取一个表是否已经启用的应答消息
    /// </summary>
    public class IsTableEnableResponseMessage : ThriftMessage
    {
        #region Constructor.

        /// <summary>
        ///     获取一个表所分布在HBase的区域范围应答消息
        /// </summary>
        public IsTableEnableResponseMessage()
        {
            uint version = VERSION_1 | (uint)(MessageTypes.Call);
            Identity = new MessageIdentity
            {
                Command = "isTableEnabled",
                Version = (int)version
            };
            Identity.CommandLength = (uint) Identity.Command.Length;
        }

        #endregion

        #region Members.

        /// <summary>
        ///     获取或设置表名
        /// </summary>
        [ThriftProperty(0, PropertyTypes.Bool)]
        public bool IsEnable { get; set; }
        /// <summary>
        ///     获取或设置IO错误信息
        /// </summary>
        [ThriftProperty(1, PropertyTypes.String)]
        public IOError IOErrorMessage { get; set; }

        #endregion
    }
}