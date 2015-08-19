using System;
using System.Collections.Generic;
using Gridsum.NHBaseThrift.Exceptions;
using Gridsum.NHBaseThrift.Objects;

namespace Gridsum.NHBaseThrift.Client
{
    /// <summary>
    ///    HBase客户端
    /// </summary>
    public interface IHBaseClient
    {
        #region Methods.

        /// <summary>
        ///    创建一个HBase表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columnFamilies">列簇名称信息</param>
        /// <returns>如果创建成功，则返回可以操作该表的实例</returns>
        /// <exception cref="AlreadyExistsException">表已经存在</exception>
        /// <exception cref="IOErrorException">IO错误</exception>
        /// <exception cref="IllegalArgumentException">参数错误</exception>
        /// <exception cref="ArgumentNullException">参数不能为空</exception>
        /// <exception cref="CommunicationTimeoutException">通信超时</exception>
        /// <exception cref="CommunicationFailException">通信失败</exception>
        /// <exception cref="NoConnectionException">内部无任何可用的远程连接异常，这通常代表无法连接到任何一台远程服务器</exception>
        IHTable CreateTable(string tableName, params string[] columnFamilies);
        /// <summary>
        ///    创建一个HBase表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="descriptors">表描述符</param>
        /// <returns>如果创建成功，则返回可以操作该表的实例</returns>
        /// <exception cref="AlreadyExistsException">表已经存在</exception>
        /// <exception cref="IOErrorException">IO错误</exception>
        /// <exception cref="IllegalArgumentException">参数错误</exception>
        /// <exception cref="ArgumentNullException">参数不能为空</exception>
        /// <exception cref="CommunicationTimeoutException">通信超时</exception>
        /// <exception cref="CommunicationFailException">通信失败</exception>
        /// <exception cref="NoConnectionException">内部无任何可用的远程连接异常，这通常代表无法连接到任何一台远程服务器</exception>
        IHTable CreateTable(string tableName, params ColumnDescriptor[] descriptors);
        /// <summary>
        ///    获取一个HBase表的操作接口
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>如果获取成功，则返回可以操作该表的实例</returns>
        IHTable GetTable(string tableName);
        /// <summary>
        ///    删除一个HBase表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <exception cref="ArgumentNullException">参数不能为空</exception>
        /// <exception cref="CommunicationTimeoutException">通信超时</exception>
        /// <exception cref="CommunicationFailException">通信失败</exception>
        void DeleteTable(string tableName);
        /// <summary>
        ///    获取HBase中所有的表名信息
        /// </summary>
        /// <returns>返回所有的表名</returns>
        /// <exception cref="IOErrorException">IO错误</exception>
        /// <exception cref="CommunicationTimeoutException">通信超时</exception>
        /// <exception cref="CommunicationFailException">通信失败</exception>
        List<string> GetTableNames();
		/// <summary>
		///		获取HBase中RegionServer的数量
		/// </summary>
		/// <returns>返回HBase中RegionServer的数量</returns>
	    int GetRegionServerNumber();

	    #endregion
    }
}