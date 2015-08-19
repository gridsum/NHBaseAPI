using System;

namespace Gridsum.NHBaseThrift.Proxies
{
    /// <summary>
    ///     内部用来保存每一个数据类型大小的容器
    /// </summary>
    internal static class Size
    {
        public const uint Bool = sizeof (bool);
        public const uint Char = sizeof (char);
        public const uint Byte = sizeof (byte);
        public const uint SByte = sizeof (sbyte);
        public const uint Decimal = sizeof (decimal);
        public const uint Int16 = sizeof (short);
        public const uint UInt16 = sizeof (ushort);
        public const uint Float = sizeof (float);
        public const uint Int32 = sizeof (int);
        public const uint UInt32 = sizeof (uint);
        public const uint UInt64 = sizeof (ulong);
        public const uint Int64 = sizeof (long);
        public const uint Double = sizeof (double);
        public const uint DateTime = sizeof (long);
        public const uint IntPtr = sizeof (int);
        public static readonly unsafe uint Guid = (uint) sizeof(Guid);
        public const uint BitFlag = 1;
        public const uint IPEndPoint = sizeof (long) + sizeof (int);
        public static readonly unsafe uint TimeSpan = (uint) sizeof (TimeSpan);
    }
}