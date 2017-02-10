using System.Collections.Generic;
using System.Net;
using NUnit.Framework;

namespace Gridsum.NHBaseThrift.UnitTests.Processors
{
	[TestFixture]
	class EndPointUnitTest
	{
		[Test]
		public void CompareTest()
		{
			IPEndPoint iep1 = new IPEndPoint(1000, 1000);
			IPEndPoint iep2 = new IPEndPoint(1000, 2000);
			Assert.IsFalse(Equals(iep1, iep2));

			Dictionary<IPEndPoint, List<string>> dic = new Dictionary<IPEndPoint, List<string>>();
			dic[iep1] = new List<string>{"1"};
			dic[iep2] = new List<string>{"2"};
			Assert.AreEqual(1, dic[iep1].Count);
			Assert.AreEqual("1", dic[iep1][0]);
			Assert.AreEqual(1, dic[iep2].Count);
			Assert.AreEqual("2", dic[iep2][0]);

			iep1 = new IPEndPoint(1000, 1000);
			iep2 = new IPEndPoint(1000, 1000);
			IPEndPoint iep3 = new IPEndPoint(1000, 1000);
			Assert.IsTrue(Equals(iep1, iep2));

			dic = new Dictionary<IPEndPoint, List<string>>();
			dic[iep1] = new List<string> { "1" };
			dic[iep2].Add("2");
			Assert.IsTrue(dic.ContainsKey(iep1));
			Assert.IsTrue(dic.ContainsKey(iep2));
			Assert.AreEqual(2, dic[iep1].Count);
			Assert.AreEqual("1",dic[iep1][0]);
			Assert.AreEqual(2, dic[iep2].Count);
			Assert.AreEqual("2", dic[iep2][1]);
			Assert.AreEqual(2, dic[iep3].Count);
			Assert.AreEqual("2", dic[iep2][1]);
		}
	}
}
