using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Gridsum.NHBaseThrift.Helpers;
using Gridsum.NHBaseThrift.Messages;
using Gridsum.NHBaseThrift.Proxies;
using KJFramework.Cache.Objects;
using KJFramework.Net.Channels;
using KJFramework.Net.Channels.Caches;
using NUnit.Framework;
using Thrift.Protocol;

namespace Gridsum.NHBase.UnitTests.Messages
{
	[TestFixture]
	class ScannerOpenWithScanTest
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
			ScannerOpenWithScanRequestMessage req = new ScannerOpenWithScanRequestMessage();

			string tableName = "TableName1";
			string startRow = "row0";
			string stopRow = "row100";
			long timeStamp = 111;
			string[] Columns ={ "cf:col" };
			int caching = 222;
			string filterString = "filter";
			int batchSize = 333;
			bool sortColumns = false;
			bool reversed = false;

			req.TableName = tableName;
			NHBaseThrift.Objects.TScan scan = new NHBaseThrift.Objects.TScan();
			scan.StartRow = TypeConversionHelper.StringToByteArray(startRow);
			scan.StopRow = TypeConversionHelper.StringToByteArray(stopRow);
			//scan.Timestamp = timeStamp;
			scan.Columns = Columns;
			scan.Caching = caching;
			scan.FilterString = filterString;
			//scan.BatchSize = batchSize;
			scan.SortColumns = sortColumns;
			scan.Reversed = reversed;
			req.Scan = scan;
			req.Bind();
			Assert.IsTrue(req.IsBind);
			Assert.IsNotNull(req.Body);

			TMemoryStreamTransport stream = new TMemoryStreamTransport();
			Hbase.Client client = new Hbase.Client(new TBinaryProtocol(stream));
			TScan scanOrigion = new TScan();
			scanOrigion.StartRow = TypeConversionHelper.StringToByteArray(startRow);
			scanOrigion.StopRow = TypeConversionHelper.StringToByteArray(stopRow);
			//scanOrigion.Timestamp = timeStamp;
			scanOrigion.Columns = new List<byte[]>
			{
				TypeConversionHelper.StringToByteArray("cf:col")
			};
			scanOrigion.Caching = caching;
			scanOrigion.FilterString = TypeConversionHelper.StringToByteArray(filterString);
			//scanOrigion.BatchSize = batchSize;
			scanOrigion.SortColumns = sortColumns;
			scanOrigion.Reversed = reversed;
			client.send_scannerOpenWithScan(Encoding.UTF8.GetBytes(req.TableName),
				scanOrigion, null);
			byte[] originalData = stream.GetBuffer();

			foreach (byte b in originalData)
			{
				Console.Write("{0}\n", b);
			}
			Console.WriteLine("===");
			foreach (byte b in req.Body)
			{
				Console.Write("{0}\n",b);
			}

			for (int i = 0; i < originalData.Length; i++)
			{
				bool result = originalData[i] == req.Body[i];
				if (!result) Console.WriteLine("Different index: " + i);
				Assert.IsTrue(result);
			}
			Assert.AreEqual(originalData.Length, req.Body.Length);
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
