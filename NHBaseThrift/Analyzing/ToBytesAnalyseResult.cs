using System;
using System.Diagnostics;
using System.Reflection;
using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Proxies;
using Gridsum.NHBaseThrift.Stubs;

namespace Gridsum.NHBaseThrift.Analyzing
{
    /// <summary>
    ///     可转化为元数据的分析结果，提供了相关的基本操作。
    /// </summary>
    [DebuggerDisplay("Type: {Property.PropertyType}")]
    public class ToBytesAnalyseResult : AnalyseResult
    {
        #region Members

        private IPropertyStub _stub;
        /// <summary>
        ///     获取或设置目标对象类型
        /// </summary>
        public Type TargetType { get; set; }
   

        /// <summary>
        ///     热处理函数
        /// </summary>
        public Action<IMemorySegmentProxy, ThriftPropertyAttribute, ToBytesAnalyseResult, object, bool, bool> CacheProcess;

        #endregion

        #region Methods

        /// <summary>
        ///     获取当前字段值
        /// </summary>
        /// <param name="instance">对象实例</param>
        /// <returns>值</returns>
        public T GetValue<T>(Object instance)
        {
            return _stub.Get<T>(instance);
        }

        /// <summary>
        ///     初始化
        /// </summary>
        public ToBytesAnalyseResult Initialize()
        {
            if (_stub == null)
            {
                MethodInfo methodInfo = Property.GetGetMethod(true);
                _stub = PropertyStubHelper.Create(TargetType, Property.PropertyType, methodInfo);
                _stub.Initialize(methodInfo);
            }
            return this;
        }

        #endregion
    }
}