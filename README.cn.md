# NHBaseAPI 介绍

标签（空格分隔）： API Document HBase Thrift

---
###**前言**###

NHBaseAPI是一款完全基于.NET平台所开发的HBase API。它内部使用的是Thrift协议接口与HBase进行通信。作为一款完全基于.NET平台所开发的API，我们内部刻意保证了其跨平台的特性。也就是说使用者不需要任何改动NHaseAPI的代码就可以被运行在Linux系统中的Mono平台下。

###**简介**###
由于此API是基于Thrift解析与远程HBase服务器进行通信的，我们基于的Thrift协议版本为: 0.9.2。在NHaseAPI的内部我们完全重写了Thrift协议。也就是说我们在遵循Thrift的二进制协议格式大前提下完全自己写了一套自动化的Thrift协议高性能实现。自己重做一套的原因大体上有2个：

#### - 性能问题

> 原生的Thrift协议实现主要分为2大部分(网络层以及序列化/反序列化) 在网络层实现上，原生的Thrift协议栈并没有实现将一个完整的消息包一次性发送到远程的常规场景。这样的实现机制会大量耗费服务器端的性能资源。第二个问题就是序列化问题，原生的序列化方式会产生极大的内存耗损，这主要体现在会产生大量的托管堆内存碎片上。

#### - BUG

> 原生的实现中在网络层是有BUG的。这个BUG是每次发送消息时，从协议栈发出的消息SEQID都为0。这样直接造成了应用程序在多线程的场景下无法正常使用的情况，除非我们手动控制多线程同步，但是这样的话会极大的降低应用程序的输出。

在接口的设计上，我们进行了详细的规划以及思考，在满足日常使用的前提下极大的减少了使用者在使用此API时需要了解的前置知识。NHaseAPI的易用性不光光体现在API的接口设计上，更体现在了内部的运行机制。比如自动计算RegionServer的分布、自动断线重连、自动多TCP SOCKET进行请求负载等等。


###**NHBaseAPI的优势**###

- 自动从远程ZooKeeper来获取HBase的资源信息
- 在进行HBase表操作时会先自动拿到表的Region分布信息，并且在进行操作时会自动根据Region的信息对操作进行不同RegionServer的请求分发。
- 能够自动更新HBase表的Region信息
- 高性能
	- 非常低.NET GC Time
	- 非常少的GC内存碎片
	- 基于MSIL级别的.NET反射功能实现, 完全无装箱/拆箱操作
	- 针对部分热内存的使用上，采用了非托管内存申请以及托管指针操作的方式
	- 网络层采用底层SOCKET外加WINDOWS IOCP高性能网络模型进行实现，确保网络通信成本的最小化
    - 在网络层上，会为每个RegionServer保持多个TCP连接以均衡对于这个RegionServer的相同TCP压力
- 非常易用，仅需要一个ConnectionStr就可以初始化NHBase API
- 在操作接口上提供批量写入接口，并支持根据RegionServer的数量基于Region的分布自动分批次发送
- 全局恰当的错误处理
	- 优美的日志文件
		- 完全的网络通信日志
		- 完全的API操作日志
	- 接口级别支持请求错误提示功能，提供给使用者请求错误的数据以供后续重试
- ……

在当前版本的接口设计中我们只提供了对于HBase操作的基础功能，并且仅提供了操作HBase的同步方法，在后续的版本中我们将会提供异步的操作方法。目前提供的基础操作有：

###**NHBaseAPI支持的功能**###
#### - 表级别操作
- 创建表
- 删除表
- 启用表
- 禁用表
- 判断指定表是否在启用状态
- 列出所有表名称

#### - 数据操作
- 插入单条数据
- 批量数据插入
- 根据Rowkey读取一行数据
- 根据RowKey范围读取多行数据
- 原子性累加

#### - 集群
- 在创建HBase表实例时(HTable)，会从ZooKeeper中自动获取一个HBase表的RowKey分布
- 支持在HTable的生命周期中自动更新/维护该表的RowKey分布

###**NHBaseAPI的使用方式**###
#### - 创建一个NHBaseAPI的客户端实例

你可能会惊奇地发现，创建NHBaseAPI客户端仅仅需要传入一个连接字符串即可。

```csharp
string connectionStr = "zk=xxxxxx:2181,xxxxxx:2181,xxxxxx:2181";
IHBaseClient client = new HBaseClient(connectionStr)
```

一个连接字符串中包含了若干个设置信息。一个非常简单的初始化方法是仅需要在连接字符串中包含zk信息即可。

连接字符串中的设置项:

