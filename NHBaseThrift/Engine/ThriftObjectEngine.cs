using System;
using System.Collections.Generic;
using KJFramework.Tracing;
using NHBaseThrift.Analyzing;
using NHBaseThrift.Attributes;
using NHBaseThrift.Contracts;
using NHBaseThrift.Enums;
using NHBaseThrift.Exceptions;
using NHBaseThrift.Helpers;
using NHBaseThrift.Network;
using NHBaseThrift.Proxies;
using NHBaseThrift.Stubs;
using NHBaseThrift.TypeProcessors;
using NHBaseThrift.TypeProcessors.Maps;

namespace NHBaseThrift.Engine
{
    /// <summary>
    ///     Thrift对象引擎，提供了相关的基本操作
    /// </summary>
    public class ThriftObjectEngine
    {
        #region Members

        private static readonly ITracing _tracing = TracingManager.GetTracing(typeof(ThriftObjectEngine));

        #endregion

        #region Methods.

        /// <summary>
        ///     将一个智能对象转换为二进制元数据
        /// </summary>
        /// <param name="obj">智能对象</param>
        /// <returns>返回二进制元数据</returns>
        /// <exception cref="PropertyNullValueException">字段相关的Attribute.IsRequire = true, 并且该字段的值为null</exception>
        /// <exception cref="UnexpectedValueException">不期待的结果异常，通常因为对Blob类型的取值 = null</exception>
        /// <exception cref="NotSupportedException">系统不支持的序列化类型</exception>
        /// <exception cref="DefineNoMeaningException">无意义的智能字段Attribute值</exception>
        /// <exception cref="MethodAccessException">类型权限定义错误</exception>
        /// <exception cref="Exception">内部错误</exception>
        public static byte[] ToBytes(IThriftObject obj)
        {
            if (obj == null) return null;
            IMemorySegmentProxy proxy = null;
            try
            {
                proxy = MemorySegmentProxyFactory.Create();
                ToBytes(obj, proxy);
                //end flag of Thrift protocol object.
                proxy.WriteSByte(0x00);
                return proxy.GetBytes();
            }
            catch(Exception ex)
            {
                if (proxy != null) proxy.Dispose();
                throw;
            }
        }

