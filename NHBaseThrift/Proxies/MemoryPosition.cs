using System.Runtime.InteropServices;

namespace NHBaseThrift.Proxies
{
    /// <summary>
    ///     内存位置基础结构
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MemoryPosition
    {
        #region Constructor

        /// <summary>
        ///     内存位置基础结构
        /// </summary>
        /// <param name="index">内存片段索引</param>
        /// <param name="offset">内存片段偏移</param>
        public MemoryPosition(int index, uint offset)
        {
            SegmentIndex = index;
            SegmentOffset = offset;
        }

        #endregion

        #region Members

        /// <summary>
        ///     内存片段索引
        /// </summary>
        public int SegmentIndex;
        /// <summary>
        ///     内存片段偏移
        /// </summary>
        public uint SegmentOffset;

        #endregion

        #region Methods

        /// <summary>
        ///     计算截止位置与开始位置之间的距离
        /// </summary>
        /// <param name="segmentCount">内存段数量</param>
        /// <param name="start">起始位置</param>
        /// <param name="end">截止位置</param>
        /// <returns>返回它们之间所差的距离</returns>
        public static int CalcLength(int segmentCount, MemoryPosition start, MemoryPosition end)
        {
            return (int) (((end.SegmentIndex)*ThriftProtocolMemoryAllotter.SegmentSize + end.SegmentOffset) -
                          ((start.SegmentIndex)*ThriftProtocolMemoryAllotter.SegmentSize + start.SegmentOffset));
        }

        #endregion
    }
}