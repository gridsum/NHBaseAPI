using System;
using System.Net;

namespace NHBaseThrift.Proxies
{
    /// <summary>
    ///     内存片段接口
    /// </summary>
    public unsafe interface IMemorySegment
    {
        /// <summary>
        ///     获取当前内存片段的剩余可用长度
        /// </summary>
        uint RemainingSize { get; }
        /// <summary>
        ///     获取当前内存片段的可用偏移
        /// </summary>
        uint Offset { get; }
        /// <summary>
        ///     获取内存段内部的数据起始位置指针
        /// </summary>
        /// <returns>返回数据起始位置指针</returns>
        byte* GetPointer();
        /// <summary>
        ///     初始化当前内存片段
        /// </summary>
        IMemorySegment Initialize();
        /// <summary>
        ///     跳过指定字节长度
        /// </summary>
        /// <param name="length">需要跳过的字节长度</param>
        void Skip(uint length);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteInt32(int* value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteInt64(long* value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteInt16(short* value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteChar(char value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteUInt32(uint* value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteUInt64(ulong* value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteUInt16(ushort* value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteBoolean(bool value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteFloat(float* value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteDouble(double* value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteString(string value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteByte(byte value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteSByte(sbyte value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteDecimal(decimal* value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteDateTime(DateTime* value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteIntPtr(IntPtr* value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteGuid(Guid* value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteIPEndPoint(IPEndPoint value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteTimeSpan(TimeSpan* value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型值的内存地址</param>
        /// <param name="length">写入长度</param>
        void WriteMemory(IntPtr value, uint length);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="data">需要写入的内存</param>
        /// <param name="offset">起始内存偏移</param>
        /// <param name="length">写入长度</param>
        void WriteMemory(byte[] data, uint offset, uint length);
        /// <summary>
        ///     确定当前内存片段是否还有足够的大小空间
        /// </summary>
        /// <param name="size">需求的空间大小</param>
        /// <param name="remainingSize">剩余长度 </param>
        /// <returns>返回判断后的结果</returns>
        bool EnsureSize(uint size, out uint remainingSize);
    }
}