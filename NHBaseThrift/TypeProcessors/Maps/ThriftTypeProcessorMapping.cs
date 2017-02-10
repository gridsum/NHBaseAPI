using System;
using System.Collections.Generic;
using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Enums;
using KJFramework.Tracing;

namespace Gridsum.NHBaseThrift.TypeProcessors.Maps
{
    /// <summary>
    ///     ThriftЭ�����ʹ�����ӳ����ṩ����صĻ���������
    /// </summary>
    public sealed class ThriftTypeProcessorMapping
    {
        #region Constructor.

        /// <summary>
        ///     ThriftЭ�����ʹ�����ӳ����ṩ����صĻ���������
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
        ///     ��ʼ������ϵͳ�ڲ��ṩ��ThriftЭ�����ʹ�����
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
        ///     ע��һ��ThriftЭ�����ʹ�����
        ///     <para>* ��������͵Ĵ������Ѿ����ڣ�������滻������</para>
        /// </summary>
        /// <param name="processor">ThriftЭ�����ʹ�����</param>
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
        ///     ע��һ������ָ��֧�����͵�ThriftЭ�����ʹ�����
        /// </summary>
        /// <param name="supportedType">֧�ֵĴ�������</param>
        public void UnRegist(Type supportedType)
        {
            if (supportedType == null) return;
            try {  _processor.Remove(supportedType); }
            catch (Exception ex) { _tracing.Error(ex, null); }
        }

        /// <summary>
        ///     ��ȡһ������ָ��֧�����͵�ThriftЭ�����ʹ�����
        /// </summary>
        /// <param name="supportedType">֧�ֵĴ�������</param>
        /// <returns>����ThriftЭ�����ʹ�����</returns>
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