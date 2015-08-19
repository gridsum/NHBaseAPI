using System.Text;
using Gridsum.NHBaseThrift.Analyzing;
using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Helpers;
using Gridsum.NHBaseThrift.Network;
using Gridsum.NHBaseThrift.Objects;
using Gridsum.NHBaseThrift.Proxies;

namespace Gridsum.NHBaseThrift.TypeProcessors
{
    /// <summary>
    ///     Thrift网络消息唯一标识类型处理器，提供了相关的基本操作
    /// </summary>
    public class MessageIdentityTypeProcessor : ThriftTypeProcessor
    {
        #region Constructor.

        /// <summary>
        ///     String类型Thrift协议字段处理器，提供了相关的基本操作
        /// </summary>
        public MessageIdentityTypeProcessor()
        {
            _supportedType = typeof(MessageIdentity);
            _expectedDataSize = -1;
        }

        #endregion

        #region Overrides of IntellectTypeProcessor

        /// <summary>
        ///     从第三方客户数据转换为元数据
        /// </summary>
        /// <param name="proxy">内存片段代理器</param>
        /// <param name="attribute">字段属性</param>
        /// <param name="analyseResult">分析结果</param>
        /// <param name="target">目标对象实例</param>
        /// <param name="isArrayElement">当前写入的值是否为数组元素标示</param>
        public override void Process(IMemorySegmentProxy proxy, ThriftPropertyAttribute attribute, ToBytesAnalyseResult analyseResult, object target, bool isArrayElement = false, bool isNullable = false)
        {
            MessageIdentity value = analyseResult.GetValue<MessageIdentity>(target);
            byte[] data = Encoding.UTF8.GetBytes(value.Command);
            proxy.WriteInt32(value.Version.ToBigEndian());
            proxy.WriteInt32(((int)value.CommandLength).ToBigEndian());
            proxy.WriteMemory(data, 0, (uint)data.Length);
            proxy.WriteInt32(((int)value.SequenceId).ToBigEndian());
        }

        /// <summary>
        ///     从元数据转换为第三方客户数据
        /// </summary>
        /// <param name="instance">目标对象</param>
        /// <param name="result">分析结果</param>
        /// <param name="container">网络数据容器</param>
        public override GetObjectResultTypes Process(object instance, GetObjectAnalyseResult result, INetworkDataContainer container)
        {
            MessageIdentity identity;
            if(!container.TryReadMessageIdentity(out identity)) return GetObjectResultTypes.NotEnoughData;
            result.SetValue(instance, identity);
            return GetObjectResultTypes.Succeed;
        }

        #endregion
    }
}