using Gridsum.NHBaseThrift.Analyzing;
using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Exceptions;
using Gridsum.NHBaseThrift.Helpers;
using Gridsum.NHBaseThrift.Network;
using Gridsum.NHBaseThrift.Proxies;

namespace Gridsum.NHBaseThrift.TypeProcessors
{
    /// <summary>
    ///     byte类型Thrift协议字段处理器，提供了相关的基本操作
    /// </summary>
    public class ByteThriftTypeProcessor : ThriftTypeProcessor
    {
        #region Constructor.

        /// <summary>
        ///     byte类型Thrift协议字段处理器，提供了相关的基本操作
        /// </summary>
        public ByteThriftTypeProcessor()
        {
            _supportedType = typeof(byte);
            _expectedDataSize = 1;
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
			byte value;
			if (!isNullable) value = analyseResult.GetValue<byte>(target);
			else
			{
				byte? nullableValue = analyseResult.GetValue<byte?>(target);
				if (nullableValue == null)
				{
					if (!attribute.Optional) return;
					throw new PropertyNullValueException(
						string.Format(ExceptionMessage.EX_PROPERTY_VALUE, attribute.Id,
										analyseResult.Property.Name,
										analyseResult.Property.PropertyType));
				}
				value = (byte)nullableValue;
			}
            proxy.WriteSByte((sbyte)attribute.PropertyType);
            proxy.WriteInt16(((short)attribute.Id).ToBigEndian());
            proxy.WriteByte(value);
        }

        /// <summary>
        ///     从元数据转换为第三方客户数据
        /// </summary>
        /// <param name="instance">目标对象</param>
        /// <param name="result">分析结果</param>
        /// <param name="container">网络数据容器</param>
        public override GetObjectResultTypes Process(object instance, GetObjectAnalyseResult result, INetworkDataContainer container)
        {
            byte value;
            if(!container.TryReadByte(out value)) return GetObjectResultTypes.NotEnoughData;
            result.SetValue(instance, value);
            return GetObjectResultTypes.Succeed;
        }

        #endregion
    }
}