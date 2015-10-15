using System;
using System.Collections.Generic;
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
    public class CreateTableRequestMessageTest
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
            CreateTableRequestMessage req = new CreateTableRequestMessage();
            req.TableName = "TableName1";
			req.ColumnFamilies = new[] { new Objects.ColumnDescriptor { Name = "cf" } };
            req.Bind();
            Assert.IsTrue(req.IsBind);
            Assert.IsNotNull(req.Body);

            TMemoryStreamTransport stream = new TMemoryStreamTransport();
            Hbase.Client client = new Hbase.Client(new TBinaryProtocol(stream));
            client.send_createTable(Encoding.UTF8.GetBytes(req.TableName), new List<ColumnDescriptor>{new ColumnDescriptor{Name = Encoding.UTF8.GetBytes("cf")}});
            byte[] originalData = stream.GetBuffer();

            Assert.IsTrue(originalData.Length == req.Body.Length);
            for (int i = 0; i < originalData.Length; i++)
            {
                bool result = originalData[i] == req.Body[i];
                if(!result) Console.WriteLine("Different index: " + i);
                Assert.IsTrue(result);
            }
        }

        [Test]
        public void SerializeTest2()
        {
            CreateTableRequestMessage req = new CreateTableRequestMessage();
            req.TableName = "TableName1";
			req.ColumnFamilies = new[] { new Gridsum.NHBaseThrift.Objects.ColumnDescriptor { Name = "cf" }, new Objects.ColumnDescriptor { Name = "xf" } };
            req.Bind();
            Assert.IsTrue(req.IsBind);
            Assert.IsNotNull(req.Body);

            TMemoryStreamTransport stream = new TMemoryStreamTransport();
            Hbase.Client client = new Hbase.Client(new TBinaryProtocol(stream));
            client.send_createTable(Encoding.UTF8.GetBytes(req.TableName), new List<ColumnDescriptor> { new ColumnDescriptor { Name = Encoding.UTF8.GetBytes("cf") }, new ColumnDescriptor { Name = Encoding.UTF8.GetBytes("xf") } });
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
            CreateTableRequestMessage req = new CreateTableRequestMessage();
            req.TableName = "TableName1";
            req.ColumnFamilies = new[] { new Objects.ColumnDescriptor { Name = "cf" } };
            req.Bind();
            Assert.IsTrue(req.IsBind);
            Assert.IsNotNull(req.Body);

            TMemoryStreamTransport stream = new TMemoryStreamTransport();
            Hbase.Client client = new Hbase.Client(new TBinaryProtocol(stream));
            client.send_createTable(Encoding.UTF8.GetBytes(req.TableName), new List<ColumnDescriptor> { new ColumnDescriptor { Name = Encoding.UTF8.GetBytes("cf") } });
            byte[] originalData = stream.GetBuffer();

            Assert.IsTrue(originalData.Length == req.Body.Length);
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
            CreateTableRequestMessage newObj;
            Assert.IsTrue(ThriftObjectEngine.TryGetObject<CreateTableRequestMessage>(container, out newObj) == GetObjectResultTypes.Succeed);
            Assert.IsNotNull(newObj);
            Assert.IsTrue(newObj.TableName == req.TableName);
            Assert.IsNotNull(newObj.ColumnFamilies);
            Assert.IsTrue(newObj.ColumnFamilies.Length == 1);
            Assert.IsTrue(newObj.ColumnFamilies[0].Name == "cf");
            Assert.IsTrue(newObj.ColumnFamilies[0].BloomFilterTypes == "NONE");
            Assert.IsTrue(newObj.ColumnFamilies[0].MaxVersions == 3);
            Assert.IsTrue(newObj.ColumnFamilies[0].Compression == "NONE");
            Assert.IsTrue(newObj.ColumnFamilies[0].TimeToLive == 0x7fffffff);
        }


        [Test]
        public void DeserializeTest2()
        {
            CreateTableRequestMessage req = new CreateTableRequestMessage();
            req.TableName = "TableName1";
            req.ColumnFamilies = new[] { new Objects.ColumnDescriptor { Name = "cf" }, new Objects.ColumnDescriptor { Name = "xf" } };
            req.Bind();
            Assert.IsTrue(req.IsBind);
            Assert.IsNotNull(req.Body);

            TMemoryStreamTransport stream = new TMemoryStreamTransport();
            Hbase.Client client = new Hbase.Client(new TBinaryProtocol(stream));
            client.send_createTable(Encoding.UTF8.GetBytes(req.TableName), new List<ColumnDescriptor> { new ColumnDescriptor { Name = Encoding.UTF8.GetBytes("cf") }, new ColumnDescriptor { Name = Encoding.UTF8.GetBytes("xf") } });
            byte[] originalData = stream.GetBuffer();

            Assert.IsTrue(originalData.Length == req.Body.Length);
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
            CreateTableRequestMessage newObj;
            Assert.IsTrue(ThriftObjectEngine.TryGetObject<CreateTableRequestMessage>(container, out newObj) == GetObjectResultTypes.Succeed);
            Assert.IsNotNull(newObj);
            Assert.IsTrue(newObj.TableName == req.TableName);
            Assert.IsNotNull(newObj.ColumnFamilies);
            Assert.IsTrue(newObj.ColumnFamilies.Length == 2);
            for (int i = 0; i < newObj.ColumnFamilies.Length; i++)
            {
                Assert.IsTrue(newObj.ColumnFamilies[i].Name == "cf" || newObj.ColumnFamilies[i].Name == "xf");
                Assert.IsTrue(newObj.ColumnFamilies[i].BloomFilterTypes == "NONE");
                Assert.IsTrue(newObj.ColumnFamilies[i].MaxVersions == 3);
                Assert.IsTrue(newObj.ColumnFamilies[i].Compression == "NONE");
                Assert.IsTrue(newObj.ColumnFamilies[i].TimeToLive == 0x7fffffff);
            }
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