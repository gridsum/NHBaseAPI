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
    ///     Int32����ThriftЭ���ֶδ��������ṩ����صĻ�������
    /// </summary>
    public class Int32ThriftTypeProcessor : ThriftTypeProcessor
    {
        #region Constructor.

        /// <summary>
        ///     Int32����ThriftЭ���ֶδ��������ṩ����صĻ�������
        /// </summary>
        public Int32ThriftTypeProcessor()
        {
            _supportedType = typeof(int);
            _expectedDataSize = 4;
        }

        #endregion

        #region Overrides of IntellectTypeProcessor

        /// <summary>
        ///     �ӵ������ͻ�����ת��ΪԪ����
        /// </summary>
        /// <param name="proxy">�ڴ�Ƭ�δ�����</param>
        /// <param name="attribute">�ֶ�����</param>
        /// <param name="analyseResult">�������</param>
        /// <param name="target">Ŀ�����ʵ��</param>
        /// <param name="isArrayElement">��ǰд���ֵ�Ƿ�Ϊ����Ԫ�ر�ʾ</param>
		/// <param name="isNullable">Ԫ���������Ƿ�ɿ�</param>
        public override void Process(IMemorySegmentProxy proxy, ThriftPropertyAttribute attribute, ToBytesAnalyseResult analyseResult, object target, bool isArrayElement = false, bool isNullable = false)
        {
			int value;
			if (!isNullable) value = analyseResult.GetValue<int>(target);
			else
			{
				int? nullableValue = analyseResult.GetValue<int?>(target);
				if (nullableValue == null)
				{
					if (!attribute.Optional) return;
					throw new PropertyNullValueException(
						string.Format(ExceptionMessage.EX_PROPERTY_VALUE, attribute.Id,
										analyseResult.Property.Name,
										analyseResult.Property.PropertyType));
				}
				value = (int)nullableValue;
			}
            proxy.WriteSByte((sbyte)attribute.PropertyType);
            proxy.WriteInt16(((short)attribute.Id).ToBigEndian());
            proxy.WriteInt32(value.ToBigEndian());
        }

        /// <summary>
        ///     ��Ԫ����ת��Ϊ�������ͻ�����
        /// </summary>
        /// <param name="instance">Ŀ�����</param>
        /// <param name="result">�������</param>
        /// <param name="container">������������</param>
        public override GetObjectResultTypes Process(object instance, GetObjectAnalyseResult result, INetworkDataContainer container)
        {
            int value;
            if(!container.TryReadInt32(out value)) return GetObjectResultTypes.NotEnoughData;
            result.SetValue(instance, value.ToLittleEndian());
            return GetObjectResultTypes.Succeed;
        }

        #endregion
    }
}