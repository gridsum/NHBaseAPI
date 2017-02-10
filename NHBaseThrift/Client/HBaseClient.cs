using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Gridsum.NHBaseThrift.Exceptions;
using Gridsum.NHBaseThrift.Messages;
using Gridsum.NHBaseThrift.Network;
using Gridsum.NHBaseThrift.Network.Agents;
using Gridsum.NHBaseThrift.Network.Transactions;
using Gridsum.NHBaseThrift.Objects;
using Gridsum.NHBaseThrift.Proxies;
using KJFramework.EventArgs;
using KJFramework.Tracing;
using ZooKeeperNet;

namespace Gridsum.NHBaseThrift.Client
{
    /// <summary>
    ///     HBase基于Thrift通信协议的客户端实现
    /// </summary>
    public class HBaseClient :  IHBaseClient
    {
        #region Constructor.
        
        /// <summary>
        ///     HBase基于Thrift通信协议的客户端实现
        /// </summary>
        /// <param name="connectionStr">
        ///     连接串信息
        ///     <para>* 此连接串中至少要出现如下key信息:</para>
        ///     <para>* zk=ip:port,ip:port;</para>
        ///     <para>* 可选的key如下:</para>
        ///     <para>* zkTimeout=00:00:30</para>
        ///     <para>* tPort=9090</para>
        ///     <para>* memSegSize=256 (取值范围: 64~1024，并且是64的倍数)</para>
        ///     <para>* memSegMultiples=100000 (当前申请的非托管内存大小 = Multiples*SegmentSize)</para>
        ///     <para>* minConnection=1 (针对相同的远端TCP IP和端口情况下所保持的最小连接数, 默认为1)</para>
        ///     <para>* maxConnection=3 (针对相同的远端TCP IP和端口情况下所保持的最大连接数, 默认为3)</para>
        ///     <para>* allowPrintDumpInfo=false (是否允许在内部出现错误时在日志中打印出网络错误所包含的详细信息)</para>
        /// </param>
        /// <exception cref="ArgumentException">参数错误</exception>
        /// <exception cref="ArgumentNullException">参数不能为空</exception>
        /// <exception cref="Exception">初始化失败</exception>
        /// <exception cref="SocketException">未知的地址信息，通常代表了主机名无法找到指定的对应IP地址</exception>
        /// <exception cref="IPMappingFailException">主机名找不到IP</exception>
        /// <exception cref="ZooKeeperInitializationException">无法正确初始化远程ZooKeeper的状态</exception>
        public HBaseClient(string connectionStr)
        {
            if (string.IsNullOrEmpty(connectionStr)) throw new ArgumentNullException("connectionStr");
            Initialize(connectionStr);
        }

		/// <summary>
		///     HBase基于Thrift通信协议的客户端实现
		/// </summary>
        static HBaseClient()
        {
            _hostMappingManager = new HostMappingManager();
        }

        #endregion

        #region Members.

        private int _tPort;
        private ZooKeeper _zkClient;
        private IList<IPEndPoint> _regionServers;
        private Dictionary<string, string> _arguments;
        private static ThriftProtocolConnectionPool _connectionPool;
        private static readonly HostMappingManager _hostMappingManager;
        private AutoResetEvent _zooKeeperInitLock = new AutoResetEvent(false);
        private static readonly ThriftProtocolStack _protocolStack = new ThriftProtocolStack();
        private static readonly ITracing _tracing = TracingManager.GetTracing(typeof(HBaseClient));
        private static readonly ThriftMessageTransactionManager _transactionManager = new ThriftMessageTransactionManager();

        #endregion

        #region Methods.

        //Try to initializes internal statements.
        private void Initialize(string connectionStr)
        {
            _arguments = connectionStr.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(v => v.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries)).ToDictionary(v => v[0].Trim(), vs => vs[1]);
            string zkStr, zkTimeoutStr;
            if(!_arguments.TryGetValue("zk", out zkStr)) throw new ArgumentException("#Lost KEY argument: \"zk\"");
            TimeSpan zkTimeout;
            if (!_arguments.TryGetValue("zkTimeout", out zkTimeoutStr)) zkTimeout = new TimeSpan(0, 0, 30);
            else zkTimeout = TimeSpan.Parse(zkTimeoutStr);
            string portStr;
            if (!_arguments.TryGetValue("tPort", out portStr)) _tPort = 9090;
            else _tPort = int.Parse(portStr);
            //MEMORY SETTINGS
            string memSegSizeStr, memSegMultiplesStr;
            int memSegSize, memSegMultiples;
            if (!_arguments.TryGetValue("memSegSize", out memSegSizeStr)) memSegSize = 256;
            else memSegSize = int.Parse(memSegSizeStr);
            if (!_arguments.TryGetValue("memSegMultiples", out memSegMultiplesStr)) memSegMultiples = 100000;
            else memSegMultiples = int.Parse(memSegMultiplesStr);
            ThriftProtocolMemoryAllotter.InitializeEnvironment((uint) memSegSize, (uint) memSegMultiples);
            //NETWORK SETTINGS
            int minConnection, maxConnection;
            string minConnectionStr, maxConnectionStr;
            if (!_arguments.TryGetValue("minConnection", out minConnectionStr)) minConnection = 1;
            else minConnection = int.Parse(minConnectionStr);
            if (!_arguments.TryGetValue("maxConnection", out maxConnectionStr)) maxConnection = 3;
            else maxConnection = int.Parse(maxConnectionStr);
            _connectionPool = new ThriftProtocolConnectionPool(minConnection, maxConnection);
            bool allowPrintDumpInfo;
            string allowPrintDumpInfoStr;
            if (!_arguments.TryGetValue("allowPrintDumpInfo", out allowPrintDumpInfoStr)) allowPrintDumpInfo = false;
            else allowPrintDumpInfo = bool.Parse(allowPrintDumpInfoStr);
            Global.AllowedPrintDumpInfo = allowPrintDumpInfo;