| 设置项 | 默认值 | 描述 | 是否可选 |
| -----  | -----  | ---- | -----    |
|zkTimeout|00:00:30|远程ZooKeeper的Session过期时间|Y|
|memSegSize|256|接收网络数据时的单个内存片段大小 单位: byte (取值范围: 64~1024，并且是64的倍数)|Y|
|memSegMultiples|100000|当前申请的非托管内存大小 = Multiples*SegmentSize|Y|
|minConnection|1|针对相同的远端TCP IP和端口情况下所保持的最小连接数|Y|
|maxConnection|3|针对相同的远端TCP IP和端口情况下所保持的最大连接数|Y|
|allowPrintDumpInfo|false|是否允许在内部出现错误时在日志中打印出网络错误所包含的详细信息|Y|
|exclusiveMode|false|RS 资源专有模式，如果开启的话，系统内部将会遵从 rsServers 来初始化远程服务器列表|Y|
|rsServers|null|RS IP 列表，格式为：`rsServers=ip:port,ip:port`|Y|

#### - 创建表操作
```csharp
//省略初始化步骤
IHBaseClient client = new HBaseClient(connectionStr)
IHTable table = client.CreateTable("TableName", "cf");
```

#### - 删除表操作
```csharp
//省略初始化步骤
IHBaseClient client = new HBaseClient(connectionStr)
client.DeleteTable("TableName");
```

#### - 插入单条数据操作
```csharp
//省略初始化步骤
IHBaseClient client = new HBaseClient(connectionStr)
IHTable table = client.GetTable("TableName");
string tableName = string.Format("gridsum_test_thrift_table_test_{0}", DateTime.Now.Millisecond);
ColumnInfo[] columninfos = new[]
{
	new ColumnInfo
	{
		ColumnFamily = "cf",
		ColumnName = "col1",
		Value = Encoding.UTF8.GetBytes("world1")
	}
};
byte[] rowKey = ...;
table.Insert(rowKey, columninfos);
```


#### - 根据RowKey读取单挑数据操作

```csharp
//省略初始化步骤
IHBaseClient client = new HBaseClient(connectionStr)
IHTable table = client.GetTable("TableName");
byte[] rowKey = ...;
RowInfo info = table.GetRow(rowKey);
```


#### - 批量插入数据操作

```csharp
//省略初始化步骤
IHBaseClient client = new HBaseClient(connectionStr)
IHTable table = client.GetTable("TableName");
byte[] rowKey = ...;
BatchMutation[] rows = new[]
{
	new BatchMutation{
		RowKey = Encoding.UTF8.GetBytes("rowkey1"), 
		Mutations = new[]
		{
			new Mutation{ColumnName = "cf:col1", Value = Encoding.UTF8.GetBytes("value1")}
		}},
	new BatchMutation{
		RowKey = Encoding.UTF8.GetBytes("rowkey2"), 
		Mutations = new[]
		{
			new Mutation{ColumnName = "cf:col2", Value = Encoding.UTF8.GetBytes("value2")}
		}}
};
BatchMutation[] exceptionBatchMutations;
//批量插入数据，并且通过out回来的数据接收插入失败的数据集合
//可以通过方法的整体返回值判断是否所有本批次的数据都插入成功了
bool result = table.BatchInsert(out exceptionBatchMutations, rows);
```

#### - 根据RowKey范围读取数据

```csharp
//省略初始化步骤
IHBaseClient client = new HBaseClient(connectionStr)
IHTable table = client.GetTable("TableName");
byte[] startRowKey = ...;
byte[] endRowKey = ...;
//测试代码中指定了一个列簇
List<string> columnFamily = new List<string> {"cf"};

// 读取一个列簇中所有列的数据
Scanner scanner = table.NewScanner(startRowKey, endRowKey, columnFamily);
RowInfo info;
while ((info = scanner.GetNext()) != null)
{
	foreach (KeyValuePair<string, Cell> pair in info.Columns)
	{
	    //pair中包含了读取到的数据
	}
}

// 读取一个列簇中指定列的数据
columnFamily = new List<string> {"cf:column1"};
scanner = table.NewScanner(startRowKey, endRowKey, columnFamily);
while ((info = scanner.GetNext()) != null)
{
	foreach (KeyValuePair<string, Cell> pair in info.Columns)
	{
	    //pair中包含了读取到的数据
	}
}
```

#### - 通过Filter过滤数据
```csharp
TScan scan = new TScan()
{
	StartRow = new byte[] {0},
	StopRow = new byte[] {0xff},
	FilterString = "SingleColumnValueFilter('cf','column1',=,'regexstring:value[23]4')",
	//FilterString = "SingleColumnValueFilter('cf','column1',=,'substring:value2')",
	Columns = new[] {"cf:column1"}
};
Scanner scanner = table.NewScanner(scan);
try
{
	while ((info = scanner.GetNext()) != null)
	{
		tmpDictionary[info.RowKey] = new Dictionary<string, string>();
		foreach (KeyValuePair<string, Cell> pair in info.Columns)
		{
			 //pair中包含了读取到的数据
		}
	}
}
catch(Exception ex)
{
	scanner.Dispose();
}
```

###**期望**###

我们很期待每一位使用者都能向我们提出宝贵的意见。我们将会在日后的时间里持续改进这个API，并且持续为.NET开源社区做出贡献。

