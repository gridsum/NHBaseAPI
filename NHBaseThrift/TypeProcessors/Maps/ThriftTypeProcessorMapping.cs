using System;
using System.Collections.Generic;
using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Enums;
using KJFramework.Tracing;

namespace Gridsum.NHBaseThrift.TypeProcessors.Maps
{
    /// <summary>
    ///     Thrift协议类型处理器映射表，提供了相关的基本操作。
    /// </summary>
    public sealed class ThriftTypeProcessorMapping
    {
        #region Constructor.

        /// <summary>
        ///     Thrift协议类型处理器映射表，提供了相关的基本操作。
        /// </summary>
        private ThriftTypeProcessorMapping()
        {
            Initialize();
        }

        #endregion

        #region Members.

        public static readonly ThriftTypeProcessorMapping Instance = new ThriftTypeProcessorMapping();
        private static readonly ITracing _tracing = TracingManager.GetTracing(typeof(ThriftTypeProcessorMapping));
        private readonly Dictionary<Type, IThriftTypeProcessor> _processor = new Dictionary<Type, IThriftTypeProcessor>();
        public readonly static ThriftPropertyAttribute DefaultAttribute = new ThriftPropertyAttribute(0, PropertyTypes.Struct, false);

        #endregion

        #region Methods.

        /// <summary>
        ///     初始化所有系统内部提供的Thrift协议类型处理器
        /// </summary>
        private void Initialize()
        {
            Regist(new ByteThriftTypeProcessor());
            Regist(new BoolThriftTypeProcessor());
            Regist(new Int16ThriftTypeProcessor());
            Regist(new Int32ThriftTypeProcessor());
            Regist(new Int64ThriftTypeProcessor());
            Regist(new StringThriftTypeProcessor());
            Regist(new MessageIdentityTypeProcessor());
            Regist(new MapStringStringThriftTypeProcessor());
            Regist(new MapStringCellThriftTypeProcessor());
			Regist(new TScanThriftTypeProcessor());
        }

        /// <summary>
        ///     注册一个Thrift协议类型处理器
        ///     <para>* 如果该类型的处理器已经存在，则进行替换操作。</para>
        /// </summary>
        /// <param name="processor">Thrift协议类型处理器</param>
        public void Regist(IThriftTypeProcessor processor)
        {
            if (processor == null) return;
            try
            {
                if (_processor.ContainsKey(processor.SupportedType))
                {
                    _processor[processor.SupportedType] = processor;
                    return;
                }
                _processor.Add(processor.SupportedType, processor);
            }
            catch (Exception ex) { _tracing.Error(ex, null); }
        }

        /// <summary>
        ///     注销一个具有指定支持类型的Thrift协议类型处理器
        /// </summary>
        /// <param name="supportedType">支持的处理类型</param>
        public void UnRegist(Type supportedType)
        {
            if (supportedType == null) return;
            try {  _processor.Remove(supportedType); }
            catch (Exception ex) { _tracing.Error(ex, null); }
        }

        /// <summary>
        ///     获取一个具有指定支持类型的Thrift协议类型处理器
        /// </summary>
        /// <param name="supportedType">支持的处理类型</param>
        /// <returns>返回Thrift协议类型处理器</returns>
        public IThriftTypeProcessor GetProcessor(Type supportedType)
        {
            if (supportedType == null) return null;
            try
            {
                IThriftTypeProcessor result;
                return _processor.TryGetValue(supportedType, out result) ? result : null;
            }
            catch (Exception ex)
            {
                _tracing.Error(ex, null);
                return null;
            }
        }

        #endregion
    }
}