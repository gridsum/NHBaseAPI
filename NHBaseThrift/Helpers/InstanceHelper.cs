using System;
using Gridsum.NHBaseThrift.Analyzing;
using Gridsum.NHBaseThrift.Contracts;
using Gridsum.NHBaseThrift.Engine;
using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Exceptions;
using Gridsum.NHBaseThrift.Network;
using Gridsum.NHBaseThrift.TypeProcessors;
using Gridsum.NHBaseThrift.TypeProcessors.Maps;

namespace Gridsum.NHBaseThrift.Helpers
{
    /// <summary>
    ///     实例帮助器
    /// </summary>
    internal static class InstanceHelper
    {
        #region Methods

        /// <summary>
        ///     设置字段实例
        /// </summary>
        /// <param name="instance">对象实例</param>
        /// <param name="analyseResult">字段临时解析结构</param>
        /// <param name="container">网络数据容器</param>
        public static GetObjectResultTypes SetInstance(object instance, GetObjectAnalyseResult analyseResult, INetworkDataContainer container)
        {
            GetObjectAnalyseResult analyze = analyseResult;

            //热处理判断
            if (analyze.HasCacheFinished) return analyze.CacheProcess(instance, analyseResult, container);

            #region 普通类型判断

            IThriftTypeProcessor intellectTypeProcessor = ThriftTypeProcessorMapping.Instance.GetProcessor(analyze.Property.PropertyType);
            if (intellectTypeProcessor != null)
            {
                //添加热缓存
                IThriftTypeProcessor processor = intellectTypeProcessor;
                analyze.CacheProcess = processor.Process;
                analyze.HasEnoughData = processor.HasEnoughData;
                analyze.HasCacheFinished = true;
                return analyze.CacheProcess(instance, analyseResult, container);
            }

            #endregion

            #region 枚举类型判断

            //枚举类型
            if (analyze.Property.PropertyType.IsEnum)
            {
                Type enumType = Enum.GetUnderlyingType(analyze.Property.PropertyType);
                intellectTypeProcessor = ThriftTypeProcessorMapping.Instance.GetProcessor(enumType);
                if (intellectTypeProcessor == null) throw new Exception("Cannot support this enum type! #type: " + analyze.Property.PropertyType);
                //添加热处理
                IThriftTypeProcessor processor = intellectTypeProcessor;
                analyze.CacheProcess = processor.Process;
                analyze.HasEnoughData = processor.HasEnoughData;
                analyze.HasCacheFinished = true;
                return analyze.CacheProcess(instance, analyseResult, container);
            }

            #endregion

            #region 可空类型判断

            Type innerType;
            if ((innerType = Nullable.GetUnderlyingType(analyze.Property.PropertyType)) != null)
            {
                intellectTypeProcessor = ThriftTypeProcessorMapping.Instance.GetProcessor(innerType);
                if (intellectTypeProcessor != null)
                {
                    //添加热缓存
                    IThriftTypeProcessor processor = intellectTypeProcessor;
                    analyze.CacheProcess = processor.Process;
                    analyze.HasEnoughData = processor.HasEnoughData;
                    analyze.HasCacheFinished = true;
                    return analyze.CacheProcess(instance, analyseResult, container);
                }
                throw new Exception("Cannot find compatible processor, #type: " + analyze.Property.PropertyType);
            }

            #endregion

            #region Thrift类型的判断

            //Thrift对象的判断
            if (analyze.Property.PropertyType.IsClass && analyze.Property.PropertyType.GetInterface(Consts.ThriftObjectFullName) != null)
            {
                //添加热缓存
                analyze.CacheProcess = delegate(object innerInstance, GetObjectAnalyseResult innerAnalyseResult, INetworkDataContainer innerContainer)
                {
                    GetObjectResultTypes r;
                    ThriftObject tobj;
                    if ((r = ThriftObjectEngine.TryGetObject(innerAnalyseResult.Property.PropertyType, innerContainer, out tobj, true)) != GetObjectResultTypes.Succeed) return r;
                    innerAnalyseResult.SetValue(innerInstance, tobj);
                    return GetObjectResultTypes.Succeed;
                };
                analyze.HasCacheFinished = true;
                return analyze.CacheProcess(instance, analyseResult, container); ;
            }

            #endregion

            #region 数组的判断

            if (analyze.Property.PropertyType.IsArray)
            {
                Type elementType = analyze.Property.PropertyType.GetElementType();
                intellectTypeProcessor = ArrayTypeProcessorMapping.Instance.GetProcessor(analyze.Property.PropertyType);
                if (intellectTypeProcessor != null)
                {
                    //添加热缓存
                    IThriftTypeProcessor processor = intellectTypeProcessor;
                    analyze.HasEnoughData = processor.HasEnoughData;
                    analyze.CacheProcess = processor.Process;
                    analyze.HasCacheFinished = true;
                    return analyze.CacheProcess(instance, analyseResult, container);
                }
                if (elementType.IsSubclassOf(typeof (ThriftObject)))
                {
                    #region IntellectObject type array processor.

                    //add HOT cache.
                    analyze.CacheProcess = delegate(object innerInstance, GetObjectAnalyseResult innerAnalyseResult, INetworkDataContainer innerContainer)
                    {
                        if(!innerContainer.CheckEnoughSize(5)) return GetObjectResultTypes.NotEnoughData;
                        byte tmpData;
                        int arrLen;
                        if (!innerContainer.TryReadByte(out tmpData)) return GetObjectResultTypes.NotEnoughData;
                        PropertyTypes arrElementType = (PropertyTypes)tmpData;
                        if (!innerContainer.TryReadInt32(out arrLen)) return GetObjectResultTypes.NotEnoughData;
                        arrLen = arrLen.ToLittleEndian();
                        Func<int, ThriftObject[]> func = ThriftObjectArrayHelper.GetFunc<ThriftObject>(analyze.Property.PropertyType);
                        ThriftObject[] array = func(arrLen);
                        for (int i = 0; i < arrLen; i++)
                        {
                            GetObjectResultTypes r;
                            ThriftObject tobj;
                            if ((r = ThriftObjectEngine.TryGetObject(innerAnalyseResult.Property.PropertyType.GetElementType(), innerContainer, out tobj, true)) != GetObjectResultTypes.Succeed) return r;
                            array[i] = tobj;
                        }
                        innerAnalyseResult.SetValue(innerInstance, array);
                        return GetObjectResultTypes.Succeed;
                    };

                    #endregion
                }
                else throw new NotSupportedException(string.Format(ExceptionMessage.EX_NOT_SUPPORTED_VALUE, analyseResult.Attribute.Id, analyseResult.Property.Name, analyseResult.Property.PropertyType));
                analyze.HasCacheFinished = true;
                return analyze.CacheProcess(instance, analyseResult, container);
            }

            #endregion

            throw new Exception("Cannot support this data type: " + analyze.Property.PropertyType);
        }

        #endregion
    }
}