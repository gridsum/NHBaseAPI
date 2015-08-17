namespace NHBaseThrift.Helpers
{
    /// <summary>
    ///    类型扩展方法
    /// </summary>
    internal static class ExtensionMethods
    {
        #region Methods.

        /// <summary>
        ///    转换到大字节序
        /// </summary>
        /// <param name="value">需要转换的数值</param>
        public unsafe static short ToBigEndian(this short value)
        {
            byte* ptmpValue = stackalloc byte[2];
            ptmpValue[0] = (byte)(0xff & (value >> 8));
            ptmpValue[1] = (byte)(0xff & value);
            return *(short*)ptmpValue;
        }

        /// <summary>
        ///    转换到小字节序
        /// </summary>
        /// <param name="value">需要转换的数值</param>
        public unsafe static short ToLittleEndian(this short value)
        {
            byte* ptmpValue = (byte*)&value;
            return (short) (((ptmpValue[0] & 0xff) << 8) | ((ptmpValue[1] & 0xff)));
        }

        /// <summary>
        ///    转换到大字节序
        /// </summary>
        /// <param name="value">需要转换的数值</param>
        public unsafe static int ToBigEndian(this int value)
        {
            byte* ptmpValue = stackalloc byte[4];
            ptmpValue[0] = (byte)(0xff & (value >> 24));
            ptmpValue[1] = (byte)(0xff & (value >> 16));
            ptmpValue[2] = (byte)(0xff & (value >> 8));
            ptmpValue[3] = (byte)(0xff & value);
            return *(int*) ptmpValue;
        }

        /// <summary>
        ///    转换到小字节序
        /// </summary>
        /// <param name="value">需要转换的数值</param>
        public unsafe static int ToLittleEndian(this int value)
        {
            byte* ptmpValue = (byte*)&value;
            return ((ptmpValue[0] & 0xff) << 24) | ((ptmpValue[1] & 0xff) << 16) | ((ptmpValue[2] & 0xff) << 8) | ((ptmpValue[3] & 0xff));
        }

        /// <summary>
        ///    转换到大字节序
        /// </summary>
        /// <param name="value">需要转换的数值</param>
        public unsafe static long ToBigEndian(this long value)
        {
            byte* ptmpValue = stackalloc byte[8];
            ptmpValue[0] = (byte)(0xff & (value >> 56));
            ptmpValue[1] = (byte)(0xff & (value >> 48));
            ptmpValue[2] = (byte)(0xff & (value >> 40));
            ptmpValue[3] = (byte)(0xff & (value >> 32));
            ptmpValue[4] = (byte)(0xff & (value >> 24));
            ptmpValue[5] = (byte)(0xff & (value >> 16));
            ptmpValue[6] = (byte)(0xff & (value >> 8));
            ptmpValue[7] = (byte)(0xff & value);
            return *(long*)ptmpValue;
        }


        /// <summary>
        ///    转换到小字节序
        /// </summary>
        /// <param name="value">需要转换的数值</param>
        public unsafe static long ToLittleEndian(this long value)
        {
            byte* ptmpValue = (byte*)&value;
            return (
                  ((ptmpValue[0] & 0xff) << 56) |
                  ((ptmpValue[1] & 0xff) << 48) |
                  ((ptmpValue[2] & 0xff) << 40) |
                  ((ptmpValue[3] & 0xff) << 32) |
                  ((ptmpValue[4] & 0xff) << 24) |
                  ((ptmpValue[5] & 0xff) << 16) |
                  ((ptmpValue[6] & 0xff) << 8) |
                  ptmpValue[7] & 0xff);
        }

        #endregion
    }
}