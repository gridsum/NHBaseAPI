using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Gridsum.NHBaseThrift.Engine;
using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Helpers;
using Gridsum.NHBaseThrift.Messages;
using Gridsum.NHBaseThrift.Network;
using Gridsum.NHBaseThrift.Proxies;
using KJFramework.Cache.Cores;
using KJFramework.Net.Channels;
using KJFramework.Net.Channels.Caches;
using KJFramework.Net.Channels.Events;
using NUnit.Framework;
using Thrift.Protocol;
using MemorySegment = KJFramework.Cache.Objects.MemorySegment;

namespace Gridsum.NHBaseThrift.UnitTests.Messages
{
	[TestFixture]
	class InsertNewRowRequestMessageTest
	{
		#region Methods.

		[SetUp]
		public void Initialize()
		{
			ChannelConst.Initialize();
			InitializeSegments();
			ThriftProtocolMemoryAllotter.InitializeEnvironment(256, 100000);
		}

		[Test]
		[Description("无Attribute。插一列")]
		public void SerializeTest()
		{
			InsertNewRowRequestMessage req = new InsertNewRowRequestMessage();
			req.TableName = "TableName1";
			req.RowKey = TypeConversionHelper.StringToByteArray("123");
			req.Mutations = new[]
			{
				new Objects.Mutation
				{
					ColumnName = "f:c1",
					Value = TypeConversionHelper.StringToByteArray("value1")
				}
			};
			req.Bind();
			Assert.IsTrue(req.IsBind);
			Assert.IsNotNull(req.Body);

			TMemoryStreamTransport stream = new TMemoryStreamTransport();
			Hbase.Client client = new Hbase.Client(new TBinaryProtocol(stream));
			client.send_mutateRow(Encoding.UTF8.GetBytes(req.TableName), req.RowKey, 
				new List<Mutation>
				{
					new Mutation
					{
						Column = Encoding.UTF8.GetBytes("f:c1"),
						Value = Encoding.UTF8.GetBytes("value1")
					}
				}, null);
			byte[] originalData = stream.GetBuffer();

			Assert.IsTrue(originalData.Length == req.Body.Length);
			for (int i = 0; i < originalData.Length; i++)
			{
				bool result = originalData[i] == req.Body[i];
			    if (!result) Console.WriteLine("Different index: " + i);
                Assert.IsTrue(result);
			}
		}

		[Test]
		[Description("带Attribute。插一列")]
		public void Serialize1Test()
		{
			InsertNewRowRequestMessage req = new InsertNewRowRequestMessage();
			req.TableName = "TableName1";
			req.RowKey = TypeConversionHelper.StringToByteArray("123");
            req.Mutations = new[]{
				new Objects.Mutation
					{
						ColumnName = "f:c1",
						Value = TypeConversionHelper.StringToByteArray("value1")
					}};
			req.Attributes = new Dictionary<string, string> {{"key","value"}};
			req.Bind();
			Assert.IsTrue(req.IsBind);
			Assert.IsNotNull(req.Body);

			TMemoryStreamTransport stream = new TMemoryStreamTransport();
			Hbase.Client client = new Hbase.Client(new TBinaryProtocol(stream));
			client.send_mutateRow(Encoding.UTF8.GetBytes(req.TableName), (req.RowKey),
				new List<Mutation>
				{
					new Mutation
					{
						Column = Encoding.UTF8.GetBytes("f:c1"),
						Value = Encoding.UTF8.GetBytes("value1")
					}
				}, new Dictionary<byte[], byte[]> { {Encoding.UTF8.GetBytes("key"), Encoding.UTF8.GetBytes("value") }});
			byte[] originalData = stream.GetBuffer();

			Assert.AreEqual(originalData.Length, req.Body.Length);
			for (int i = 0; i < originalData.Length; i++)
			{
				bool result = originalData[i] == req.Body[i];
				if (!result) Console.WriteLine("Different index: " + i);
				Assert.IsTrue(result);
			}
		}

