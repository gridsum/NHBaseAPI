using System;
using System.Collections.Generic;
using System.Reflection;
using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Engine;
using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Messages;
using Gridsum.NHBaseThrift.Network;
using Gridsum.NHBaseThrift.Objects;
using Gridsum.NHBaseThrift.Proxies;
using Gridsum.NHBaseThrift.Helpers;
using KJFramework.Cache.Cores;
using KJFramework.Net.Channels;
using KJFramework.Net.Channels.Caches;
using KJFramework.Net.Channels.Events;
using NUnit.Framework;
using MemorySegment = KJFramework.Cache.Objects.MemorySegment;

namespace Gridsum.NHBaseThrift.UnitTests.Processors
{
    #region Assembled Classes.
    public class MapStringCellTest1 : ThriftMessage
    {
        [ThriftProperty(1, PropertyTypes.Map)]
        public Dictionary<string, Cell> Id1 { get; set; }
    }

    #endregion

    [TestFixture]
    public class MapStringCellUnitTests
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
		[Ignore]
        public byte[] SerializeTest()
        {
            MapStringCellTest1 obj = new MapStringCellTest1() { Id1 = new Dictionary<string, Cell> { { "k", new Cell {Value = TypeConversionHelper.StringToByteArray("v"), Timestamp = 123} } }, Identity = new MessageIdentity { Command = "test", CommandLength = 4U, SequenceId = 1, Version = 3 } };
            obj.Bind();
            Assert.IsTrue(obj.IsBind);
            Assert.IsNotNull(obj.Body);
            Assert.AreEqual(51, obj.Body.Length);
            unsafe
            {
                fixed (byte* pByte = obj.Body)
                {
                    Assert.IsTrue((*(int*)pByte).ToLittleEndian() == 3);
                    Assert.IsTrue((*(int*)(pByte + 4)).ToLittleEndian() == 4);
                    Assert.IsTrue((*(pByte + 8)) == 't');
                    Assert.IsTrue((*(pByte + 9)) == 'e');
                    Assert.IsTrue((*(pByte + 10)) == 's');
                    Assert.IsTrue((*(pByte + 11)) == 't');
                    Assert.IsTrue((*(int*)(pByte + 12)).ToLittleEndian() == 1);
                    Assert.IsTrue((PropertyTypes)(*(pByte + 16)) == PropertyTypes.Map);
                    Assert.IsTrue((*(short*)(pByte + 17)).ToLittleEndian() == 1);
                    Assert.IsTrue((PropertyTypes)(*(pByte + 19)) == PropertyTypes.String);
                    Assert.IsTrue((PropertyTypes)(*(pByte + 20)) == PropertyTypes.Struct);
                    Assert.IsTrue((*(int*)(pByte + 21)).ToLittleEndian() == 1);
                    Assert.IsTrue((*(int*)(pByte + 25)).ToLittleEndian() == 1);
                    Assert.IsTrue((*(pByte + 29)) == 'k');
                    Assert.IsTrue((PropertyTypes)(*(pByte + 30)) == PropertyTypes.String);
                    Assert.IsTrue((*(short*)(pByte + 31)).ToLittleEndian() == 1);
                    Assert.IsTrue((*(int*)(pByte + 33)).ToLittleEndian() == 1);
                    Assert.IsTrue((*(pByte + 37)) == 'v');
                    Assert.IsTrue((PropertyTypes)(*(pByte + 38)) == PropertyTypes.I64);
                    Assert.IsTrue((*(short*)(pByte + 39)).ToLittleEndian() == 2);
                    Assert.IsTrue((*(long*)(pByte + 41)).ToLittleEndian() == 123);
                }
            }
            return obj.Body;
        }

        [Test]
        public void DeserializeTest()
        {
            byte[] data = SerializeTest();
            IFixedCacheStub<SocketBuffStub> fixedStub = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub).Cache.SetValue(stub);
            SetMemorySegment(stub, new MemorySegment(new ArraySegment<byte>(data, 0, data.Length)));
            INetworkDataContainer container = new NetworkDataContainer();
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub, data.Length));

            MapStringCellTest1 newObj;
            GetObjectResultTypes type = ThriftObjectEngine.TryGetObject(container, out newObj);
            Assert.IsTrue(type == GetObjectResultTypes.Succeed);
            Assert.IsNotNull(newObj);
            Assert.IsNotNull(newObj.Identity.Command);
            Assert.IsTrue(newObj.Identity.Command == "test");
            Assert.IsTrue(newObj.Identity.Version == 3);
            Assert.IsTrue(newObj.Identity.CommandLength == 4);
            Assert.IsTrue(newObj.Identity.SequenceId == 1);
			Assert.AreEqual(newObj.Id1["k"].Value ,TypeConversionHelper.StringToByteArray("v"));
            Assert.IsTrue(newObj.Id1["k"].Timestamp == 123);
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
