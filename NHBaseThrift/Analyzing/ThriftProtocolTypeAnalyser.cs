using System;
using System.Collections.Concurrent;
using NHBaseThrift.Helpers;
using NHBaseThrift.Stubs;

namespace NHBaseThrift.Analyzing
{
    /// <summary>
    ///     类型分析器，提供了相关的基本操作。
    /// </summary>
    internal abstract class ThriftProtocolTypeAnalyser<T, K> : IThriftProtocolTypeAnalyser<T, K>
    {
        #region Members.

        protected ConcurrentDictionary<string, T> _result = new ConcurrentDictionary<string, T>();

        #endregion

        #region Methods..

        /// <summary>
        ///     获取指定对象
        /// </summary>
        /// <param name="token">类型编号</param>
        /// <returns>返回分析结果</returns>
        protected T GetObject(string token)
        {
            T result;
            return _result.TryGetValue(token, out result) ? result : default(T);
        }

        /// <summary>
        ///     注册一个分析结果
        /// </summary>
        /// <param name="token">类型编号</param>
        /// <param name="result">分析结果</param>
        protected void RegistAnalyseResult(string token, T result)
        {
            if (_result.ContainsKey(token)) return;
            if (!_result.TryAdd(token, result))
                throw new Exception("Cannot regist an analyze result! #type token: " + token);
        }

        #endregion

        #region Implementation of IIntellectTypeAnalyser<T>

        /// <summary>
        ///     分析一个类型中的Thrift协议属性
        /// </summary>
        /// <param name="type">要分析的类型</param>
        /// <returns>返回分析的结果</returns>
        public abstract T Analyse(K type);

        /// <summary>
        ///     清空当前所有的分析结果
        /// </summary>
        public void Clear()
        {
            _result.Clear();
        }

        
        protected VT GetVT(Type type)
        {
            Type innerType;
            if (type.IsEnum) return FixedTypeManager.IsVT(type.GetEnumUnderlyingType());
            if ((innerType = Nullable.GetUnderlyingType(type)) != null) return FixedTypeManager.IsVT(innerType);
            return FixedTypeManager.IsVT(type);
        }

        #endregion
    }
}