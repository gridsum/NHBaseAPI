using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Gridsum.NHBaseThrift.Client;
using Gridsum.NHBaseThrift.Helpers;
using Gridsum.NHBaseThrift.Objects;

namespace NHBaseThrift.BatchInsertTest
{
	class Program
	{
		static void InsertHugeDataTest(int rowNum, int bytesNum, int batchSize, string startKey, string endKey, int testCount)
		{
			//new MemoryFailPoint(10);
			IHBaseClient client = new HBaseClient("zk=10.200.200.56:2181,10.200.200.57:2181,10.200.200.58:2181;zkTimeout=00:05:00;memSegSize=1048576;memSegMultiples=1000");
			string tableName = string.Format("mediad_test_thrift_table_test_{0}", DateTime.Now.Second);
			string hugeData = new string('a', bytesNum);
			Stopwatch sw = new Stopwatch();
			Stopwatch sw2 = new Stopwatch();
			Console.WriteLine(((rowNum * Encoding.UTF8.GetBytes(hugeData).Length).ToString()));
			try
			{
				IHTable table = client.CreateTable(tableName, "cf");
				BatchMutation[] rows = new BatchMutation[rowNum];
				for (int i = 1; i <= rowNum; i++)
				{
					rows[i-1] = new BatchMutation
					{
						RowKey = TypeConversionHelper.StringToByteArray(i.ToString()),
						Mutations = new[]
						{
							new Mutation {ColumnName = "cf:col1", Value = TypeConversionHelper.StringToByteArray("value"+i)},
							new Mutation {ColumnName = "cf:col2", Value = TypeConversionHelper.StringToByteArray(hugeData+i)}
						}
					};
				}
				BatchMutation[] exceptionBatchMutations;
				List<Thread> threads = new List<Thread>();
				for (int i = 0; i < testCount; i++)
				{
					threads.Add(new Thread(() =>
					{
						Stopwatch swthread = new Stopwatch();
						swthread.Restart();
						Console.WriteLine(table.BatchInsert(out exceptionBatchMutations, rows));
						swthread.Stop();
						Console.WriteLine((swthread.ElapsedTicks / (decimal)Stopwatch.Frequency));
					}));
				}
				foreach (Thread thread in threads)
					thread.Start();
				foreach (Thread thread in threads)
					thread.Join();
				Console.WriteLine("=End=");

				Console.WriteLine("GetRows batchSize:{1} 1~{0}:", rowNum, batchSize);
				Scanner scanner = table.NewScanner(new byte[]{0}, new byte[]{0xff, 0xff, 0xff}, new List<string> {"cf:col1", "cf:col2"});

				Dictionary<string, string> tmpDictionary = new Dictionary<string, string>();
				sw.Restart();
				RowInfo info;
				while ((info = scanner.GetNext()) != null)
				{
					//Console.WriteLine("read row {0}", info.RowKey);
					foreach (KeyValuePair<string, Cell> pair in info.Columns)
					{

					}
				}
				Console.WriteLine(sw.ElapsedTicks / (decimal)Stopwatch.Frequency);
				if (rowNum != tmpDictionary.Keys.Count) Console.WriteLine("rowNum != tmpDictionary.Keys.Count");
				for (int i = 1; i <= rowNum; i++)
				{
					if (!(hugeData + i).Equals(tmpDictionary[i.ToString()]))
					{
						Console.WriteLine("content not eqal!");
						return;
					}
				}
			}
			finally
			{
				client.DeleteTable(tableName);
			}
		}
		unsafe static void Main(string[] args)
		{
			int rowNum = Convert.ToInt32(ConfigurationManager.AppSettings["rowNumber"]);
			int bytesNumber = Convert.ToInt32(ConfigurationManager.AppSettings["bytesNumber"]);
			int batchSize = Convert.ToInt32(ConfigurationManager.AppSettings["batchSize"]);
			int testCount = Convert.ToInt32(ConfigurationManager.AppSettings["testCount"]);
			string startKey = (ConfigurationManager.AppSettings["startKey"]);
			string endKey = (ConfigurationManager.AppSettings["endKey"]);
			Console.WriteLine("rowNumber:{0}, bytesNumber:{1}, total bytes:{2}",rowNum, bytesNumber, rowNum*bytesNumber);
			while(true)
			{
				InsertHugeDataTest(rowNum, bytesNumber, batchSize, startKey, endKey, testCount);
				Console.ReadLine();
			}
		}
	}
}
