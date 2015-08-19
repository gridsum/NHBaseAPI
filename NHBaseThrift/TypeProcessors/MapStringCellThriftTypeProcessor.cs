using System.Collections.Generic;
using System.Text;
using Gridsum.NHBaseThrift.Analyzing;
using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Engine;
using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Helpers;
using Gridsum.NHBaseThrift.Network;
using Gridsum.NHBaseThrift.Objects;
using Gridsum.NHBaseThrift.Proxies;

namespace Gridsum.NHBaseThrift.TypeProcessors
{
    /// <summary>
    ///     Dictionary(string, Cell)类型Thrift协议字段处理器，提供了相关的基本操作
    /// </summary>
    class MapStringCellThriftTypeProcessor : ThriftTypeProcessor
    {
        #region Constructor.

        /// <summary>
        ///     Dictionary(string, Cell)类型Thrift协议字段处理器，提供了相关的基本操作
        /// </summary>
        public MapStringCellThriftTypeProcessor()
        {
            _supportedType = typeof(Dictionary<string, Cell>);
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
        /// <param name="isNullable">是否为可空字段标示</param>
        public override void Process(IMemorySegmentProxy proxy, ThriftPropertyAttribute attribute, ToBytesAnalyseResult analyseResult, object target, bool isArrayElement = false, bool isNullable = false)
        {
            Dictionary<string, Cell> value = analyseResult.GetValue<Dictionary<string, Cell>>(target);
            if (value == null) return;
            proxy.WriteSByte((sbyte)attribute.PropertyType);
            proxy.WriteInt16(attribute.Id.ToBigEndian());
            proxy.WriteByte((byte)PropertyTypes.String);
            proxy.WriteByte((byte)PropertyTypes.Struct);
            proxy.WriteInt32((value.Count).ToBigEndian());
            foreach (KeyValuePair<string, Cell> pair in value)
            {
                byte[] data = Encoding.UTF8.GetBytes(pair.Key);
                proxy.WriteInt32(data.Length.ToBigEndian());
                proxy.WriteMemory(data, 0, (uint)data.Length);

                pair.Value.Bind();
                data = pair.Value.Body;
                proxy.WriteMemory(data, 0, (uint)data.Length);
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
            byte keyType, valueType;
            int count;
            if (!container.TryReadByte(out keyType)) return GetObjectResultTypes.NotEnoughData;
            if (!container.TryReadByte(out valueType)) return GetObjectResultTypes.NotEnoughData;
            if (!container.TryReadInt32(out count)) return GetObjectResultTypes.NotEnoughData;
            count = count.ToLittleEndian();
            Dictionary<string, Cell> value = new Dictionary<string, Cell>(count);
            for (int i = 0; i < count; i++)
            {
                int stringCount;
                if (!container.TryReadInt32(out stringCount)) return GetObjectResultTypes.NotEnoughData;
                stringCount = stringCount.ToLittleEndian();
                string keyContent;
                if (!container.TryReadString(Encoding.UTF8, stringCount, out keyContent)) return GetObjectResultTypes.NotEnoughData;

                Cell valueContent;
                GetObjectResultTypes type = ThriftObjectEngine.TryGetObject(typeof(Cell), container, out valueContent, true);
                if (type != GetObjectResultTypes.Succeed) return type;
                value.Add(keyContent, valueContent);
            }
            result.SetValue(instance, value);
            return GetObjectResultTypes.Succeed;
        }

        #endregion
    }
}
