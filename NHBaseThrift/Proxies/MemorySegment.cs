using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using KJFramework.Core.Native;

namespace Gridsum.NHBaseThrift.Proxies
{
    /// <summary>
    ///     内存片段
    /// </summary>
    internal unsafe sealed class MemorySegment : IMemorySegment
    {
        #region Constructor

        /// <summary>
        ///     内存片段
        /// </summary>
        /// <param name="data">总内存数据段</param>
        /// <param name="startOffset">当前内存段起始位置</param>
        /// <param name="length">当前内存段可用长度</param>
        public MemorySegment(byte* data, uint startOffset, uint length)
        {
            _data = data;
            _startOffset = startOffset;
            _length = length;
            _endOffset = _startOffset + length;
            _startData = (_data + startOffset);
        }

        #endregion

        #region Members

        private readonly byte* _data;
        private readonly byte* _startData;
        private readonly uint _startOffset;
        private readonly uint _endOffset;
        private readonly uint _length;
        private uint _currentOffset;

        #endregion

        #region Implementation of IMemorySegment

        /// <summary>
        ///     获取当前内存片段的剩余可用长度
        /// </summary>
        public uint RemainingSize { get { return ThriftProtocolMemoryAllotter.SegmentSize - _currentOffset; } }
        /// <summary>
        ///     获取当前内存片段的可用偏移
        /// </summary>
        public uint Offset { get { return _currentOffset; } }

        /// <summary>
        ///     获取内存段内部的数据起始位置指针
        /// </summary>
        /// <returns>返回数据起始位置指针</returns>
        public byte* GetPointer()
        {
            return _startData;
        }

        /// <summary>
        ///     初始化当前内存片段
        /// </summary>
        public IMemorySegment Initialize()
        {
            _currentOffset = 0;
            return this;
        }

        /// <summary>
        ///     跳过指定字节长度
        /// </summary>
        /// <param name="length">需要跳过的字节长度</param>
        public void Skip(uint length)
        {
            for (int i = 0; i < length; i++)
                *(&_startData[_currentOffset++]) = 0x00;
        }

        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        public void WriteInt32(int* value)
        {
            *(int*)(&_startData[_currentOffset]) = *value;
            _currentOffset += Size.Int32;
        }

        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        public void WriteInt64(long* value)
        {
            *(long*)(&_startData[_currentOffset]) = *value;
            _currentOffset += Size.Int64;
        }

        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        public void WriteInt16(short* value)
        {
            *(short*)(&_startData[_currentOffset]) = *value;
            _currentOffset += Size.Int16;
        }

        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        public void WriteChar(char value)
        {
            *(char*)(&_startData[_currentOffset]) = value;
            _currentOffset += Size.Char;
        }

        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        public void WriteUInt32(uint* value)
        {
            *(uint*)(&_startData[_currentOffset]) = *value;
            _currentOffset += Size.UInt32;
        }

        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        public void WriteUInt64(ulong* value)
        {
            *(ulong*)(&_startData[_currentOffset]) = *value;
            _currentOffset += Size.UInt64;
        }

        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        public void WriteUInt16(ushort* value)
        {
            *(ushort*)(&_startData[_currentOffset]) = *value;
            _currentOffset += Size.UInt16;
        }

        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        public void WriteBoolean(bool value)
        {
            *(bool*)(&_startData[_currentOffset]) = value;
            _currentOffset += Size.Bool;
        }

        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        public void WriteFloat(float* value)
        {
            *(float*)(&_startData[_currentOffset]) = *value;
            _currentOffset += Size.Float;
        }

        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        public void WriteDouble(double* value)
        {
            *(double*)(&_startData[_currentOffset]) = *value;
            _currentOffset += Size.Double;
        }

        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        public void WriteString(string value)
        {
            byte[] strBytes;
            fixed (byte* pByte = (strBytes = Encoding.UTF8.GetBytes(value)))
                Native.Win32API.memcpy((IntPtr)(&_startData[_currentOffset]), (IntPtr)pByte, (uint)strBytes.Length);
            _currentOffset += (uint)strBytes.Length;
        }

        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        public void WriteByte(byte value)
        {
            *&_startData[_currentOffset] = value;
            _currentOffset += Size.Byte;
        }

        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        public void WriteSByte(sbyte value)
        {
            *(sbyte*)(&_startData[_currentOffset]) = value;
            _currentOffset += Size.SByte;
        }

        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        public void WriteDecimal(decimal* value)
        {
            *(decimal*)(&_startData[_currentOffset]) = *value;
            _currentOffset += Size.Decimal;
        }

        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        public void WriteDateTime(DateTime* value)
        {
            *(long*)(&_startData[_currentOffset]) = (*value).Ticks;
            _currentOffset += Size.DateTime;
        }

        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        public void WriteIntPtr(IntPtr* value)
        {
            *(int*)(&_startData[_currentOffset]) = (*value).ToInt32();
            _currentOffset += Size.IntPtr;
        }

        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        public void WriteGuid(Guid* value)
        {
            *(Guid*)(&_startData[_currentOffset]) = *value;
            _currentOffset += Size.Guid;
        }

        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        public void WriteIPEndPoint(IPEndPoint value)
        {
            byte* pByte = &_startData[_currentOffset];
            *(long*)(pByte) = value.Address.Address;
            *(int*)(pByte + 8) = value.Port;
            _currentOffset += Size.IPEndPoint;
        }

        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型的值</param>
        public void WriteTimeSpan(TimeSpan* value)
        {
            *(long*)(&_startData[_currentOffset]) = (*value).Ticks;
            _currentOffset += Size.TimeSpan;
        }

        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="value">指定类型值的内存地址</param>
        /// <param name="length">写入长度</param>
        public void WriteMemory(IntPtr value, uint length)
        {
            Native.Win32API.memcpy(new IntPtr(_startData + _currentOffset), value, length);
            _currentOffset += length;
        }

        /// <summary>
        ///     写入一个指定类型的值
        /// </summary>
        /// <param name="data">需要写入的内存</param>
        /// <param name="offset">起始内存偏移</param>
        /// <param name="length">写入长度</param>
        public void WriteMemory(byte[] data, uint offset, uint length)
        {
            Marshal.Copy(data, (int)offset, new IntPtr(_startData + _currentOffset), (int)length);
            _currentOffset += length;
        }

        /// <summary>
        ///     确定当前内存片段是否还有足够的大小空间
        /// </summary>
        /// <param name="size">需求的空间大小</param>
        /// <param name="remainingSize">剩余长度 </param>
        /// <returns>返回判断后的结果</returns>
        public bool EnsureSize(uint size, out uint remainingSize)
        {
            remainingSize = RemainingSize;
            return remainingSize >= size;
        }

        #endregion
    }
}