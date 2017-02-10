using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using MemorySegment = KJFramework.Cache.Objects.MemorySegment;

namespace Gridsum.NHBaseThrift.UnitTests.Messages
{
	[TestFixture]
	class InsertRowsRequestMessageTest
	{
		#region Methods.

		[SetUp]
		public void Initialize()
		{
			ChannelConst.Initialize();
			InitializeSegments();
			ThriftProtocolMemoryAllotter.InitializeEnvironment(256, 100000);
		}
		#endregion


		[Test]
		public void SerializeTest()
		{
			InsertNewRowsRequestMessage req = new InsertNewRowsRequestMessage();
			req.TableName = "TableName1";
			req.RowBatch = new[]
			{
				new Objects.BatchMutation()
				{
					RowKey = TypeConversionHelper.StringToByteArray("row"),
					Mutations = new[]
					{
						new Objects.Mutation()
						{
							ColumnName = "cf:col1",
							Value =TypeConversionHelper.StringToByteArray("value1")
						}
					}
				}
			};

			req.Bind();
			Assert.IsTrue(req.IsBind);
			Assert.IsNotNull(req.Body);

			TMemoryStreamTransport stream = new TMemoryStreamTransport();
			Hbase.Client client = new Hbase.Client(new TBinaryProtocol(stream));
			client.send_mutateRows(Encoding.UTF8.GetBytes(req.TableName), new List<BatchMutation>
				{
					new BatchMutation
					{
						Row = Encoding.UTF8.GetBytes("row"),
						Mutations = new List<Mutation>
						{
							new Mutation
							{
								Column = Encoding.UTF8.GetBytes("cf:col1"),
								Value = Encoding.UTF8.GetBytes("value1")
							}
						}
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

		[Test]
		[Description("大量序列化性能测试")]
		[Ignore("耗时操作")]
		public void SerializeTest1()
		{
			ThriftProtocolMemoryAllotter.InitializeEnvironment(262144, 1000);
			int dataNumber = 50;
			string testString = new string('a', 1024*1024);
			Objects.BatchMutation[] rows = new Objects.BatchMutation[dataNumber];
			for (int i = 0; i < dataNumber; i++)
			{
				rows[i] = new Objects.BatchMutation
				{
					RowKey = TypeConversionHelper.StringToByteArray("row" + i),
					Mutations = new[]
						{
							new Objects.Mutation {ColumnName = "cf:col1", Value = TypeConversionHelper.StringToByteArray("value1")},
							new Objects.Mutation {ColumnName = "cf:col2", Value = TypeConversionHelper.StringToByteArray(testString)}
						}
				};
			}
			InsertNewRowsRequestMessage req = new InsertNewRowsRequestMessage();
			req.TableName = "TableName1";
			req.RowBatch = rows;

			Stopwatch watcher = Stopwatch.StartNew();
			for (int i = 0; i < 10000; i++)
			{
				req.Bind();
			}
			watcher.Stop();
			Console.WriteLine("Elapsed:" + watcher.Elapsed);
		}

	}
}