            _zkClient = new ZooKeeper(zkStr, zkTimeout, new ZooKeeperWatcher(WaitForZooKeeperInitialization));
            if (!_zooKeeperInitLock.WaitOne(new TimeSpan(0, 0, 30))) throw new ZooKeeperInitializationException();
            _tracing.Info("#Sync remote ZooKeeper state succeed.");
            UpdateRegionServers(null);
        }

        //release control until we have made sure the remote ZooKeeper's state.
        private void WaitForZooKeeperInitialization(WatchedEvent @event)
        {
            if (@event.State == KeeperState.SyncConnected) _zooKeeperInitLock.Set();
        }

        //updates remote region servers IP table.
        private void UpdateRegionServers(WatchedEvent @event)
        {
            //obtains a list of remote region server address. just likes: "gs-server-1003,60020,1433640602093"
            IEnumerable<string> children = _zkClient.GetChildren("/hbase/rs", new ZooKeeperWatcher(UpdateRegionServers));
            List<IPEndPoint> regionServers = new List<IPEndPoint>();
            foreach (string rs in children)
            {
                string[] args = rs.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                regionServers.Add(new IPEndPoint(IPAddress.Parse(_hostMappingManager.GetIPAddressByHostName(args[0])), _tPort));
            }
            Interlocked.Exchange(ref _regionServers, regionServers);
        }
        
        /// <summary>
        ///    创建一个HBase表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columnFamilies">列簇名称信息</param>
        /// <returns>如果创建成功，则返回可以操作该表的实例</returns>
        /// <exception cref="AlreadyExistsException">表已经存在</exception>
        /// <exception cref="IOErrorException">IO错误</exception>
        /// <exception cref="IllegalArgumentException">参数错误</exception>
        /// <exception cref="NoConnectionException">内部无任何可用的远程连接异常，这通常代表无法连接到任何一台远程服务器</exception>
        public IHTable CreateTable(string tableName, params string[] columnFamilies)
        {
            return CreateTable(tableName, columnFamilies.Select(v => new ColumnDescriptor {Name = v}).ToArray());
        }

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
        public IHTable CreateTable(string tableName, params ColumnDescriptor[] descriptors)
        {
            if (string.IsNullOrEmpty(tableName)) throw new ArgumentNullException("tableName");
            if (descriptors == null || descriptors.Length == 0) throw new ArgumentNullException("descriptors");
            IThriftConnectionAgent agent = _connectionPool.GetChannel(_regionServers[0], "RegionServer", _protocolStack, _transactionManager);
            if (agent == null) throw new NoConnectionException();
            Exception ex = null;
            ThriftMessageTransaction transaction = agent.CreateTransaction();
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            transaction.ResponseArrived += delegate(object sender, LightSingleArgEventArgs<ThriftMessage> e)
            {
                CreateTableResponseMessage rspMsg = (CreateTableResponseMessage) e.Target;
                if (rspMsg.IOErrorMessage != null) ex = new IOErrorException(rspMsg.IOErrorMessage.Reason);
                else if (rspMsg.IllegalArgumentErrorMessage != null) ex = new IllegalArgumentException(rspMsg.IllegalArgumentErrorMessage.Reason);
                else if (rspMsg.AlreadyExistsErrorMessage != null) ex = new AlreadyExistsException(rspMsg.AlreadyExistsErrorMessage.Reason);
                autoResetEvent.Set();
            };
            transaction.Timeout += delegate
            {
                ex = new CommunicationTimeoutException(transaction.SequenceId);
                autoResetEvent.Set();
            };
            transaction.Failed += delegate
            {
                ex = new CommunicationFailException(transaction.SequenceId);
                autoResetEvent.Set();
            };
            CreateTableRequestMessage reqMsg = new CreateTableRequestMessage {TableName = tableName, ColumnFamilies = descriptors};
            transaction.SendRequest(reqMsg);
            autoResetEvent.WaitOne();
            if (ex != null) throw ex;
			return new HTable(reqMsg.TableName, this, _hostMappingManager);
        }

        /// <summary>
        ///    获取一个HBase表的操作接口
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>如果获取成功，则返回可以操作该表的实例</returns>
        public IHTable GetTable(string tableName)
        {
			return new HTable(tableName, this, _hostMappingManager);
        }