        /// <summary>
        ///     将一个智能对象转换为二进制元数据
        /// </summary>
        /// <param name="obj">智能对象</param>
        /// <param name="proxy">内存段代理器</param>
        /// <returns>返回当前已经被序列化对象的总体长度</returns>
        internal static void ToBytes(IThriftObject obj, IMemorySegmentProxy proxy)
        {
            //获取智能对象中的智能属性，并按照Id来排序
            ToBytesAnalyseResult[] properties = Analyser.ToBytesAnalyser.Analyse(obj);
            if (properties.Length == 0) return;
            for (int l = 0; l < properties.Length; l++)
            {
                ToBytesAnalyseResult property = properties[l];
                //先检查完全缓存机制
                if (property.HasCacheFinished)
                {
                    property.CacheProcess(proxy, property.Attribute, property, obj, false, false);
                    continue;
                }

                #region 普通类型判断

                IThriftTypeProcessor intellectTypeProcessor = ThriftTypeProcessorMapping.Instance.GetProcessor(property.Property.PropertyType);
                if (intellectTypeProcessor != null)
                {
                    //添加热缓存
                    IThriftTypeProcessor processor = intellectTypeProcessor;
                    property.CacheProcess = processor.Process;
                    property.CacheProcess(proxy, property.Attribute, property, obj, false, false);
                    property.HasCacheFinished = true;
                    continue;
                }

                #endregion

                #region 枚举类型判断

                //枚举类型
                if (property.Property.PropertyType.IsEnum)
                {
                    //获取枚举类型
                    Type enumType = Enum.GetUnderlyingType(property.Property.PropertyType);
                    intellectTypeProcessor = ThriftTypeProcessorMapping.Instance.GetProcessor(enumType);
                    if (intellectTypeProcessor == null)
                        throw new NotSupportedException(string.Format(ExceptionMessage.EX_NOT_SUPPORTED_VALUE, property.Attribute.Id, property.Property.Name, property.Property.PropertyType));
                    //添加热缓存
                    IThriftTypeProcessor processor = intellectTypeProcessor;
                    property.CacheProcess = processor.Process;
                    property.CacheProcess(proxy, property.Attribute, property, obj, false, false);
                    property.HasCacheFinished = true;
                    continue;
                }

                #endregion

                #region 可空类型判断

                Type innerType;
                if ((innerType = Nullable.GetUnderlyingType(property.Property.PropertyType)) != null)
                {
                    intellectTypeProcessor = ThriftTypeProcessorMapping.Instance.GetProcessor(innerType);
                    if (intellectTypeProcessor != null)
                    {
                        //添加热缓存
                        IThriftTypeProcessor processor = intellectTypeProcessor;
                        property.CacheProcess = delegate(IMemorySegmentProxy innerProxy, ThriftPropertyAttribute innerAttribute, ToBytesAnalyseResult innerAnalyseResult, object innerTarget, bool innerIsArrayElement, bool innerNullable)
                        {
                            processor.Process(innerProxy, innerAttribute, innerAnalyseResult, innerTarget, innerIsArrayElement, true);
                        };
                        property.CacheProcess(proxy, property.Attribute, property, obj, false, true);
                        property.HasCacheFinished = true;
                        continue;
                    }
                    throw new NotSupportedException(string.Format(ExceptionMessage.EX_NOT_SUPPORTED_VALUE, property.Attribute.Id, property.Property.Name, property.Property.PropertyType));
                }

                #endregion

                #region Thrift Object类型判断

                if (property.Property.PropertyType.GetInterface(Consts.ThriftObjectFullName) != null)
                {
                    //添加热缓存
                    property.CacheProcess = delegate(IMemorySegmentProxy innerProxy, ThriftPropertyAttribute innerAttribute, ToBytesAnalyseResult innerAnalyseResult, object innerTarget, bool innerIsArrayElement, bool innerNullable)
                    {
                        IThriftObject innerIntellectObj = innerAnalyseResult.GetValue<IThriftObject>(innerTarget);
                        if (innerIntellectObj == null) return;
                        innerProxy.WriteByte((byte)innerAttribute.Id);
                        MemoryPosition startPos = innerProxy.GetPosition();
                        innerProxy.Skip(4);
                        ToBytes(innerIntellectObj, innerProxy);
                        MemoryPosition endPos = innerProxy.GetPosition();
                        innerProxy.WriteBackInt32(startPos, MemoryPosition.CalcLength(innerProxy.SegmentCount, startPos, endPos) - 4);
                    };
                    property.CacheProcess(proxy, property.Attribute, property, obj, false, false);
                    property.HasCacheFinished = true;
                    continue;
                }

                #endregion

                #region 数组的判断

                if (property.Property.PropertyType.IsArray)
                {
                    if (!property.Property.PropertyType.HasElementType)
                        throw new NotSupportedException(string.Format(ExceptionMessage.EX_NOT_SUPPORTED_VALUE, property.Attribute.Id, property.Property.Name, property.Property.PropertyType));
                    Type elementType = property.Property.PropertyType.GetElementType();
                    VT vt = FixedTypeManager.IsVT(elementType);
                    //special optimize.
                    IThriftTypeProcessor arrayProcessor = ArrayTypeProcessorMapping.Instance.GetProcessor(property.Property.PropertyType);
                    if (arrayProcessor != null) property.CacheProcess = arrayProcessor.Process;
                    //is VT, but cannot find special processor.
                    else if (vt != null)
                        throw new NotSupportedException(string.Format(ExceptionMessage.EX_NOT_SUPPORTED_VALUE, property.Attribute.Id, property.Property.Name, property.Property.PropertyType));
                    else if (elementType.IsSubclassOf(typeof(ThriftObject)))
                    {
                        //Add hot cache.
                        property.CacheProcess = delegate(IMemorySegmentProxy innerProxy, ThriftPropertyAttribute innerAttribute, ToBytesAnalyseResult innerAnalyseResult, object innerTarget, bool innerIsArrayElement, bool innerNullable)
                        {
                            IThriftObject[] array = innerAnalyseResult.GetValue<IThriftObject[]>(innerTarget);
                            if (array == null)
                            {
                                if (innerAttribute.Optional) return;
                                throw new PropertyNullValueException(string.Format(ExceptionMessage.EX_PROPERTY_VALUE, innerAttribute.Id, innerAnalyseResult.Property.Name, innerAnalyseResult.Property.PropertyType));
                            }
                            //property type
                            innerProxy.WriteSByte((sbyte)innerAttribute.PropertyType);
                            innerProxy.WriteInt16(((short)innerAttribute.Id).ToBigEndian());
                            //element type(1) + array length(4)
                            innerProxy.WriteSByte((sbyte)PropertyTypes.Struct);
                            innerProxy.WriteInt32(array.Length.ToBigEndian());
                            for (int i = 0; i < array.Length; i++)
                            {
                                IThriftObject element = array[i];
                                ToBytes(element, innerProxy);
                                //end flag of Thrift protocol object.
								innerProxy.WriteSByte(0x00);
                            }
                        };
                    }
                    else
                    {
                        intellectTypeProcessor = ThriftTypeProcessorMapping.Instance.GetProcessor(elementType);
                        if (intellectTypeProcessor == null) throw new NotSupportedException(string.Format(ExceptionMessage.EX_NOT_SUPPORTED_VALUE, property.Attribute.Id, property.Property.Name, elementType));
                        //Add hot cache.
                        IThriftTypeProcessor processor = intellectTypeProcessor;
                        property.CacheProcess = delegate(IMemorySegmentProxy innerProxy, ThriftPropertyAttribute innerAttribute, ToBytesAnalyseResult innerAnalyseResult, object innerTarget, bool innerIsArrayElement, bool innerNullable)
                        {
                            Array array = innerAnalyseResult.GetValue<Array>(innerTarget);
                            if (array == null)
                            {
                                if (innerAttribute.Optional) return;
                                throw new PropertyNullValueException(string.Format(ExceptionMessage.EX_PROPERTY_VALUE, innerAttribute.Id, innerAnalyseResult.Property.Name, innerAnalyseResult.Property.PropertyType));
                            }
                            //id(1) + total length(4) + rank(4)
                            innerProxy.WriteByte((byte)innerAttribute.Id);
                            MemoryPosition startPosition = innerProxy.GetPosition();
                            innerProxy.Skip(4);
                            innerProxy.WriteInt32(array.Length);
                            for (int i = 0; i < array.Length; i++)
                            {
                                object element = array.GetValue(i);
                                if (element == null) innerProxy.WriteUInt16(0);
                                else
                                {
                                    MemoryPosition innerStartObjPosition = innerProxy.GetPosition();
                                    innerProxy.Skip(Size.UInt16);
                                    processor.Process(innerProxy, innerAttribute, innerAnalyseResult, element, true);
                                    MemoryPosition innerEndObjPosition = innerProxy.GetPosition();
                                    innerProxy.WriteBackUInt16(innerStartObjPosition, (ushort)(MemoryPosition.CalcLength(innerProxy.SegmentCount, innerStartObjPosition, innerEndObjPosition) - 2));
                                }
                            }
                            MemoryPosition endPosition = innerProxy.GetPosition();
                            innerProxy.WriteBackInt32(startPosition, MemoryPosition.CalcLength(innerProxy.SegmentCount, startPosition, endPosition) - 4);
                        };
                    }
                    property.CacheProcess(proxy, property.Attribute, property, obj, false, false);
                    property.HasCacheFinished = true;
                    continue;
                }

                #endregion

                throw new NotSupportedException(string.Format(ExceptionMessage.EX_NOT_SUPPORTED_VALUE, property.Attribute.Id, property.Property.Name, property.Property.PropertyType));
            }
        }

