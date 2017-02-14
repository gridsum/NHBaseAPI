using System;
using System.Collections.Generic;
using System.Net;
using Gridsum.NHBaseThrift.Comparator;
using Gridsum.NHBaseThrift.Exceptions;
using Gridsum.NHBaseThrift.Objects;
using KJFramework.Tracing;

namespace Gridsum.NHBaseThrift.Client
{
    /// <summary>
    ///    HBase表
    /// </summary>
    internal class HTable : IHTable
    {
        #region Constructor.

        /// <summary>
        ///    HBase表
        /// </summary>
		public HTable(string tableName, HBaseClient client, IHostMappingManager hostMappingManager, IHTableRegionManager regionManager = null)
        {
            _client = client;
            _hostMappingManager = hostMappingManager;
            TableName = tableName;
            if (!client.IsExclusiveMode) EnsureHTableRegions();
            else
            {
                if (regionManager == null) throw new ArgumentException("#Cannot ignore \"regionManager\" field when client is running on the exclusive mode.");
                lock (_lockObj) _regions[TableName] = _regionManager = regionManager;
            }
        }

        #endregion

        #region Members.

		private Random _rnd = new Random();
        private readonly HBaseClient _client;
        private IHTableRegionManager _regionManager;
        private static readonly object _lockObj = new object();
        private readonly IHostMappingManager _hostMappingManager;
	    private readonly IByteArrayComparator _byteArrayComparator;
	    private static readonly ITracing _tracing = TracingManager.GetTracing(typeof (HTable));
        private static readonly IDictionary<string, IHTableRegionManager> _regions = new Dictionary<string, IHTableRegionManager>(); 

        /// <summary>
        ///    获取当前表名
        /// </summary>
        public string TableName { get; private set; }

        #endregion

        #region Methods.

        /// <summary>
        ///    启用该表
        /// </summary>
		/// <exception cref="IOErrorException">IO错误</exception>
		/// <exception cref="ArgumentNullException">参数不能为空</exception>
		/// <exception cref="CommunicationTimeoutException">通信超时</exception>
		/// <exception cref="CommunicationFailException">通信失败</exception>
        public void Enable()
        {
			throw new NotImplementedException();
        }

        /// <summary>
        ///    停用该表
        /// </summary>
        /// <exception cref="IOErrorException">IO错误</exception>
        /// <exception cref="ArgumentNullException">参数不能为空</exception>
        /// <exception cref="CommunicationTimeoutException">通信超时</exception>
        /// <exception cref="CommunicationFailException">通信失败</exception>
        public void Disable()
        {
            _client.Disable(TableName);
        }

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
        public void Insert(byte[] rowKey, params ColumnInfo[] columnInfos)
        {
			if (rowKey == null || rowKey.Length == 0) throw new ArgumentNullException("rowKey");
			if (columnInfos == null || columnInfos.Length == 0) throw new ArgumentNullException("columnInfos");
			Mutation[] mutations = new Mutation[columnInfos.Length];
	        for (int i=0; i < columnInfos.Length; i++)
	        {
				mutations[i] = new Mutation();
		        mutations[i].ColumnName = string.Format("{0}:{1}", columnInfos[i].ColumnFamily, columnInfos[i].ColumnName);
				mutations[i].Value = columnInfos[i].Value;
	        }
            IPEndPoint iep = _regionManager.GetRegionByRowKey(rowKey);
			if (iep == null) throw new RegionNotFoundException(string.Format("#Couldn't found any matched RS by specified row key: {0}", BitConverter.ToString(rowKey)));
            _client.InsertRow(TableName, rowKey, iep, mutations);
        }

		/// <summary>
		///    Get the specified columns
		/// </summary>
		/// <param name="rowKey">rowkey</param>
		/// <exception cref="IOErrorException"></exception>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="CommunicationTimeoutException"></exception>
		/// <exception cref="CommunicationFailException"></exception>
		/// <returns></returns>
        public RowInfo GetRow(byte[] rowKey)
        {
			if (rowKey == null || rowKey.Length == 0) throw new ArgumentNullException("rowKey");
			IPEndPoint iep = _regionManager.GetRegionByRowKey(rowKey);
	        RowInfo[] infos = _client.GetRowFromTable(TableName, rowKey, iep);
	        if (infos == null || infos.Length == 0) return null;
			return infos[0];
        }

