using System.Text;
using Gridsum.NHBaseThrift;
using Gridsum.NHBaseThrift.Comparator;
using NUnit.Framework;

namespace Gridsum.NHBaseThrift.UnitTests.Processors
{
	[TestFixture]
	class RegionInfoUnitTest
	{
		#region Members.

		private static RegionInfo _info;

		#endregion

		#region Methods.

		[SetUp]
		public void Initialize()
		{
			_info = new RegionInfo {Comparator = new ByteArrayComparator()};
		}

		[Test]
		[Description("region范围为全集匹配测试")]
		public void MatchTest1()
		{
			_info.StartKey = null;
			_info.EndKey = null;
			byte[] rowkey = Encoding.UTF8.GetBytes("row");
			Assert.IsTrue(_info.IsMatch(rowkey));
			rowkey = Encoding.UTF8.GetBytes("");
			Assert.IsTrue(_info.IsMatch(rowkey));
		}

		[Test]
		[Description("region有上界无下界匹配测试")]
		public void MatchTest12()
		{
			_info.Comparator = new ByteArrayComparator();
			_info.StartKey = null;
			_info.EndKey = Encoding.UTF8.GetBytes("11");
			byte[] rowkey = Encoding.UTF8.GetBytes("1");
			Assert.IsTrue(_info.IsMatch(rowkey));
			rowkey = Encoding.UTF8.GetBytes("2");
			Assert.IsFalse(_info.IsMatch(rowkey));
			rowkey = Encoding.UTF8.GetBytes("211");
			Assert.IsFalse(_info.IsMatch(rowkey));
		}

		[Test]
		[Description("region有下界无上界匹配测试")]
		public void MatchTest3()
		{
			_info.StartKey = Encoding.UTF8.GetBytes("99");
			_info.EndKey = null;
			byte[] rowkey = Encoding.UTF8.GetBytes("999");
			Assert.IsTrue(_info.IsMatch(rowkey));
			rowkey = Encoding.UTF8.GetBytes("89");
			Assert.IsFalse(_info.IsMatch(rowkey));
			rowkey = Encoding.UTF8.GetBytes("8999");
			Assert.IsFalse(_info.IsMatch(rowkey));
		}

		[Test]
		[Description("region有上下界匹配测试")]
		public void MatchTest4()
		{
			_info.StartKey = Encoding.UTF8.GetBytes("21");
			_info.EndKey = Encoding.UTF8.GetBytes("29");
			byte[] rowkey = Encoding.UTF8.GetBytes("22");
			Assert.IsTrue(_info.IsMatch(rowkey));
			rowkey = Encoding.UTF8.GetBytes("32");
			Assert.IsFalse(_info.IsMatch(rowkey));
			rowkey = Encoding.UTF8.GetBytes("322");
			Assert.IsFalse(_info.IsMatch(rowkey));
			rowkey = Encoding.UTF8.GetBytes("12");
			Assert.IsFalse(_info.IsMatch(rowkey));
			rowkey = Encoding.UTF8.GetBytes("122");
			Assert.IsFalse(_info.IsMatch(rowkey));
		}

		#endregion
	}
}
