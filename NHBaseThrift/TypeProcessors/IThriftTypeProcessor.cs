using System;
using Gridsum.NHBaseThrift.Analyzing;
using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Network;
using Gridsum.NHBaseThrift.Proxies;

namespace Gridsum.NHBaseThrift.TypeProcessors
{
    /// <summary>
    ///    Thrift协议中所使用类型的处理器
    /// </summary>
    public interface IThriftTypeProcessor
    {
        #region Members.
        
        /// <summary>
        ///     获取支持的类型
        /// </summary>
        Type SupportedType { get; }
        /// <summary>
        ///    获取一个值，该值标示了当从原始byte数组解析成为此类型时所期望传入的最小可用数据长度
        ///    <para>* 如果是动态的则设置 -1即可</para>
        /// </summary>
        int ExpectedDataSize { get; }

        #endregion

        #region Methods.

        /// <summary>
        ///     从第三方客户数据转换为元数据
        /// </summary>
        /// <param name="proxy">内存片段代理器</param>
        /// <param name="attribute">字段属性</param>
        /// <param name="analyseResult">分析结果</param>
        /// <param name="target">目标对象实例</param>
        /// <param name="isArrayElement">当前写入的值是否为数组元素标示</param>
        /// <param name="isNullable">是否为可空字段标示</param>
        void Process(IMemorySegmentProxy proxy, ThriftPropertyAttribute attribute, ToBytesAnalyseResult analyseResult, object target, bool isArrayElement = false, bool isNullable = false);
        /// <summary>
        ///     从元数据转换为第三方客户数据
        /// </summary>
        /// <param name="instance">目标对象</param>
        /// <param name="result">分析结果</param>
        /// <param name="container">网络数据容器</param>
        GetObjectResultTypes Process(object instance, GetObjectAnalyseResult result, INetworkDataContainer container);
        /// <summary>
        ///    尝试检查一下当前所需要解析的数据可用长度是否满足此类型的解析需求
        ///    <para>* 此方法只有当ExpectedDataSize = -1时才会被调用</para>
        /// </summary>
        /// <param name="container">网络数据容器</param>
        /// <returns></returns>
        bool HasEnoughData(INetworkDataContainer container);

        #endregion
    }
}