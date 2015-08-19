using System;
using System.Net;

namespace Gridsum.NHBaseThrift
{
    /// <summary>
    ///     HBase表区域分布管理器接口
    /// </summary>
    public interface IHTableRegionManager
    {
        #region Methods.

        /// <summary>
        ///     根据一个指定的行键来确定远程Region服务器地址
        /// </summary>
        /// <param name="rowKey">行键</param>
        /// <returns>返回对应的Region服务器地址</returns>
        /// <exception cref="ArgumentNullException">参数不能为空</exception>
        IPEndPoint GetRegionByRowKey(byte[] rowKey);

        #endregion
    }
}