	    /// <summary>
		///    Get the specified columns
	    /// </summary>
	    /// <param name="rowKey">rowkey</param>
		/// <param name="columns">columns</param>
	    /// <exception cref="IOErrorException"></exception>
	    /// <exception cref="ArgumentNullException"></exception>
	    /// <exception cref="CommunicationTimeoutException"></exception>
	    /// <exception cref="CommunicationFailException"></exception>
	    /// <returns></returns>
		public RowInfo GetRow(byte[] rowKey, List<string> columns)
		{
			if (rowKey == null || rowKey.Length == 0) throw new ArgumentNullException("rowKey");
		    if (columns == null || columns.Count == 0) return GetRow(rowKey);
			IPEndPoint iep = _regionManager.GetRegionByRowKey(rowKey);
			RowInfo[] infos = _client.GetRowWithColumnsFromTable(TableName, rowKey, columns.ToArray(), iep);
			if (infos == null || infos.Length == 0) return null;
			return infos[0];
		}

        //Ensures how many distributed table regions it has.
        private void EnsureHTableRegions()
        {
            lock (_lockObj)
            {
                if (!_regions.TryGetValue(TableName, out _regionManager))
                {
                    Region[] regions = _client.GetTableRegions(TableName);
                    _regions[TableName] = _regionManager = new HTableRegionManager(regions, _hostMappingManager);
                }
            }
        }

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
	    public bool BatchInsert(out BatchMutation[] exceptionMutations, params BatchMutation[] batchMutation)
	    {
			exceptionMutations = null;
			List<BatchMutation> exceptionMutationList = new List<BatchMutation>();
			if (batchMutation == null || batchMutation.Length == 0) return true;
			Dictionary<IPEndPoint, List<BatchMutation>> dic = new Dictionary<IPEndPoint, List<BatchMutation>>();
			foreach (BatchMutation mutation in batchMutation)
			{
				IPEndPoint iep = _regionManager.GetRegionByRowKey(mutation.RowKey);
				List<BatchMutation> list;
				if (!dic.TryGetValue(iep, out list)) list = new List<BatchMutation>();
				list.Add(mutation);
				dic[iep] = list;
			}
			foreach (KeyValuePair<IPEndPoint, List<BatchMutation>> pair in dic)
			{
				try
				{
					_client.BatchInsert(TableName, pair.Key, pair.Value.ToArray());
				}
				catch(Exception ex)
				{
					exceptionMutationList.AddRange(pair.Value);
					_tracing.Error(string.Format("[{0}]BatchInsert exception : {1}" , pair.Key.Address, ex.Message));
					_tracing.Error(ex, null);
				}
			}
		    if (exceptionMutationList.Count > 0)
		    {
			    exceptionMutations = exceptionMutationList.ToArray();
				return false;
		    }
			return true;
		}

	    /// <summary>
	    ///    获取一段范围的数据行的scanner
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
	    public Scanner NewScanner(byte[] startKey, byte[] endKey, List<string> columns, Dictionary<string, string> attribute = null)
	    {
			if (columns == null) throw new ArgumentNullException("columns");
			if(columns.Count == 0) throw new ArgumentException("columns are undefined");
			IPEndPoint iep = _regionManager.GetRegionByRowKey(startKey);
			int scannerId = _client.GetScannerOpenWithStop(TableName, startKey, endKey, iep, columns.ToArray());
			return new Scanner(scannerId, _client, iep);
	    }

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
	    public Scanner NewScanner(TScan scan, Dictionary<string, string> attribute = null)
	    {
		    byte[] randomKey;
			if (scan == null) throw new ArgumentNullException("scan");
		    if (scan.StartRow != null) randomKey = scan.StartRow;
			else if (scan.StopRow != null) randomKey = scan.StopRow;
			// get a random rowkey to share the hbase pressure
			else
			{
				byte[] bytes = new byte[1];
				_rnd.NextBytes(bytes);
				randomKey = bytes;
			}
			IPEndPoint iep = _regionManager.GetRegionByRowKey(randomKey);
			int scannerId = _client.GetScannerOpenWithScan(TableName, scan, iep);
			return new Scanner(scannerId, _client, iep);
	    }

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
		public long AtomicIncrement(byte[] rowKey, string column, long @value = 1)
		{
			if (rowKey == null || rowKey.Length == 0) throw new ArgumentNullException("rowKey");
			if (string.IsNullOrEmpty(column)) throw new ArgumentNullException("column");
			IPEndPoint iep = _regionManager.GetRegionByRowKey(rowKey);
			return _client.AtomicIncrement(TableName, rowKey, column, iep, @value);
		}

		#endregion

	}
}