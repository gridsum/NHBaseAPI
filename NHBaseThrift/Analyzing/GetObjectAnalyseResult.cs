using System;
using System.Reflection;
using System.Text;
using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Helpers;
using Gridsum.NHBaseThrift.Network;
using Gridsum.NHBaseThrift.Stubs;

namespace Gridsum.NHBaseThrift.Analyzing
{    
    /// <summary>
    ///     字符串转换约束委托
    /// </summary>
    /// <param name="type">转换的类型</param>
    /// <param name="stringBuilder">字符串构建器</param>
    /// <param name="property">字段信息</param>
    /// <param name="instance">对象实例</param>
    /// <param name="space">缩进空间</param>
    /// <param name="isArrayLoop">是否陷入数组循环的标示</param>
    public delegate void ToStringHandler(Type type, StringBuilder stringBuilder, PropertyInfo property, Object instance, string space, bool isArrayLoop);

    /// <summary>
    ///     可转化为对象的分析结果，提供了相关的基本操作。
    /// </summary>
    public class GetObjectAnalyseResult : AnalyseResult
    {
        #region Members.

        private FastInvokeHandler _delegate;
        private FastInvokeHandler _getDelegate;
        private IPropertySetStub _setStub;
        /// <summary>
        ///     热处理函数
        /// </summary>
        internal Func<object, GetObjectAnalyseResult, INetworkDataContainer, GetObjectResultTypes> CacheProcess;
        /// <summary>
        ///     字符串转换器
        /// </summary>
        public ToStringHandler StringConverter;
        /// <summary>
        ///      获取或设置目标对象类型
        /// </summary>
        public Type TargetType;
        /// <summary>
        ///    获取一个值，该值标示了当从原始byte数组解析成为此类型时所期望传入的最小可用数据长度
        /// </summary>
        public int ExpectedDataSize;
        /// <summary>
        ///    尝试检查一下当前所需要解析的数据可用长度是否满足此类型的解析需求
        ///    <para>* 此方法只有当ExpectedDataSize = -1时才会被调用</para>
        /// </summary>
        internal Func<INetworkDataContainer, bool> HasEnoughData;

        #endregion

        #region Methods

        /// <summary>
        ///     获取当前字段值
        /// </summary>
        /// <param name="instance">对象实例</param>
        /// <returns>值</returns>
        public Object GetValue(Object instance)
        {
            if (_getDelegate == null)
            {
                MethodInfo methodInfo = Property.GetGetMethod(true);
                _getDelegate = DynamicHelper.GetMethodInvoker(methodInfo);
            }
            return _getDelegate(instance, null);
        }

        /// <summary>
        ///     初始化
        /// </summary>
        public GetObjectAnalyseResult Initialize()
        {
            if (_setStub == null)
            {
                MethodInfo methodInfo = Property.GetSetMethod(true);
                _setStub = PropertySetStubHelper.Create(TargetType, Property.PropertyType);
                _setStub.Initialize(methodInfo);
            }
            return this;
        }

        /// <summary>
        ///  设置当前属性值
        /// </summary>
        /// <typeparam name="T">属性类型</typeparam>
        /// <param name="instance">目标对象</param>
        /// <param name="value">属性数值</param>
        public void SetValue<T>(object instance, T value)
        {
            _setStub.Set(instance, value);
        }

        /// <summary>
        ///     为当前字段设置值
        /// </summary>
        /// <param name="instance">对象实例</param>
        /// <param name="args">值</param>
        public void SetValue(object instance, params object[] args)
        {
            if (_delegate == null)
            {
                MethodInfo methodInfo = Property.GetSetMethod(true);
                _delegate = DynamicHelper.GetMethodInvoker(methodInfo);
            }
            _delegate(instance, args);
        }

        #endregion
    }
}