        /// <summary>
        ///    删除一个HBase表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <exception cref="ArgumentNullException">参数不能为空</exception>
        /// <exception cref="CommunicationTimeoutException">通信超时</exception>
        /// <exception cref="CommunicationFailException">通信失败</exception>
        public void DeleteTable(string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) throw new ArgumentNullException("tableName");
            Disable(tableName);
            IThriftConnectionAgent agent = _connectionPool.GetChannel(_regionServers[0], "RegionServer", _protocolStack, _transactionManager);
            if (agent == null) throw new NoConnectionException();
            Exception ex = null;
            ThriftMessageTransaction transaction = agent.CreateTransaction();
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            transaction.ResponseArrived += delegate(object sender, LightSingleArgEventArgs<ThriftMessage> e)
            {
                DeleteTableResponseMessage rspMsg = (DeleteTableResponseMessage)e.Target;
                if (rspMsg.IOErrorMessage != null) ex = new IOErrorException(rspMsg.IOErrorMessage.Reason);
                autoResetEvent.Set();
            };
            transaction.Timeout += delegate
            {
                ex = new CommunicationTimeoutException(transaction.SequenceId);
                autoResetEvent.Set();
            };
            transaction.Failed += delegate
            {
                ex = new CommunicationFailException(transaction.SequenceId);
                autoResetEvent.Set();
            };
            DeleteTableRequestMessage reqMsg = new DeleteTableRequestMessage { TableName = tableName };
            transaction.SendRequest(reqMsg);
            autoResetEvent.WaitOne();
            if (ex != null) throw ex;
        }

        /// <summary>
        ///    获取HBase中所有的表名信息
        /// </summary>
        /// <returns>返回所有的表名</returns>
        /// <exception cref="IOErrorException">IO错误</exception>
        /// <exception cref="CommunicationTimeoutException">通信超时</exception>
        /// <exception cref="CommunicationFailException">通信失败</exception>
        public List<string> GetTableNames()
        {
            IThriftConnectionAgent agent = _connectionPool.GetChannel(_regionServers[0], "RegionServer", _protocolStack, _transactionManager);
            if (agent == null) throw new NoConnectionException();
            Exception ex = null;
            ThriftMessageTransaction transaction = agent.CreateTransaction();
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            List<string> tables = null;
            transaction.ResponseArrived += delegate(object sender, LightSingleArgEventArgs<ThriftMessage> e)
            {
                GetTableNamesResponseMessage rspMsg = (GetTableNamesResponseMessage)e.Target;
                if (rspMsg.IOErrorMessage != null) ex = new IOErrorException(rspMsg.IOErrorMessage.Reason);
                tables = (rspMsg.Tables == null ? new List<string>() : new List<string>(rspMsg.Tables));
                autoResetEvent.Set();
            };
            transaction.Timeout += delegate
            {
                ex = new CommunicationTimeoutException(transaction.SequenceId);
                autoResetEvent.Set();
            };
            transaction.Failed += delegate
            {
                ex = new CommunicationFailException(transaction.SequenceId);
                autoResetEvent.Set();
            };
            GetTableNamesRequestMessage reqMsg = new GetTableNamesRequestMessage();
            transaction.SendRequest(reqMsg);
            autoResetEvent.WaitOne();
            if (ex != null) throw ex;
            return tables;
        }

        /// <summary>
        ///    禁用HBase中指定表
        ///    <para>* 此方法会自动检测目标表是否处在启用的状态</para>
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <exception cref="IOErrorException">IO错误</exception>
        /// <exception cref="ArgumentNullException">参数不能为空</exception>
        /// <exception cref="CommunicationTimeoutException">通信超时</exception>
        /// <exception cref="CommunicationFailException">通信失败</exception>
        internal void Disable(string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) throw new ArgumentNullException("tableName");
            if (!IsTableEnable(tableName)) return;
            IThriftConnectionAgent agent = _connectionPool.GetChannel(_regionServers[0], "RegionServer", _protocolStack, _transactionManager);
            if (agent == null) throw new NoConnectionException();
            Exception ex = null;
            ThriftMessageTransaction transaction = agent.CreateTransaction();
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            transaction.ResponseArrived += delegate(object sender, LightSingleArgEventArgs<ThriftMessage> e)
            {
                DisableTableResponseMessage rspMsg = (DisableTableResponseMessage)e.Target;
                if (rspMsg.IOErrorMessage != null) ex = new IOErrorException(rspMsg.IOErrorMessage.Reason);
                autoResetEvent.Set();
            };
            transaction.Timeout += delegate
            {
                ex = new CommunicationTimeoutException(transaction.SequenceId);
                autoResetEvent.Set();
            };
            transaction.Failed += delegate
            {
                ex = new CommunicationFailException(transaction.SequenceId);
                autoResetEvent.Set();
            };
            DisableTableRequestMessage reqMsg = new DisableTableRequestMessage{TableName = tableName};
            transaction.SendRequest(reqMsg);
            autoResetEvent.WaitOne();
            if (ex != null) throw ex;
        }

        /// <summary>
        ///    判断一个指定的HBase表是否当前出于启用的状态
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <exception cref="IOErrorException">IO错误</exception>
        /// <exception cref="ArgumentNullException">参数不能为空</exception>
        /// <exception cref="CommunicationTimeoutException">通信超时</exception>
        /// <exception cref="CommunicationFailException">通信失败</exception>
        internal bool IsTableEnable(string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) throw new ArgumentNullException("tableName");
            IThriftConnectionAgent agent = _connectionPool.GetChannel(_regionServers[0], "RegionServer", _protocolStack, _transactionManager);
            if (agent == null) throw new NoConnectionException();
            bool result = false;
            Exception ex = null;
            ThriftMessageTransaction transaction = agent.CreateTransaction();
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            transaction.ResponseArrived += delegate(object sender, LightSingleArgEventArgs<ThriftMessage> e)
            {
                IsTableEnableResponseMessage rspMsg = (IsTableEnableResponseMessage)e.Target;
                if (rspMsg.IOErrorMessage != null) ex = new IOErrorException(rspMsg.IOErrorMessage.Reason);
                result = rspMsg.IsEnable;
                autoResetEvent.Set();
            };
            transaction.Timeout += delegate
            {
                ex = new CommunicationTimeoutException(transaction.SequenceId);
                autoResetEvent.Set();
            };
            transaction.Failed += delegate
            {
                ex = new CommunicationFailException(transaction.SequenceId);
                autoResetEvent.Set();
            };
            IsTableEnableRequestMessage reqMsg = new IsTableEnableRequestMessage { TableName = tableName };
            transaction.SendRequest(reqMsg);
            autoResetEvent.WaitOne();
            if (ex != null) throw ex;
            return result;
        }

        ///  <summary>
        /// 		插入一行数据
        ///  </summary>
        ///  <param name="tableName">表名</param>
        ///  <param name="rowkey">行键</param>
        /// <param name="iep">对应的Region的服务器地址</param>
        /// <param name="mutations">列信息</param>
        ///  <param name="attributes"></param>
        ///  <exception cref="IOErrorException">IO错误</exception>
        ///  <exception cref="IllegalArgumentException">参数错误</exception>
        ///  <exception cref="CommunicationTimeoutException">通信超时</exception>
        ///  <exception cref="CommunicationFailException">通信失败</exception>
        ///  <returns>是否插入成功</returns>
        internal bool InsertRow(string tableName, byte[] rowkey, IPEndPoint iep, Mutation[] mutations, Dictionary<string,string> attributes=null)
		{
			if (string.IsNullOrEmpty(tableName)) throw new ArgumentNullException("tableName");
			if (rowkey == null || rowkey.Length == 0) throw new ArgumentNullException("rowkey");
            if (mutations == null || mutations.Length == 0) throw new ArgumentNullException("mutations");
            IThriftConnectionAgent agent = _connectionPool.GetChannel(iep, "RegionServer", _protocolStack, _transactionManager);
			if (agent == null) throw new NoConnectionException();
            Exception ex = null;
			ThriftMessageTransaction transaction = agent.CreateTransaction();
			AutoResetEvent autoResetEvent = new AutoResetEvent(false);
			transaction.ResponseArrived += delegate(object sender, LightSingleArgEventArgs<ThriftMessage> e)
			{
				InsertNewRowResponseMessage rspMsg = (InsertNewRowResponseMessage)e.Target;
				if (rspMsg.IOErrorMessage != null) ex = new IOErrorException(rspMsg.IOErrorMessage.Reason);
				else if (rspMsg.IllegalArgumentErrorMessage != null) ex = new IllegalArgumentException(rspMsg.IllegalArgumentErrorMessage.Reason);
				autoResetEvent.Set();
			};
			transaction.Timeout += delegate
			{
				ex = new CommunicationTimeoutException(transaction.SequenceId);
				autoResetEvent.Set();
			};
			transaction.Failed += delegate
			{
				ex = new CommunicationFailException(transaction.SequenceId);
				autoResetEvent.Set();
			};
			InsertNewRowRequestMessage reqMsg = new InsertNewRowRequestMessage
			{
				TableName = tableName,
				RowKey = rowkey
			};
            reqMsg.Mutations = mutations;
            reqMsg.Attributes = attributes;
			transaction.SendRequest(reqMsg);
			autoResetEvent.WaitOne();
			if (ex != null) throw ex;
			return true;
		}

	    /// <summary>
	    ///    获取一行数据
	    /// </summary>
	    /// <param name="tableName">表名</param>
		/// <param name="rowKey">键名</param>
		/// <param name="iep">对应的Region的服务器地址</param>
	    /// <exception cref="IOErrorException">IO错误</exception>
	    /// <exception cref="ArgumentNullException">参数不能为空</exception>
	    /// <exception cref="CommunicationTimeoutException">通信超时</exception>
	    /// <exception cref="CommunicationFailException">通信失败</exception>
	    /// <returns>查询结果</returns>
	    internal RowInfo[] GetRowFromTable(string tableName, byte[] rowKey, IPEndPoint iep)
        {
            if (string.IsNullOrEmpty(tableName)) throw new ArgumentNullException("tableName");
			if (rowKey == null || rowKey.Length == 0) throw new ArgumentNullException("rowKey");
			IThriftConnectionAgent agent = _connectionPool.GetChannel(iep, "RegionServer", _protocolStack, _transactionManager);
            if (agent == null) throw new NoConnectionException();
            RowInfo[] result = {};
            Exception ex = null;
            ThriftMessageTransaction transaction = agent.CreateTransaction();
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            transaction.ResponseArrived += delegate(object sender, LightSingleArgEventArgs<ThriftMessage> e)
            {
                GetRowResponseMessage rspMsg = (GetRowResponseMessage)e.Target;
                if (rspMsg.IOErrorMessage != null) ex = new IOErrorException(rspMsg.IOErrorMessage.Reason);
                result = rspMsg.RowInfos;
                autoResetEvent.Set();
            };
            transaction.Timeout += delegate
            {
                ex = new CommunicationTimeoutException(transaction.SequenceId);
                autoResetEvent.Set();
            };
            transaction.Failed += delegate
            {
                ex = new CommunicationFailException(transaction.SequenceId);
                autoResetEvent.Set();
            };
			GetRowRequestMessage reqMsg = new GetRowRequestMessage { TableName = tableName, RowKey = rowKey, Attributes = new Dictionary<string, string>() };
            transaction.SendRequest(reqMsg);
            autoResetEvent.WaitOne();
            if (ex != null) throw ex;
            return result;
        }

	    /// <summary>
	    ///    获取一行数据
	    /// </summary>
	    /// <param name="tableName">表名</param>
	    /// <param name="keyName">键名</param>
	    /// <param name="iep">对应的Region的服务器地址</param>
	    /// <param name="columns">列名</param>
	    /// <exception cref="IOErrorException">IO错误</exception>
	    /// <exception cref="ArgumentNullException">参数不能为空</exception>
	    /// <exception cref="CommunicationTimeoutException">通信超时</exception>
	    /// <exception cref="CommunicationFailException">通信失败</exception>
	    /// <returns>查询结果</returns>
	    internal RowInfo[] GetRowWithColumnsFromTable(string tableName, string keyName, IPEndPoint iep, string[] columns)
		{
			if (string.IsNullOrEmpty(tableName)) throw new ArgumentNullException("tableName");
			if (string.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");
			IThriftConnectionAgent agent = _connectionPool.GetChannel(iep, "RegionServer", _protocolStack, _transactionManager);
			if (agent == null) throw new NoConnectionException();
			RowInfo[] result = { };
			Exception ex = null;
			ThriftMessageTransaction transaction = agent.CreateTransaction();
			AutoResetEvent autoResetEvent = new AutoResetEvent(false);
			transaction.ResponseArrived += delegate(object sender, LightSingleArgEventArgs<ThriftMessage> e)
			{
				GetRowWithColumnsResponseMessage rspMsg = (GetRowWithColumnsResponseMessage)e.Target;
				if (rspMsg.IOErrorMessage != null) ex = new IOErrorException(rspMsg.IOErrorMessage.Reason);
				result = rspMsg.RowInfos;
				autoResetEvent.Set();
			};
			transaction.Timeout += delegate
			{
				ex = new CommunicationTimeoutException(transaction.SequenceId);
				autoResetEvent.Set();
			};
			transaction.Failed += delegate
			{
				ex = new CommunicationFailException(transaction.SequenceId);
				autoResetEvent.Set();
			};
			GetRowWithColumnsRequestMessage reqMsg = new GetRowWithColumnsRequestMessage { TableName = tableName, RowKey = keyName, Columns = columns, Attributes = new Dictionary<string, string>() };
			transaction.SendRequest(reqMsg);
			autoResetEvent.WaitOne();
			if (ex != null) throw ex;
			return result;
		}

        /// <summary>
        ///    从指定的scanner获取数据
        /// </summary>
        /// <param name="id">scanner序列号</param>
        /// <exception cref="IOErrorException">IO错误</exception>
        /// <exception cref="CommunicationTimeoutException">通信超时</exception>
        /// <exception cref="CommunicationFailException">通信失败</exception>
        internal RowInfo[] GetRowFromScanner(int id)
        {
            IThriftConnectionAgent agent = _connectionPool.GetChannel(_regionServers[0], "RegionServer", _protocolStack, _transactionManager);
            if (agent == null) throw new NoConnectionException();
            RowInfo[] result = { };
            Exception ex = null;
            ThriftMessageTransaction transaction = agent.CreateTransaction();
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            transaction.ResponseArrived += delegate(object sender, LightSingleArgEventArgs<ThriftMessage> e)
            {
                ScannerGetResponseMessage rspMsg = (ScannerGetResponseMessage)e.Target;
                if (rspMsg.IOErrorMessage != null) ex = new IOErrorException(rspMsg.IOErrorMessage.Reason);
                result = rspMsg.RowInfos;
                autoResetEvent.Set();
            };
            transaction.Timeout += delegate
            {
                ex = new CommunicationTimeoutException(transaction.SequenceId);
                autoResetEvent.Set();
            };
            transaction.Failed += delegate
            {
                ex = new CommunicationFailException(transaction.SequenceId);
                autoResetEvent.Set();
            };
            ScannerGetRequestMessage reqMsg = new ScannerGetRequestMessage { Id = id };
            transaction.SendRequest(reqMsg);
            autoResetEvent.WaitOne();
            if (ex != null) throw ex;
            return result;
        }

        /// <summary>
        ///    从指定的scanner获取数据
        /// </summary>
        /// <param name="id">scanner序列号</param>
        /// <param name="rowCount">查询行数量</param>
		/// <param name="iep">对应的Region的服务器地址</param>
        /// <exception cref="IOErrorException">IO错误</exception>
        /// <exception cref="CommunicationTimeoutException">通信超时</exception>
        /// <exception cref="CommunicationFailException">通信失败</exception>
        internal RowInfo[] GetRowsFromScanner(int id, int rowCount, IPEndPoint iep)
        {
			IThriftConnectionAgent agent = _connectionPool.GetChannel(iep, "RegionServer", _protocolStack, _transactionManager);
            if (agent == null) throw new NoConnectionException();
            RowInfo[] result = { };
            Exception ex = null;
            ThriftMessageTransaction transaction = agent.CreateTransaction();
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            transaction.ResponseArrived += delegate(object sender, LightSingleArgEventArgs<ThriftMessage> e)
            {
                ScannerGetListResponseMessage rspMsg = (ScannerGetListResponseMessage)e.Target;
                if (rspMsg.IOErrorMessage != null) ex = new IOErrorException(rspMsg.IOErrorMessage.Reason);
                result = rspMsg.RowInfos;
                autoResetEvent.Set();
            };
            transaction.Timeout += delegate
            {
                ex = new CommunicationTimeoutException(transaction.SequenceId);
                autoResetEvent.Set();
            };
            transaction.Failed += delegate
            {
                ex = new CommunicationFailException(transaction.SequenceId);
                autoResetEvent.Set();
            };
            ScannerGetListRequestMessage reqMsg = new ScannerGetListRequestMessage { Id = id, RowsCount = rowCount };
            transaction.SendRequest(reqMsg);
            autoResetEvent.WaitOne();
            if (ex != null) throw ex;
            return result;
        }

		/// <summary>
		///		插入多行数据
		/// </summary>
		/// <param name="tableName">表名</param>
		/// <param name="iep">对应的Region的服务器地址</param>
		/// <param name="batchMutation">列数据集合</param>
		/// <exception cref="IOErrorException">IO错误</exception>
		/// <exception cref="ArgumentNullException">参数不能为空</exception>
		/// <exception cref="CommunicationTimeoutException">通信超时</exception>
		/// <exception cref="CommunicationFailException">通信失败</exception>
		/// <returns>是否成功插入</returns>
		internal bool BatchInsert(string tableName, IPEndPoint iep, BatchMutation[] batchMutation)
	    {
			if (batchMutation == null) throw new ArgumentNullException("batchMutation");
			IThriftConnectionAgent agent = _connectionPool.GetChannel(iep, "RegionServer", _protocolStack, _transactionManager);
			if (agent == null) throw new NoConnectionException();
			Exception ex = null;
			ThriftMessageTransaction transaction = agent.CreateTransaction();
			AutoResetEvent autoResetEvent = new AutoResetEvent(false);
			transaction.ResponseArrived += delegate(object sender, LightSingleArgEventArgs<ThriftMessage> e)
			{
				InsertNewRowsResponseMessage rspMsg = (InsertNewRowsResponseMessage)e.Target;
				if (rspMsg.IOErrorMessage != null) ex = new IOErrorException(rspMsg.IOErrorMessage.Reason);
				autoResetEvent.Set();
			};
			transaction.Timeout += delegate
			{
				ex = new CommunicationTimeoutException(transaction.SequenceId);
				autoResetEvent.Set();
			};
			transaction.Failed += delegate
			{
				ex = new CommunicationFailException(transaction.SequenceId);
				autoResetEvent.Set();
			};
			InsertNewRowsRequestMessage reqMsg = new InsertNewRowsRequestMessage { TableName = tableName, RowBatch = batchMutation, Attributes = new Dictionary<string, string>() };
			transaction.SendRequest(reqMsg);
			autoResetEvent.WaitOne();
			if (ex != null) throw ex;
			return true;
	    }

        /// <summary>
        ///    获取一个HBase表在整体集群中的位置分布
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <exception cref="IOErrorException">IO错误</exception>
        /// <exception cref="ArgumentNullException">参数不能为空</exception>
        /// <exception cref="CommunicationTimeoutException">通信超时</exception>
        /// <exception cref="CommunicationFailException">通信失败</exception>
        internal Region[] GetTableRegions(string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) throw new ArgumentNullException("tableName");
            IThriftConnectionAgent agent = _connectionPool.GetChannel(_regionServers[0], "RegionServer", _protocolStack, _transactionManager);
            if (agent == null) throw new NoConnectionException();
            Exception ex = null;
            Region[] result = null;
            ThriftMessageTransaction transaction = agent.CreateTransaction();
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            transaction.ResponseArrived += delegate(object sender, LightSingleArgEventArgs<ThriftMessage> e)
            {
                GetTableRegionsResponseMessage rspMsg = (GetTableRegionsResponseMessage)e.Target;
                if (rspMsg.IOErrorMessage != null) ex = new IOErrorException(rspMsg.IOErrorMessage.Reason);
                result = rspMsg.Regions;
                autoResetEvent.Set();
            };
            transaction.Timeout += delegate
            {
                ex = new CommunicationTimeoutException(transaction.SequenceId);
                autoResetEvent.Set();
            };
            transaction.Failed += delegate
            {
                ex = new CommunicationFailException(transaction.SequenceId);
                autoResetEvent.Set();
            };
            GetTableRegionsRequestMessage reqMsg = new GetTableRegionsRequestMessage { TableName = tableName };
            transaction.SendRequest(reqMsg);
            autoResetEvent.WaitOne();
            if (ex != null) throw ex;
            return result;
        }

	    ///  <summary>
	    /// 		按rowkey上下限获取scannerId
	    ///  </summary>
	    ///  <param name="tableName">表名</param>
	    ///  <param name="startKey">rowkey范围上界(结果中包含上界数据)</param>
	    ///  <param name="endKey">rowkey范围下界(结果中不包含下界数据)</param>
	    ///  <param name="iep">对应的Region的服务器地址</param>
	    ///  <param name="columns">指定获取的列名</param>
		///  <param name="attribute">attribute</param>
	    ///  <exception cref="IOErrorException">IO错误</exception>
	    ///  <exception cref="ArgumentNullException">参数不能为空</exception>
	    ///  <exception cref="CommunicationTimeoutException">通信超时</exception>
	    ///  <exception cref="CommunicationFailException">通信失败</exception>
	    ///  <returns>scannerId</returns>
	    internal int GetScannerOpenWithStop(string tableName, byte[] startKey, byte[] endKey, IPEndPoint iep, string[] columns, Dictionary<string, string> attribute = null)
		{
			if (string.IsNullOrEmpty(tableName)) throw new ArgumentNullException("tableName");
			if (startKey == null || startKey.Length == 0) throw new ArgumentNullException("startKey");
			if (endKey == null || endKey.Length == 0) throw new ArgumentNullException("endKey");
			IThriftConnectionAgent agent = _connectionPool.GetChannel(iep, "RegionServer", _protocolStack, _transactionManager);
			if (agent == null) throw new NoConnectionException();
			Exception ex = null;
			int result = -1;
			ThriftMessageTransaction transaction = agent.CreateTransaction();
			AutoResetEvent autoResetEvent = new AutoResetEvent(false);
			transaction.ResponseArrived += delegate(object sender, LightSingleArgEventArgs<ThriftMessage> e)
			{
				ScannerOpenWithStopResponseMessage rspMsg = (ScannerOpenWithStopResponseMessage)e.Target;
				if (rspMsg.IOErrorMessage != null) ex = new IOErrorException(rspMsg.IOErrorMessage.Reason);
				result = rspMsg.ScannerId;
				autoResetEvent.Set();
			};
			transaction.Timeout += delegate
			{
				ex = new CommunicationTimeoutException(transaction.SequenceId);
				autoResetEvent.Set();
			};
			transaction.Failed += delegate
			{
				ex = new CommunicationFailException(transaction.SequenceId);
				autoResetEvent.Set();
			};
			ScannerOpenWithStopRequestMessage reqMsg = new ScannerOpenWithStopRequestMessage { TableName = tableName, StartRow = startKey, EndRow = endKey, Columns = columns, Attribute = attribute};
			transaction.SendRequest(reqMsg);
			autoResetEvent.WaitOne();
			if (ex != null) throw ex;
			return result;
		}


	    ///  <summary>
		/// 		自定义TScan获取scannerId。Tscan document：http://hbase.apache.org/book.html#thrift
	    ///  </summary>
	    ///  <param name="tableName">表名</param>
		///  <param name="scan">A Scan object is used to specify scanner parameters</param>
	    ///  <param name="iep">对应的Region的服务器地址</param>
	    ///  <param name="attribute">attribute</param>
	    ///  <exception cref="IOErrorException">IO错误</exception>
	    ///  <exception cref="ArgumentNullException">参数不能为空</exception>
	    ///  <exception cref="CommunicationTimeoutException">通信超时</exception>
	    ///  <exception cref="CommunicationFailException">通信失败</exception>
	    ///  <returns>scannerId</returns>
	    internal int GetScannerOpenWithScan(string tableName, TScan scan, IPEndPoint iep, Dictionary<string, string> attribute = null)
		{
			if (string.IsNullOrEmpty(tableName)) throw new ArgumentNullException("tableName");
			if (scan == null) throw new ArgumentNullException("TScan");
			IThriftConnectionAgent agent = _connectionPool.GetChannel(iep, "RegionServer", _protocolStack, _transactionManager);
			if (agent == null) throw new NoConnectionException();
			Exception ex = null;
			int result = -1;
			ThriftMessageTransaction transaction = agent.CreateTransaction();
			AutoResetEvent autoResetEvent = new AutoResetEvent(false);
			transaction.ResponseArrived += delegate(object sender, LightSingleArgEventArgs<ThriftMessage> e)
			{
				ScannerOpenWithScanResponseMessage rspMsg = (ScannerOpenWithScanResponseMessage)e.Target;
				if (rspMsg.IOErrorMessage != null) ex = new IOErrorException(rspMsg.IOErrorMessage.Reason);
				result = rspMsg.ScannerId;
				autoResetEvent.Set();
			};
			transaction.Timeout += delegate
			{
				ex = new CommunicationTimeoutException(transaction.SequenceId);
				autoResetEvent.Set();
			};
			transaction.Failed += delegate
			{
				ex = new CommunicationFailException(transaction.SequenceId);
				autoResetEvent.Set();
			};
			ScannerOpenWithScanRequestMessage reqMsg = new ScannerOpenWithScanRequestMessage { TableName = tableName, Scan = scan, Attribute = attribute };
			transaction.SendRequest(reqMsg);
			autoResetEvent.WaitOne();
			if (ex != null) throw ex;
			return result;
		}

	    /// <summary>
	    ///		回收指定scanner资源
	    /// </summary>
		/// <param name="scannerId">scannerId</param>
		/// <param name="iep">对应的Region的服务器地址</param>
	    /// <exception cref="IOErrorException">IO错误</exception>
	    /// <exception cref="ArgumentNullException">参数不能为空</exception>
	    /// <exception cref="CommunicationTimeoutException">通信超时</exception>
	    /// <exception cref="CommunicationFailException">通信失败</exception>
	    internal void ScannerClose(int scannerId, IPEndPoint iep)
	    {
			IThriftConnectionAgent agent = _connectionPool.GetChannel(iep, "RegionServer", _protocolStack, _transactionManager); 
			if (agent == null) throw new NoConnectionException();
			Exception ex = null;
		    ThriftMessageTransaction transaction = agent.CreateTransaction();
			AutoResetEvent autoResetEvent = new AutoResetEvent(false);
			transaction.ResponseArrived += delegate(object sender, LightSingleArgEventArgs<ThriftMessage> e)
			{
				ScannerCloseResponseMessage rspMsg = (ScannerCloseResponseMessage)e.Target;
				if (rspMsg.IOErrorMessage != null) ex = new IOErrorException(rspMsg.IOErrorMessage.Reason);
				if (rspMsg.IOErrorMessage != null) ex = new IOErrorException(rspMsg.IOErrorMessage.Reason);
				else if (rspMsg.IllegalArgumentErrorMessage != null) ex = new IllegalArgumentException(rspMsg.IllegalArgumentErrorMessage.Reason);
				autoResetEvent.Set();
			};
			transaction.Timeout += delegate
			{
				ex = new CommunicationTimeoutException(transaction.SequenceId);
				autoResetEvent.Set();
			};
			transaction.Failed += delegate
			{
				ex = new CommunicationFailException(transaction.SequenceId);
				autoResetEvent.Set();
			};
			ScannerCloseRequestMessage reqMsg = new ScannerCloseRequestMessage { ScannerId = scannerId};
			transaction.SendRequest(reqMsg);
			autoResetEvent.WaitOne();
			if (ex != null) throw ex;
	    }

	    ///  <summary>
	    /// 	原子计数器递增
	    ///  </summary>
	    ///  <param name="tableName">表名</param>
	    ///  <param name="rowKey">rowKey</param>
	    ///  <param name="column">列名</param>
		/// <param name="iep">对应的Region的服务器地址</param>
	    /// <param name="value">递增值</param>
	    ///  <returns>递增后的结果</returns>
	    internal long AtomicIncrement(string tableName, byte[] rowKey, string column, IPEndPoint iep, long @value = 1)
	    {
			if (string.IsNullOrEmpty(tableName)) throw new ArgumentNullException("tableName");
			IThriftConnectionAgent agent = _connectionPool.GetChannel(iep, "RegionServer", _protocolStack, _transactionManager);
			if (agent == null) throw new NoConnectionException();
			Exception ex = null;
			long result = 0;
			ThriftMessageTransaction transaction = agent.CreateTransaction();
			AutoResetEvent autoResetEvent = new AutoResetEvent(false);
			transaction.ResponseArrived += delegate(object sender, LightSingleArgEventArgs<ThriftMessage> e)
			{
				AtomicIncrementResponseMessage rspMsg = (AtomicIncrementResponseMessage)e.Target;
				if (rspMsg.IOErrorMessage != null) ex = new IOErrorException(rspMsg.IOErrorMessage.Reason);
				else if (rspMsg.IllegalArgumentErrorMessage != null) ex = new IOErrorException(rspMsg.IllegalArgumentErrorMessage.Reason);
				result = rspMsg.Success;
				autoResetEvent.Set();
			};
			transaction.Timeout += delegate
			{
				ex = new CommunicationTimeoutException(transaction.SequenceId);
				autoResetEvent.Set();
			};
			transaction.Failed += delegate
			{
				ex = new CommunicationFailException(transaction.SequenceId);
				autoResetEvent.Set();
			};
			AtomicIncrementRequestMessage reqMsg = new AtomicIncrementRequestMessage { TableName = tableName, RowKey = rowKey, Column = column, Value = @value};
			transaction.SendRequest(reqMsg);
			autoResetEvent.WaitOne();
			if (ex != null) throw ex;
			return result;
	    }

		/// <summary>
		///		获取HBase中RegionServer的数量
		/// </summary>
		/// <returns>返回HBase中RegionServer的数量</returns>
	    public int GetRegionServerNumber()
	    {
		    return _regionServers.Count;
	    }

	    #endregion
    }
}