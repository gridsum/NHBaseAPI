using System;
using System.Collections.Generic;
using System.Net;
using NHBaseThrift.Stubs;

namespace NHBaseThrift.Helpers
{
    /// <summary>
    ///     固定类型管理器，为固定字节数序列化/反序列化的能力提供基础支持
    /// </summary>
    public static class FixedTypeManager
    {
        #region Constructor.

        /// <summary>
        ///     固定类型管理器，为固定字节数序列化/反序列化的能力提供基础支持
        /// </summary>
        static FixedTypeManager()
        {
            _vt = new Dictionary<string, VT>();
            _vt.Add(typeof(bool).FullName, new VT { Size = 1 });
            _vt.Add(typeof(char).FullName, new VT { Size = 1 });
            _vt.Add(typeof(byte).FullName, new VT { Size = 1 });
            _vt.Add(typeof(sbyte).FullName, new VT { Size = 1 });
            _vt.Add(typeof(decimal).FullName, new VT { Size = 16 });
            _vt.Add(typeof(short).FullName, new VT { Size = 2 });
            _vt.Add(typeof(ushort).FullName, new VT { Size = 2 });
            _vt.Add(typeof(float).FullName, new VT { Size = 4 });
            _vt.Add(typeof(int).FullName, new VT { Size = 4 });
            _vt.Add(typeof(uint).FullName, new VT { Size = 4 });
            _vt.Add(typeof(ulong).FullName, new VT { Size = 8 });
            _vt.Add(typeof(long).FullName, new VT { Size = 8 });
            _vt.Add(typeof(double).FullName, new VT { Size = 8 });
            _vt.Add(typeof(DateTime).FullName, new VT { Size = 8 });
            _vt.Add(typeof(IntPtr).FullName, new VT { Size = 4 });
            _vt.Add(typeof(Guid).FullName, new VT { Size = 16 });
            _vt.Add(typeof(IPEndPoint).FullName, new VT { Size = 12 });
            _vt.Add(typeof(TimeSpan).FullName, new VT { Size = 8 });
        }

        #endregion

        #region Members.

        private static readonly Dictionary<string, VT> _vt;

        #endregion

        #region Methods.

        /// <summary>
        ///     判断一个指定类型是否支持固定字节数序列化/反序列化
        /// </summary>
        /// <param name="type">要检测的类型</param>
        /// <returns></returns>
        public static VT IsVT(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            VT vt;
            return _vt.TryGetValue(type.FullName, out vt) ? vt : null;
        }

        /// <summary>
        ///     将一个类型添加为支持固定字节数的序列化/反序列化
        ///     <para>* 请在程序初始化的时候调用此方法</para>
        /// </summary>
        /// <param name="type">需要添加的类型</param>
        /// <param name="size">固定字节数</param>
        /// <returns>返回添加后的状态</returns>
        /// <exception cref="ArgumentNullException">参数不能为空</exception>
        /// <exception cref="ArgumentException">参数错误</exception>
        public static bool Add(Type type, int size)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (size <= 0) throw new ArgumentException("Illegal parameter: #size.");
            if (_vt.ContainsKey(type.FullName)) return false;
            _vt.Add(type.FullName, new VT { Size = size });
            return true;
        }

        #endregion
    }
}