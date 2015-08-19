using Gridsum.NHBaseThrift.Enums;

namespace Gridsum.NHBaseThrift.Comparator
{
	/// <summary>
	///		字节数组比较器
	/// </summary>
	public class ByteArrayComparator : IByteArrayComparator
	{
		#region Methods.

		/// <summary>
		///		按字节比较两字符串.
		///		<para>*两字符串按字节逐位比较</para>
		///		<para>*如果对应位置上arr1的字节大于等于arr2的字节就返回true</para>
		///		<para>*如果对应位置上arr1的字节小于arr2的字节就返回true</para>
		/// </summary>
		/// <returns>arr1大于arr2</returns>
		public CompareResult Compare(byte[] arr1, byte[] arr2)
		{
			if (arr1 == arr2) return CompareResult.Eq;
			if (arr1 == null && arr2 != null) return CompareResult.Lt;
			if (arr2 == null && arr1 != null) return CompareResult.Gt;
			int len = ((arr1.Length > arr2.Length) ? arr2.Length : arr1.Length);
			for (int i = 0; i < len; i++)
			{
				if (arr1[i] > arr2[i]) return CompareResult.Gt;
				if (arr1[i] < arr2[i]) return CompareResult.Lt;
			}
			if (arr1.Length > arr2.Length) return CompareResult.Gt;
			if (arr1.Length < arr2.Length) return CompareResult.Lt;
			return CompareResult.Eq;
		}

		#endregion
	}
}
