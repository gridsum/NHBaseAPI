namespace Gridsum.NHBaseThrift.Enums
{
    /// <summary>
    ///     解析Thrift协议对象的结果类型
    /// </summary>
    public enum GetObjectResultTypes : byte
    {
        /// <summary>
        ///    当前填充的原始数据还不足以解析出一个完整的Thrift协议对象
        /// </summary>
        NotEnoughData = 0x00,
        /// <summary>
        ///    解析成功
        /// </summary>
        Succeed = 0x01,
        /// <summary>
        ///     解析失败, 错误的数据格式
        /// </summary>
        BadFormat = 0x02,
        /// <summary>
        ///     解析失败, 位置的协议对象类型
        /// </summary>
        UnknownObjectType = 0x03
        
    }
}