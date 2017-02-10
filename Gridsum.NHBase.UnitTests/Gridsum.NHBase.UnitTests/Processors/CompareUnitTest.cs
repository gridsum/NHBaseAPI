using System.Text;
using Gridsum.NHBaseThrift.Comparator;
using Gridsum.NHBaseThrift.Enums;
using NUnit.Framework;

namespace Gridsum.NHBaseThrift.UnitTests.Processors
{
	[TestFixture]
	class CompareUnitTest
	{
		#region Methods.
		private static ByteArrayComparator _comparator;

		[SetUp]
		public void Initialize()
		{
			_comparator = new ByteArrayComparator();
		}
		[Test]
		public void CompareTest1()
		{
			Assert.AreEqual(CompareResult.Eq, _comparator.Compare(null, null));

			byte[] bytes1 = Encoding.UTF8.GetBytes("123");
			byte[] bytes2 = Encoding.UTF8.GetBytes("123");
			Assert.AreEqual(CompareResult.Eq, _comparator.Compare(bytes1, bytes2));
		}

		[Test]
		public void CompareTest2()
		{
			byte[] bytes1 = Encoding.UTF8.GetBytes("1");
			byte[] bytes2 = Encoding.UTF8.GetBytes("2");
			Assert.AreEqual(CompareResult.Lt, _comparator.Compare(bytes1, bytes2));

			bytes1 = Encoding.UTF8.GetBytes("1");
			bytes2 = Encoding.UTF8.GetBytes("123");
			Assert.AreEqual(CompareResult.Lt, _comparator.Compare(bytes1, bytes2));

			Assert.AreEqual(CompareResult.Lt, _comparator.Compare(null, bytes2));
		}

		[Test]
		public void CompareTest3()
		{
			byte[] bytes1 = Encoding.UTF8.GetBytes("2");
			byte[] bytes2 = Encoding.UTF8.GetBytes("1");
			Assert.AreEqual(CompareResult.Gt, _comparator.Compare(bytes1, bytes2));

			bytes1 = Encoding.UTF8.GetBytes("2");
			bytes2 = Encoding.UTF8.GetBytes("11");
			Assert.AreEqual(CompareResult.Gt, _comparator.Compare(bytes1, bytes2));

			Assert.AreEqual(CompareResult.Gt, _comparator.Compare(bytes1, null));
		}
		#endregion
	}
}
