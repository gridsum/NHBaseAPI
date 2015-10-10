using System;
using System.Collections.Generic;
using Gridsum.NHBaseThrift.Exceptions;
using Gridsum.NHBaseThrift.Objects;

namespace Gridsum.NHBaseThrift.Client
{
    /// <summary>
    ///    HBase表操作接口
    /// </summary>
    public interface IHTable
    {
        #region Members.

        /// <summary>
        ///    获取当前表名
        /// </summary>
        string TableName { get; }

        #endregion

        #region Methods.

		/// <summary>
		///    启用该表
		/// </summary>
		/// <exception cref="IOErrorException">IO错误</exception>
		/// <exception cref="ArgumentNullException">参数不能为空</exception>
		/// <exception cref="CommunicationTimeoutException">通信超时</exception>
		/// <exception cref="CommunicationFailException">通信失败</exception>
        void Enable();
		/// <summary>
		///    停用该表
		/// </summary>
		/// <exception cref="IOErrorException">IO错误</exception>
		/// <exception cref="ArgumentNullException">参数不能为空</exception>
		/// <exception cref="CommunicationTimeoutException">通信超时</exception>
        void Disable();
		/// <summary>
		///    插入一行新的记录
		/// </summary>
		/// <param name="rowKey">行键信息</param>
		/// <param name="columnInfos">插入的列信息</param>
		/// <exception cref="IOErrorException">IO错误</exception>
		/// <exception cref="ArgumentNullException">参数不能为空</exception>
		/// <exception cref="CommunicationTimeoutException">通信超时</exception>
		/// <exception cref="CommunicationFailException">通信失败</exception>
		/// <exception cref="RegionNotFoundException">找不到对应的RegionServer</exception>
		void Insert(byte[] rowKey, params ColumnInfo[] columnInfos);
		/// <summary>
		///    批量插入行操作
		/// </summary>
		/// <param name="exceptionMutations">插入异常的数据集合</param>
		/// <param name="batchMutation">行集合信息</param>
		/// <exception cref="IOErrorException">IO错误</exception>
		/// <exception cref="ArgumentNullException">参数不能为空</exception>
		/// <exception cref="CommunicationTimeoutException">通信超时</exception>
		/// <exception cref="CommunicationFailException">通信失败</exception>
		/// <returns>全部成功插入返回true，其它返回false</returns>
	    bool BatchInsert(out BatchMutation[] exceptionMutations, params BatchMutation[] batchMutation);
		/// <summary>
		///    获取一个具有指定键值的数据行
		/// </summary>
		/// <param name="rowKey">行键</param>
		/// <exception cref="IOErrorException">IO错误</exception>
		/// <exception cref="ArgumentNullException">参数不能为空</exception>
		/// <exception cref="CommunicationTimeoutException">通信超时</exception>
		/// <exception cref="CommunicationFailException">通信失败</exception>
		/// <returns>返回符合指定条件的行数据。若未查找到指定数据则返回null</returns>
        RowInfo GetRow(byte[] rowKey);
		/// <summary>
		///	   原子计数器递增
		/// </summary>
		/// <param name="rowKey">rowKey</param>
		/// <param name="column">列名</param>
		/// <param name="value">递增值</param>
		/// <exception cref="ArgumentNullException">列名参数不能为空</exception>
		/// <exception cref="IOErrorException">IO错误</exception>
		/// <exception cref="CommunicationTimeoutException">通信超时</exception>
		/// <exception cref="CommunicationFailException">通信失败</exception>
		/// <returns>递增后的结果</returns>
	    long AtomicIncrement(byte[] rowKey, string column, long value = 1);
		/// <summary>
		///    获取一段范围的数据行
		/// </summary>
		/// <param name="startKey">查询起始key值</param>
		/// <param name="endKey">查询终止key值</param>
		/// <param name="columns">指定列名。当值为列族名时返回列族所有所有列数据</param>
		/// <param name="attribute">attribute</param>
		/// <exception cref="IOErrorException">IO错误</exception>
		/// <exception cref="ArgumentNullException">参数不能为空</exception>
		/// <exception cref="CommunicationTimeoutException">通信超时</exception>
		/// <exception cref="CommunicationFailException">通信失败</exception>
		/// <returns>Scanner对象</returns>
	    Scanner NewScanner(byte[] startKey, byte[] endKey, List<string> columns , Dictionary<string, string> attribute = null);
	    /// <summary>
	    ///    获取一段范围的数据行的scanner
	    /// </summary>
	    /// <param name="scan">A Scan object is used to specify scanner parameters</param>
	    /// <param name="attribute">attribute</param>
	    /// <exception cref="IOErrorException">IO错误</exception>
	    /// <exception cref="ArgumentNullException">参数不能为空</exception>
	    /// <exception cref="CommunicationTimeoutException">通信超时</exception>
	    /// <exception cref="CommunicationFailException">通信失败</exception>
	    /// <returns>Scanner对象</returns>
	    Scanner NewScanner(TScan scan, Dictionary<string, string> attribute = null);

	    #endregion
    }
}