using System;
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

	public class Int16Test1 : ThriftMessage
	{
		[ThriftProperty(1, PropertyTypes.I16)]
		public short Id1 { get; set; }
	}

	#endregion

	[TestFixture]
	public class Int16UnitTests
	{
		#region Methods.

		[SetUp]
		public void Initialize()
		{
			ChannelConst.Initialize();
			InitializeSegments();
			ThriftProtocolMemoryAllotter.InitializeEnvironment(256, 100000);
		}

		public byte[] SerializeTest()
		{
			Int16Test1 obj = new Int16Test1 { Id1 = 17, Identity = new MessageIdentity { Command = "test", CommandLength = 4U, SequenceId = 1, Version = 3 } };
			obj.Bind();
			Assert.IsTrue(obj.IsBind);
			Assert.IsNotNull(obj.Body);
			Assert.IsTrue(obj.Body.Length == 22);
			unsafe
			{
				fixed (byte* pByte = obj.Body)
				{
					//MessageIdentity
					Assert.IsTrue((*(int*)pByte).ToLittleEndian() == 3);
					Assert.IsTrue((*(int*)(pByte + 4)).ToLittleEndian() == 4);
					Assert.IsTrue((*(pByte + 8)) == 't');
					Assert.IsTrue((*(pByte + 9)) == 'e');
					Assert.IsTrue((*(pByte + 10)) == 's');
					Assert.IsTrue((*(pByte + 11)) == 't');
					Assert.IsTrue((*(int*)(pByte + 12)).ToLittleEndian() == 1);
					//Field
					Assert.IsTrue((PropertyTypes)(*(pByte + 16)) == PropertyTypes.I16);
					Assert.IsTrue((*(short*)(pByte + 17)).ToLittleEndian() == 1);
					//Value
					Assert.IsTrue(obj.Id1 == (*(short*)(pByte + 19)).ToLittleEndian());
				}
			}
			return obj.Body;
		}

		[Test]
		public void DeserializeTest()
		{
			byte[] data = SerializeTest();
			Assert.IsNotNull(data);
			IFixedCacheStub<SocketBuffStub> fixedStub = new CacheStub<SocketBuffStub>();
			SocketBuffStub stub = new SocketBuffStub();
			((ICacheStub<SocketBuffStub>)fixedStub).Cache.SetValue(stub);
			SetMemorySegment(stub, new MemorySegment(new ArraySegment<byte>(data, 0, data.Length)));
			INetworkDataContainer container = new NetworkDataContainer();
			container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub, data.Length));

			Int16Test1 newObj;
			Assert.IsTrue(ThriftObjectEngine.TryGetObject(container, out newObj) == GetObjectResultTypes.Succeed);
			Assert.IsNotNull(newObj);
			Assert.IsNotNull(newObj.Identity.Command);
			Assert.IsTrue(newObj.Identity.Command == "test");
			Assert.IsTrue(newObj.Identity.Version == 3);
			Assert.IsTrue(newObj.Identity.CommandLength == 4);
			Assert.IsTrue(newObj.Identity.SequenceId == 1);
			Assert.IsTrue(newObj.Id1 == 17);
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