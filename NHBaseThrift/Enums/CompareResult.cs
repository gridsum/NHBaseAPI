namespace Gridsum.NHBaseThrift.Enums
{
	/// <summary>
	///		比较结果
	/// </summary>
	public enum CompareResult : byte
	{
		/// <summary>
		///		相等
		/// </summary>
		Eq = 0x00,
		/// <summary>
		///		大于
		/// </summary>
		Lt = 0x01,
		/// <summary>
		///		小于
		/// </summary>
		Gt = 0x02
	}
}
