using NHBaseThrift.Attributes;
using NHBaseThrift.Contracts;
using NHBaseThrift.Enums;

namespace NHBaseThrift.Objects
{
    /// <summary>
    ///    区域信息
    /// </summary>
    public class Region : ThriftObject
    {
        #region Members.

        /// <summary>
        ///    获取或设置行键开始数据
        /// </summary>
        [ThriftProperty(1, PropertyTypes.String)]
        public byte[] StartKey { get; set; }
        /// <summary>
        ///    获取或设置行键结束位置
        /// </summary>
        [ThriftProperty(2, PropertyTypes.String)]
        public byte[] EndKey { get; set; }
        /// <summary>
        ///    获取或设置编号信息
        /// </summary>
        [ThriftProperty(3, PropertyTypes.I64)]
        public long Id { get; set; }
        /// <summary>
        ///    获取或设置名称
        /// </summary>
        [ThriftProperty(4, PropertyTypes.String)]
        public string Name { get; set; }
        /// <summary>
        ///    获取或设置版本信息
        /// </summary>
        [ThriftProperty(5, PropertyTypes.Byte)]
        public byte Version { get; set; }
        /// <summary>
        ///    获取或设置服务器名称
        /// </summary>
        [ThriftProperty(6, PropertyTypes.String)]
        public string ServerName { get; set; }
        /// <summary>
        ///    获取或设置服务器通信端口
        /// </summary>
        [ThriftProperty(7, PropertyTypes.I32)]
        public int Port { get; set; }

        #endregion
    }
}