using System;
using Gridsum.NHBaseThrift.Analyzing;
using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Enums;
using Gridsum.NHBaseThrift.Helpers;
using Gridsum.NHBaseThrift.Network;
using Gridsum.NHBaseThrift.Objects;

namespace Gridsum.NHBaseThrift.TypeProcessors
{
	/// <summary>
	///     TScan Class Thrift protocal field processor, providing basic control
	/// </summary>
	public class TScanThriftTypeProcessor : ThriftTypeProcessor
	{
		#region Constructor.

		/// <summary>
		///     TScan Class Thrift protocal field processor, providing basic control
		/// </summary>
		public TScanThriftTypeProcessor()
		{
			_supportedType = typeof(TScan);
			_expectedDataSize = -1;
		}

		#endregion

		#region Methods.

		public override void Process(Proxies.IMemorySegmentProxy proxy, ThriftPropertyAttribute attribute, ToBytesAnalyseResult analyseResult, object target, bool isArrayElement = false, bool isNullable = false)
		{
			TScan value = analyseResult.GetValue<TScan>(target);
			if (value == null) return;
			proxy.WriteSByte((sbyte)attribute.PropertyType);
			proxy.WriteInt16(attribute.Id.ToBigEndian());
			value.Bind();
			proxy.WriteMemory(value.Body, 0, (uint)value.Body.Length);
		}

		public override GetObjectResultTypes Process(object instance, GetObjectAnalyseResult result, INetworkDataContainer container)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
