using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Gridsum.NHBaseThrift.Helpers;
using Gridsum.NHBaseThrift.Messages;
using Gridsum.NHBaseThrift.Proxies;
using KJFramework.Net.Channels;
using KJFramework.Net.Channels.Caches;
using NUnit.Framework;
using Thrift.Protocol;
using MemorySegment = KJFramework.Cache.Objects.MemorySegment;

namespace Gridsum.NHBaseThrift.UnitTests.Messages
{
	[TestFixture]
	class ScannerOpenWithStopTest
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
			ScannerOpenWithStopRequestMessage req = new ScannerOpenWithStopRequestMessage();
			req.TableName = "TableName1";
			req.StartRow = TypeConversionHelper.StringToByteArray("row0");
			req.EndRow = TypeConversionHelper.StringToByteArray("row100");
			req.Columns = new []
			{
				"cf:col"
			};
			req.Bind();
			Assert.IsTrue(req.IsBind);
			Assert.IsNotNull(req.Body);

			TMemoryStreamTransport stream = new TMemoryStreamTransport();
			Hbase.Client client = new Hbase.Client(new TBinaryProtocol(stream));
			client.send_scannerOpenWithStop(Encoding.UTF8.GetBytes(req.TableName),
				req.StartRow,
				req.EndRow,
				new List<byte[]>
				{
					Encoding.UTF8.GetBytes("cf:col")
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
