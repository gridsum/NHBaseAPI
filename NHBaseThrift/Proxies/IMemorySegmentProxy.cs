using System;
using System.Net;

namespace Gridsum.NHBaseThrift.Proxies
{
    /// <summary>
    ///     内存片段代理器
    /// </summary>
    public unsafe interface IMemorySegmentProxy : IDisposable
    {
        /// <summary>
        ///     获取当前代理器内部所包含的内存片段个数
        /// </summary>
        int SegmentCount { get; }
        /// <summary>
        ///     跳过指定字节长度
        /// </summary>
        /// <param name="length">需要跳过的字节长度</param>
        void Skip(uint length);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteInt32(int value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteInt64(long value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteInt64(long* value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteInt16(short value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteChar(char value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteUInt32(uint value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteUInt64(ulong value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteUInt64(ulong* value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteUInt16(ushort value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteBoolean(bool value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteFloat(float value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteDouble(double value);
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
        void WriteDecimal(decimal value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteDecimal(decimal* value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteDateTime(DateTime value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteIntPtr(IntPtr value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteGuid(Guid value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteGuid(Guid* value);
        /// <summary>
        ///     向指定内存段的指定偏移处回写一个int32数值
        /// </summary>
        /// <param name="position">回写位置</param>
        /// <param name="value">回写值</param>
        void WriteBackInt32(MemoryPosition position, int value);
        /// <summary>
        ///     向指定内存段的指定偏移处回写一个int16数值
        /// </summary>
        /// <param name="position">回写位置</param>
        /// <param name="value">回写值</param>
        void WriteBackInt16(MemoryPosition position, short value);
        /// <summary>
        ///     向指定内存段的指定偏移处回写一个uint16数值
        /// </summary>
        /// <param name="position">回写位置</param>
        /// <param name="value">回写值</param>
        void WriteBackUInt16(MemoryPosition position, ushort value);
        /// <summary>
        ///     向指定内存段的指定偏移处回写一个uint32数值
        /// </summary>
        /// <param name="position">回写位置</param>
        /// <param name="value">回写值</param>
        void WriteBackUInt32(MemoryPosition position, uint value);
        /// <summary>
        ///     向指定内存段的指定偏移处回写一个void*
        /// </summary>
        /// <param name="position">回写位置</param>
        /// <param name="value">回写值</param>
        /// <param name="length">回写长度</param>
        void WriteBackMemory(MemoryPosition position, void* value, uint length);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteIPEndPoint(IPEndPoint value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        void WriteTimeSpan(TimeSpan value);
        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型值的内存地址</param>
        /// <param name="length">写入长度</param>
        void WriteMemory(void* value, uint length);

        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="data">需要写入的内存</param>
        /// <param name="offset">起始内存偏移</param>
        /// <param name="length">写入长度</param>
        void WriteMemory(byte[] data, uint offset, uint length);
        /// <summary>
        ///     获取一个当前内部缓冲区位置的记录点
        /// </summary>
        /// <returns>返回内部缓冲区位置的记录点</returns>
        MemoryPosition GetPosition();
        /// <summary>
        ///     获取内部的缓冲区内存
        /// </summary>
        ///     <para>* 使用此方法后总是会强制回收内部资源</para>
        /// <returns>返回缓冲区内存</returns>
        byte[] GetBytes();
    }
}