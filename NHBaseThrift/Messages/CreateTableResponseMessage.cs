using NHBaseThrift.Attributes;
using NHBaseThrift.Enums;
using NHBaseThrift.Objects;

namespace NHBaseThrift.Messages
{
    /// <summary>
    ///     创建HBase表的应答消息
    /// </summary>
    public class CreateTableResponseMessage : ThriftMessage
    {
        #region Constructor.

        /// <summary>
        ///     创建HBase表的应答消息
        /// </summary>
        public CreateTableResponseMessage()
        {
            uint version = VERSION_1 | (uint)(MessageTypes.Reply);
            Identity = new MessageIdentity
            {
                Command = "createTable",
                Version = (int)version
            };
            Identity.CommandLength = (uint) Identity.Command.Length;
        }

        #endregion

        #region Members.

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
        /// <summary>
        ///     获取或设置资源已经存在错误信息
        /// </summary>
        [ThriftProperty(3, PropertyTypes.String)]
        public AlreadyExistsError AlreadyExistsErrorMessage { get; set; }

        #endregion
    }
}