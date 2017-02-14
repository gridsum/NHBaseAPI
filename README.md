# NHBaseAPI
-----

##Preface##
NHBaseAPI is an HBase API based completely on .NET platform. Internally it uses the interface under the Thrift protocol to communicate with HBase. As an API based completely on .NET platform, we deliberately preserve the multi-platform feature within it, which means developers can use the API on Mono platform in Linux without the need to change any NHBaseAPI codes.


##Intro##
Since this API is relying on the Thrift to parse the communication with remote HBase servers, the Thrift protocol version that we follow is 0.9.2. However, within the NHBaseAPI, we have completely overwritten the Thrift protocol. That is to say, that we achieved an automatic high performance realization of the Thrift protocol following the binary feature of the original Thrift. The reasons of doing such are primarily the following two:

###- Performance Issue###
The original Thrift is mainly consisted of two parts (the Network Layer and the Serialization/Deserialization). First, when realizing the Network Layer, the original Thrift protocol stack fails to realize the routine scenario of sending a complete message package to the remote server. Such defect could lead to enormous waste of performance at the server end. And second as to the Serialization, the original Thrift realized it in a way that would produce huge amounts of memory fragments in the Managed Heap, which again lead to enormous consumption of memory.

###- BUG###
There is a bug in the original Thrift realization of the Network Layer. Whenever a message is sent, the SEQID from the protocol stack would always be 0. This bug has directly caused applications to malfunction under the multithreading circumstance, unless we manually control the synchronization of the multiple threads, which would then reduce the application’s output at a significant level.
Thus, when designing the interfaces, we have planned thoroughly and carefully, and have greatly lessen the pre-knowledge of using this API while satisfying routine usage. Apart from that, NHBaseAPI is also easy to use due to its inner operating mechanisms, such as automatic computing of the distribution of Region Servers, automatic reconnection, automatic load balance of multiple TCP SOCKET request, and etc.

##The Advantages of NHBaseAPI##
- Automatically acquire the resource information of HBase from remote ZooKeeper
- Automatically acquire the Region distribution information of tables before handling any HBase table operations, and automatically distribute the request to different Region Servers while handling such operations according to the distribution information
- Automatically update the Region information of HBase tables
- High performance
	- Very low .NET GC Time
	- Very few GC memory fragments
	- MSIL level realization of .NET reflection, with completely no boxing/unboxing operations
	- Adopt Unmanaged memory application and Managed pointer operation regarding the usage of some hot memory
	- Use understratum SOCKET plus Windows IOCP high performance network model to realize the Network Layer, ensuring the minimum cost of network connection
	- The Network Layer will maintain multiple TCL connection for each Region Server, so as to balance loads for each one
- Very friendly to use. Only a ConnectionStr is needed to initialize NHBaseAPI
- Provide interface for Batch writing request, and also support sending requests automatically in batches according to the number of Region Servers
- Decent global error handling
	- Complete network connection logs
	- Complete API operation logs
	- Support hints for bad requests on interface level. Provide data on bad requests for future retries
- ......

In the current interface design, we only provide some basic functions for HBase operations, and only provide synchronous methods for these operations. However in the following versions, we will provide asynchronous methods as well. Currently we support the following basic operations.

##NHBaseAPI Supported Functions##
- Table Level Operations
	- Create Table
	- Delete Table
	- Enable Table
	- Disable Table
	- Judge if the specified table is enabled 
	- List the name of all tables
- Data Operations
	- Insert a single line of data
	- Batch insert data
	- Read a line of data by RowKey
	- Read multiple lines of data by a range of RowKey
	- Atomic accumulation
- Clusters
	- Automatically acquire a RowKey distribution in a HBase table from ZooKeeper when creating a HBase table instance (HTable)
	- Support automatic update/maintenance of an HTable’s RowKey distribution during the table’s life cycle

