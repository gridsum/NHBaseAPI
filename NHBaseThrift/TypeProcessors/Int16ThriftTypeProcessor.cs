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
    ///     Int16����ThriftЭ���ֶδ��������ṩ����صĻ�������
    /// </summary>
    public class Int16ThriftTypeProcessor : ThriftTypeProcessor
    {
        #region Constructor.

        /// <summary>
        ///     Int16����ThriftЭ���ֶδ��������ṩ����صĻ�������
        /// </summary>
        public Int16ThriftTypeProcessor()
        {
            _supportedType = typeof(short);
            _expectedDataSize = 2;
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
			short value;
			if (!isNullable) value = analyseResult.GetValue<short>(target);
			else
			{
				short? nullableValue = analyseResult.GetValue<short?>(target);
				if (nullableValue == null)
				{
					if (!attribute.Optional) return;
					throw new PropertyNullValueException(
						string.Format(ExceptionMessage.EX_PROPERTY_VALUE, attribute.Id,
										analyseResult.Property.Name,
										analyseResult.Property.PropertyType));
				}
				value = (short)nullableValue;
			}
            proxy.WriteSByte((sbyte)attribute.PropertyType);
            proxy.WriteInt16(((short)attribute.Id).ToBigEndian());
            proxy.WriteInt16(value.ToBigEndian());
        }

        /// <summary>
        ///     ��Ԫ����ת��Ϊ�������ͻ�����
        /// </summary>
        /// <param name="instance">Ŀ�����</param>
        /// <param name="result">�������</param>
        /// <param name="container">������������</param>
        public override GetObjectResultTypes Process(object instance, GetObjectAnalyseResult result, INetworkDataContainer container)
        {
            short value;
            if(!container.TryReadInt16(out value)) return GetObjectResultTypes.NotEnoughData;
            result.SetValue(instance, value.ToLittleEndian());
            return GetObjectResultTypes.Succeed;
        }

        #endregion
    }
}