        /// <summary>
        ///     将一个元数据转换为特定类型的对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="container">网络数据容器</param>
        /// <param name="value">如果解析成功, 则此字段为解析成功后的值</param>
        /// <returns>返回一个解析后的状态</returns>
        /// <exception cref="ArgumentNullException">参数不能为空</exception>
        /// <exception cref="Exception">转换失败</exception>
        public static GetObjectResultTypes TryGetObject<T>(INetworkDataContainer container, out T value)
        {
            return TryGetObject(typeof(T), container, out value);
        }

        /// <summary>
        ///     将一个元数据转换为特定类型的对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="target">特定的对象</param>
        /// <param name="container">网络数据容器</param>
        /// <param name="value">如果解析成功, 则此字段为解析成功后的值</param>
        /// <param name="isInnerObject">
        ///     是否为内部对象
        ///     <para>* 如果此值被设置为false(默认值), 则会对当前类型尝试解析MessageIdentity字段</para>
        /// </param>
        /// <returns>返回一个解析后的状态</returns>
        /// <exception cref="ArgumentNullException">参数不能为空</exception>
        /// <exception cref="Exception">转换失败</exception>
        public static GetObjectResultTypes TryGetObject<T>(Type target, INetworkDataContainer container, out T value, bool isInnerObject = false)
        {
            value = default (T);
            if (target == null) throw new ArgumentNullException("target");
            if (container == null) throw new ArgumentNullException("container");
            Dictionary<short, GetObjectAnalyseResult> result = Analyser.GetObjectAnalyser.Analyse(target);
            if (result == null) return GetObjectResultTypes.UnknownObjectType;
            GetObjectResultTypes tmpFieldParsingResult;
            //create instance for new obj.
            Object instance = Activator.CreateInstance(target);
            ThriftObject thriftObject = instance as ThriftObject;
            if (thriftObject == null) throw new Exception("Cannot convert target object to Intellect Object! #type: " + target.FullName);

            #region Data Parsing.

            GetObjectAnalyseResult analyzeResult;
            if (!isInnerObject)
            {
                #region Message Identity.

                if (!result.TryGetValue(-1, out analyzeResult)) throw new Exception("#Lost Thrift protocol object processor which it can handles type of MessageIdentity");
                if (analyzeResult.HasCacheFinished)
                {
                    if ((tmpFieldParsingResult = analyzeResult.CacheProcess(instance, analyzeResult, container)) != GetObjectResultTypes.Succeed)
                    {
                        container.ResetOffset();
                        return tmpFieldParsingResult;
                    }
                }
                else
                {
                    IThriftTypeProcessor processor = ThriftTypeProcessorMapping.Instance.GetProcessor(analyzeResult.Property.PropertyType);
                    if (processor == null) throw new Exception("#Lost Thrift protocol object processor which it can handles type of MessageIdentity");
                    analyzeResult.CacheProcess = processor.Process;
                    analyzeResult.HasEnoughData = processor.HasEnoughData;
                    analyzeResult.HasCacheFinished = true;
                    if ((tmpFieldParsingResult = analyzeResult.CacheProcess(instance, analyzeResult, container)) != GetObjectResultTypes.Succeed)
                    {
                        container.ResetOffset();
                        return tmpFieldParsingResult;
                    }
                }

                #endregion
            }
            
            while (true)
            {
                byte pType;
                //get property type.
                if (!container.TryReadByte(out pType))
                {
                    container.ResetOffset();
                    return GetObjectResultTypes.NotEnoughData;
                }
                PropertyTypes propertyType = (PropertyTypes)pType;
                //succeed parsing binary data to a Thrift protocol object by finding Thrift protocol's STOP flag. 
                if (propertyType == PropertyTypes.Stop)
                {
                    value = (T)instance;
                    return GetObjectResultTypes.Succeed;
                }
                //get id.
                short id;
                if (!container.TryReadInt16(out id))
                {
                    container.ResetOffset();
                    return GetObjectResultTypes.NotEnoughData;
                }
                id = id.ToLittleEndian();
                //get analyze result.
                if (!result.TryGetValue(id, out analyzeResult)) throw new Exception(string.Format("Illegal data contract, non-exists id! #id: {0} \r\n#Formatted MSG info: {1}, data: \r\n{2}", id, instance, container.Dump()));
                //set current property value to the target object.
                if ((tmpFieldParsingResult = InstanceHelper.SetInstance(thriftObject, analyzeResult, container)) != GetObjectResultTypes.Succeed)
                {
                    container.ResetOffset();
                    return tmpFieldParsingResult;
                }
            }

            #endregion
        }

        #endregion
    }
}