		[Test]
		[Description("无Attribute。插2列")]
		public void Serialize2Test()
		{
			InsertNewRowRequestMessage req = new InsertNewRowRequestMessage();
			req.TableName = "TableName1";
			req.RowKey = TypeConversionHelper.StringToByteArray("123");
			req.Mutations = new[]
			{
				new Objects.Mutation
				{
					ColumnName = "f:c1",
					Value = TypeConversionHelper.StringToByteArray("value1")
				},
				new Objects.Mutation
				{
					ColumnName = "f:c2",
					Value = TypeConversionHelper.StringToByteArray("value2")
				}
			};
			req.Bind();
			Assert.IsTrue(req.IsBind);
			Assert.IsNotNull(req.Body);

			TMemoryStreamTransport stream = new TMemoryStreamTransport();
			Hbase.Client client = new Hbase.Client(new TBinaryProtocol(stream));
			client.send_mutateRow(Encoding.UTF8.GetBytes(req.TableName), (req.RowKey),
				new List<Mutation>
				{
					new Mutation
					{
						Column = Encoding.UTF8.GetBytes("f:c1"),
						Value = Encoding.UTF8.GetBytes("value1")
					},
					new Mutation
					{
						Column = Encoding.UTF8.GetBytes("f:c2"),
						Value = Encoding.UTF8.GetBytes("value2")
					},
				}, null);
			byte[] originalData = stream.GetBuffer();

			Assert.IsTrue(originalData.Length == req.Body.Length);
			for (int i = 0; i < originalData.Length; i++)
			{
				bool result = originalData[i] == req.Body[i];
				if (!result) Console.WriteLine("Different index: " + i);
				Assert.IsTrue(result);
			}
		}

		[Test]
		public void DeserializeTest()
		{
			InsertNewRowRequestMessage req = new InsertNewRowRequestMessage();
			req.TableName = "TableName1";
			req.RowKey = TypeConversionHelper.StringToByteArray("123");
            req.Mutations = new[]{
				new Objects.Mutation
					{
						ColumnName = "f:c1",
						Value = TypeConversionHelper.StringToByteArray("value1")
					},
				new Objects.Mutation
					{
						ColumnName = "f:c2",
						Value = TypeConversionHelper.StringToByteArray("value2")
					}};
			req.Attributes = null;
			req.Bind();
			Assert.IsTrue(req.IsBind);
			Assert.IsNotNull(req.Body);

			TMemoryStreamTransport stream = new TMemoryStreamTransport();
			Hbase.Client client = new Hbase.Client(new TBinaryProtocol(stream));
			client.send_mutateRow(Encoding.UTF8.GetBytes(req.TableName), req.RowKey,
				new List<Mutation>
				{
					new Mutation
					{
						Column = Encoding.UTF8.GetBytes("f:c1"),
						Value = Encoding.UTF8.GetBytes("value1")
					},
					new Mutation
					{
						Column = Encoding.UTF8.GetBytes("f:c2"),
						Value = Encoding.UTF8.GetBytes("value2")
					}
				}, null);
			byte[] originalData = stream.GetBuffer();

			Assert.AreEqual(originalData.Length, req.Body.Length);
			for (int i = 0; i < originalData.Length; i++)
			{
				bool result = originalData[i] == req.Body[i];
				if (!result) Console.WriteLine("Different index: " + i);
				Assert.IsTrue(result);
			}

			byte[] data = originalData;
			IFixedCacheStub<SocketBuffStub> fixedStub = new CacheStub<SocketBuffStub>();
			SocketBuffStub stub = new SocketBuffStub();
			((ICacheStub<SocketBuffStub>)fixedStub).Cache.SetValue(stub);
			SetMemorySegment(stub, new MemorySegment(new ArraySegment<byte>(data, 0, data.Length)));
			INetworkDataContainer container = new NetworkDataContainer();
			container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub, data.Length));
			InsertNewRowRequestMessage newObj;
			Assert.IsTrue(ThriftObjectEngine.TryGetObject(container, out newObj) == GetObjectResultTypes.Succeed);
			Assert.IsNotNull(newObj);
			Assert.IsTrue(newObj.TableName == req.TableName);
			Assert.IsNotNull(newObj.TableName);
			Assert.AreEqual("TableName1", newObj.TableName);
		}

		private void SetMemorySegment(SocketBuffStub stub, MemorySegment segment)
		{
			FieldInfo fieldInfo = stub.GetType().GetField("_segment", BindingFlags.Instance | BindingFlags.NonPublic);
			fieldInfo.SetValue(stub, segment);
		}

		private void InitializeSegments()
		{
			MethodInfo methodInfo = ChannelConst.SegmentContainer.GetType().GetMethod("Initialize", BindingFlags.Instance | BindingFlags.NonPublic);
			methodInfo.Invoke(ChannelConst.SegmentContainer, null);
		}

		#endregion
	}
}
