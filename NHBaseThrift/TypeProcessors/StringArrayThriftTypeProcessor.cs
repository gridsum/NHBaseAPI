using System.Text;
using Gridsum.NHBaseThrift.Analyzing;
using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Helpers;
using Gridsum.NHBaseThrift.Network;
using Gridsum.NHBaseThrift.Proxies;

namespace Gridsum.NHBaseThrift.TypeProcessors
{
    /// <summary>
    ///     String数组类型Thrift协议字段处理器，提供了相关的基本操作
    /// </summary>
    public class StringArrayThriftTypeProcessor : ThriftTypeProcessor
    {
        #region Constructor.

        /// <summary>
        ///     String数组类型Thrift协议字段处理器，提供了相关的基本操作
        /// </summary>
        public StringArrayThriftTypeProcessor()
        {
            _supportedType = typeof(string[]);
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
            string[] value = analyseResult.GetValue<string[]>(target);
            proxy.WriteSByte((sbyte)attribute.PropertyType);
            proxy.WriteInt16(attribute.Id.ToBigEndian());

            proxy.WriteByte((byte)PropertyTypes.String);
            proxy.WriteInt32(value.Length.ToBigEndian());
            foreach (string data in value)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                proxy.WriteInt32(bytes.Length.ToBigEndian());
                proxy.WriteMemory(bytes, 0, (uint)data.Length);
            }
        }

        /// <summary>
        ///     从元数据转换为第三方客户数据
        /// </summary>
        /// <param name="instance">目标对象</param>
        /// <param name="result">分析结果</param>
        /// <param name="container">网络数据容器</param>
        public override GetObjectResultTypes Process(object instance, GetObjectAnalyseResult result, INetworkDataContainer container)
        {
            byte type;
            int count;
            if (!container.TryReadByte(out type)) return GetObjectResultTypes.NotEnoughData;
            if (!container.TryReadInt32(out count)) return GetObjectResultTypes.NotEnoughData;
            count = count.ToLittleEndian();
            string[] value = new string[count];
            for (int i = 0; i < count; i++)
            {
                int length;
                if (!container.TryReadInt32(out length)) return GetObjectResultTypes.NotEnoughData;
                length = length.ToLittleEndian();
                string content;
                if (!container.TryReadString(Encoding.UTF8, length, out content)) return GetObjectResultTypes.NotEnoughData;
                value[i] = content;
            }
            result.SetValue(instance, value);
            return GetObjectResultTypes.Succeed;
        }

        #endregion
    }
}