##How to Use NHBaseAPI##
- Create an NHBaseAPI Client Instance
You may be surprised to find out that, you only need to input a ConnectionStr to create an NHBaseAPI Client Instance.
```csharp
string connectionStr = "zk=xxxxxx:2181,xxxxxx:2181,xxxxxx:2181";
IHBaseClient client = new HBaseClient(connectionStr)
```
A ConnectionStr may contain many settings. A simple way to initialize is to only put zk information in the ConnectionStr.
Settings in the ConnectionStr:

| Settings | Default | Description | Optional |
| -------------  | -------------  | ------------- | -------------  |
|zkTimeout|00:00:30|Session Timeout for remote ZooKeeper|Y|
|memSegSize|256|Size of a single memory segment when receiving network data. Unit: Byte. Value from 64 to 1024 in times of 64.|Y|
|memSegMultiples|100000|Size of unmanaged memory currently applied. Equals Multiples * SegmentSize|Y|
|minConnection|1|Minimum connections maintained for the same remote TCP IP and port|Y|
|maxConnection|3|Maximum connections maintained for the same remote TCP IP and port|Y|
|allowPrintDumpInfo|false|Allow or not the printing of network errors’ details when such internal errors occurred|Y|
|exclusiveMode|false|Forced region server mode, when exclusiveMode is true, client will initialize rsServers by given server list(rsServers)|N|
|rsServers|null|Forced region server list. format：`rsServers=ip:port,ip:port`|N|

- Create Table Operation
```csharp
//Initialization Omitted
IHBaseClient client = new HBaseClient(connectionStr)
IHTable table = client.CreateTable("TableName", "cf");
```

- Delete Table Operation
```csharp
//Initialization Omitted
IHBaseClient client = new HBaseClient(connectionStr)
client.DeleteTable("TableName");
```

- Insert a Single Line of Data Operation
```csharp
//Initialization Omitted
IHBaseClient client = new HBaseClient(connectionStr)
IHTable table = client.GetTable("TableName");
string tableName = string.Format("test_thrift_table_test_{0}", DateTime.Now.Millisecond);
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
- Read a line of data by RowKey
```csharp
//Initialization Omitted
IHBaseClient client = new HBaseClient(connectionStr)
IHTable table = client.GetTable("TableName");
byte[] rowKey = ...;
RowInfo info = table.GetRow(rowKey);
```
- Batch Insert Data Operation
```csharp
//Initialization Omitted
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
//Batch insert data, and use out data to receive those that failed when inserting
//Use the overall return value to judge if all the data in the current batch have been successfully inserted
bool result = table.BatchInsert(out exceptionBatchMutations, rows);
```
- Read multiple lines of data by a range of RowKey

```csharp
//Initialization Omitted
IHBaseClient client = new HBaseClient(connectionStr)
IHTable table = client.GetTable("TableName");
byte[] startRowKey = ...;
byte[] endRowKey = ...;
//Assign a column family in the test
List<string> columnFamily = new List<string> {"cf"};

//Read data from all the columns contained in the column family
Scanner scanner = table.NewScanner(startRowKey, endRowKey, columnFamily);
RowInfo info;
while ((info = scanner.GetNext()) != null)
{
	foreach (KeyValuePair<string, Cell> pair in info.Columns)
	{
	    // pair contains the retrieved data
	}
}

//Read data from a specified column in the column family
columnFamily = new List<string> {"cf:column1"};
scanner = table.NewScanner(startRowKey, endRowKey, columnFamily);
while ((info = scanner.GetNext()) != null)
{
	foreach (KeyValuePair<string, Cell> pair in info.Columns)
	{
	    // pair contains the retrieved data
	}
}
```

#### - Read multiple lines of data through the filter
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
			 //pair contains the retrieved data
		}
	}
}
catch(Exception ex)
{
	scanner.Dispose();
}
```

##Expectations##
We are eager and thankful to receive valuable suggestions from every developers. We will continue improving this API in the future, and will continually make more contributions for .NET open source community.
