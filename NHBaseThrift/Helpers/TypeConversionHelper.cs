using System;
using System.Text;

namespace Gridsum.NHBaseThrift.Helpers
{
	/// <summary>
	///		类型转换帮助类
	/// </summary>
	public static class TypeConversionHelper
	{
		#region Methods.

		/// <summary>
		///		byte数组转换为long类型
		/// </summary>
		/// <param name="bytes">byte数组</param>
		/// <returns>long类型结果</returns>
		public static long ByteArraryTolong(byte[] bytes)
		{
			return BitConverter.ToInt64(bytes, 0).ToLittleEndian();
		}

		/// <summary>
		///		byte数组转换为string类型
		/// </summary>
		/// <param name="bytes">byte数组</param>
		/// <returns>string类型结果</returns>
		public static string ByteArraryToString(byte[] bytes)
		{
			return Encoding.UTF8.GetString(bytes);
		}

		/// <summary>
		///		string类型转换为byte数组类型
		/// </summary>
		/// <param name="str">string</param>
		/// <returns>byte数组类型结果</returns>
		public static byte[] StringToByteArray(string str)
		{
			return Encoding.UTF8.GetBytes(str);
		}

		#endregion
	}
}
