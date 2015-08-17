using NHBaseThrift.Enums;

namespace NHBaseThrift.Comparator
{
	/// <summary>
	///		比较接口
	/// </summary>
	public interface IByteArrayComparator
	{
		/// <summary>
		///		按字节比较两字符串.
		///		<para>*两字符串按字节逐位比较</para>
		///		<para>*如果对应位置上arr1的字节大于等于arr2的字节就返回true</para>
		///		<para>*如果对应位置上arr1的字节小于arr2的字节就返回true</para>
		/// </summary>
		/// <returns>arr1大于arr2</returns>
		CompareResult Compare(byte[] arr1, byte[] arr2);
	}
}
