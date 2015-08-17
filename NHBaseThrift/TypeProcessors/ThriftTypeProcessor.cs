using System;
using NHBaseThrift.Analyzing;
using NHBaseThrift.Attributes;
using NHBaseThrift.Enums;
using NHBaseThrift.Network;
using NHBaseThrift.Proxies;

namespace NHBaseThrift.TypeProcessors
{
    /// <summary>
    ///     Thrift协议类型处理器抽象父类，提供了相关的基本操作。
    /// </summary>
    public abstract class ThriftTypeProcessor : IThriftTypeProcessor
    {
        #region Constructor.

        /// <summary>
        ///     Thrift协议类型处理器抽象父类，提供了相关的基本操作。
        /// </summary>
        protected ThriftTypeProcessor()
        {
            //support this act by default.
            _supportUnmanagement = true;
        }

        #endregion

        #region Members.

        protected Type _supportedType;
        protected int _expectedDataSize;
        protected bool _supportUnmanagement;

        /// <summary>
        ///     获取一个值，该值标示了当前处理器是否支持以非托管的方式进行执行
        /// </summary>
        public bool SupportUnmanagement
        {
            get { return _supportUnmanagement; }
        }

        /// <summary>
        ///     获取支持的类型
        /// </summary>
        public Type SupportedType
        {
            get { return _supportedType; }
        }

        /// <summary>
        ///    获取一个值，该值标示了当从原始byte数组解析成为此类型时所期望传入的最小可用数据长度
        ///    <para>* 如果是动态的则设置 -1即可</para>
        /// </summary>
        public int ExpectedDataSize
        {
            get { return _expectedDataSize; }
        }

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
        public abstract void Process(IMemorySegmentProxy proxy, ThriftPropertyAttribute attribute, ToBytesAnalyseResult analyseResult, object target, bool isArrayElement = false, bool isNullable = false);
        /// <summary>
        ///     从元数据转换为第三方客户数据
        /// </summary>
        /// <param name="instance">目标对象</param>
        /// <param name="result">分析结果</param>
        /// <param name="container">网络数据容器</param>
        public abstract GetObjectResultTypes Process(object instance, GetObjectAnalyseResult result, INetworkDataContainer container);
        /// <summary>
        ///    尝试检查一下当前所需要解析的数据可用长度是否满足此类型的解析需求
        ///    <para>* 此方法只有当ExpectedDataSize = -1时才会被调用</para>
        /// </summary>
        /// <param name="container">网络数据容器</param>
        /// <returns></returns>
        public virtual bool HasEnoughData(INetworkDataContainer container)
        {
            return true;
        }

        #endregion
    }
}