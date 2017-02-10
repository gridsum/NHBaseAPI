using System;
using System.Reflection;
using System.Text;
using Gridsum.NHBaseThrift.Engine;
using Gridsum.NHBaseThrift.Enums;
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
	class DisableTableRequestMessageTest
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
		public void SerializeTest()
		{
			DisableTableRequestMessage req = new DisableTableRequestMessage();
			req.TableName = "TableName1";
			req.Bind();
			Assert.IsTrue(req.IsBind);
			Assert.IsNotNull(req.Body);

			TMemoryStreamTransport stream = new TMemoryStreamTransport();
			Hbase.Client client = new Hbase.Client(new TBinaryProtocol(stream));
			client.send_disableTable(Encoding.UTF8.GetBytes(req.TableName));
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
			DisableTableRequestMessage req = new DisableTableRequestMessage();
			req.TableName = "TableName1";
			req.Bind();
			Assert.IsTrue(req.IsBind);
			Assert.IsNotNull(req.Body);

			TMemoryStreamTransport stream = new TMemoryStreamTransport();
			Hbase.Client client = new Hbase.Client(new TBinaryProtocol(stream));
			client.send_disableTable(Encoding.UTF8.GetBytes(req.TableName));
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
			DisableTableRequestMessage newObj;
			Assert.IsTrue(ThriftObjectEngine.TryGetObject<DisableTableRequestMessage>(container, out newObj) == GetObjectResultTypes.Succeed);
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
