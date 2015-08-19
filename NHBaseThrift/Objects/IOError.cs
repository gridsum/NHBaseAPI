using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Contracts;
using Gridsum.NHBaseThrift.Enums;

namespace Gridsum.NHBaseThrift.Objects
{
    /// <summary>
    ///     IO错误信息对象，来自于Thrift协议的RSP消息
    /// </summary>
    public class IOError : ThriftObject
    {
        #region Members.

        /// <summary>
        ///     获取或设置错误原因
        /// </summary>
        [ThriftProperty(1, PropertyTypes.String)]
        public string Reason { get; set; }

        #endregion
    }
}