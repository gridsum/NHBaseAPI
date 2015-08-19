using System.Net;
using Gridsum.NHBaseThrift.Comparator;
using Gridsum.NHBaseThrift.Enums;

namespace Gridsum.NHBaseThrift
{
    /// <summary>
    ///     业务对象，用于存储远程Region服务器信息
    /// </summary>
    internal class RegionInfo
    {
		#region Members.

		/// <summary>
        ///    获取或设置行键开始数据
        /// </summary>
        public byte[] StartKey { get; set; }
        /// <summary>
        ///    获取或设置行键结束位置
        /// </summary>
        public byte[] EndKey { get; set; }
        /// <summary>
        ///    获取或设置编号信息
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        ///    获取或设置名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///    获取或设置版本信息
        /// </summary>
        public byte Version { get; set; }
        /// <summary>
        ///    获取或设置远程服务器地址
        /// </summary>
        public IPEndPoint Address { get; set; }
		/// <summary>
		///		获取或设置字符数组比较器
		/// </summary>
		public IByteArrayComparator Comparator { get; set; }

        #endregion

        #region Methods.

        /// <summary>
        ///     检查指定行键是否符合当前RegionServer的范围
        /// </summary>
        /// <param name="rowKey">行键</param>
        /// <returns>如果返回True, 则证明传入的行键符合当前的RegionServer数据分布范围</returns>
        public bool IsMatch(byte[] rowKey)
        {
            if ((StartKey == null || StartKey.Length == 0) && (EndKey == null || EndKey.Length == 0)) return true;
			if ((EndKey == null || EndKey.Length == 0) && ((Comparator.Compare(StartKey, rowKey) == CompareResult.Lt) || (Comparator.Compare(StartKey, rowKey) == CompareResult.Eq))) return true;
			if ((StartKey == null || StartKey.Length == 0) && ((Comparator.Compare(EndKey, rowKey) == CompareResult.Gt) || (Comparator.Compare(EndKey, rowKey) == CompareResult.Eq))) return true;
			if (((Comparator.Compare(EndKey, rowKey) == CompareResult.Gt) || (Comparator.Compare(EndKey, rowKey) == CompareResult.Eq)) &&
				((Comparator.Compare(StartKey, rowKey) == CompareResult.Lt) || (Comparator.Compare(StartKey, rowKey) == CompareResult.Eq)))
		        return true;
            return false;
        }

        #endregion
    }
}