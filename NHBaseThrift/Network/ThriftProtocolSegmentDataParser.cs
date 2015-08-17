using System;
using System.Collections.Generic;
using KJFramework.EventArgs;
using KJFramework.Net.Channels.Events;
using KJFramework.Net.Channels.Parsers;
using KJFramework.Tracing;
using NHBaseThrift.Engine;
using NHBaseThrift.Enums;
using NHBaseThrift.Messages;
using NHBaseThrift.Objects;

namespace NHBaseThrift.Network
{
    /// <summary>
    ///    Thrift协议内存片段数据解析器
    /// </summary>
    public class ThriftProtocolSegmentDataParser : ISegmentDataParser<ThriftMessage>
    {
        #region Constructor.

        /// <summary>
        ///    Thrift协议内存片段数据解析器
        /// </summary>
        public ThriftProtocolSegmentDataParser(ThriftProtocolStack protocolStack)
        {
            _protocolStack = protocolStack;
            _container = new NetworkDataContainer();
        }

        #endregion

        #region Members.

        private INetworkDataContainer _container;
        private readonly object _lockObj = new object();
        private readonly ThriftProtocolStack _protocolStack;
        private static readonly ITracing _tracing = TracingManager.GetTracing(typeof (ThriftProtocolSegmentDataParser));

        #endregion

        #region Methods.

        /// <summary>
        ///    追加一个新的数据段
        /// </summary>
        /// <param name="args">数据段接受参数</param>
        public void Append(SegmentReceiveEventArgs args)
        {
            lock (_lockObj)
            {
                _container.AppendNetworkData(args);
                MessageIdentity identity;
                if (!_container.TryReadMessageIdentity(out identity))
                {
                    _container.ResetOffset();
                    return;
                }
                _container.ResetOffset();
                Type msgType = _protocolStack.GetMessageType(identity.Command, false);
                if (msgType == null)
                    _tracing.Error("#Type of Thrift message has no any registered .NET type. #Type: {0}, seqID={1}, data: \r\n{2}", identity.Command, identity.SequenceId, _container.Dump());
                else
                {
	                try
					{
						ThriftMessage msg;
						if (ThriftObjectEngine.TryGetObject(msgType, _container, out msg) != GetObjectResultTypes.Succeed) return;
						_container.UpdateOffset();
						OnParseSucceed(new LightSingleArgEventArgs<List<ThriftMessage>>(new List<ThriftMessage> { msg }));
	                }
	                catch (Exception ex)
	                {
						_tracing.Error(ex, null);
	                }
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            _container.Dispose();
            _container = null;
        }

        #endregion

        #region Events.

        /// <summary>
        ///    解析成功事件
        /// </summary>
        public event EventHandler<LightSingleArgEventArgs<List<ThriftMessage>>> ParseSucceed;
        protected virtual void OnParseSucceed(LightSingleArgEventArgs<List<ThriftMessage>> e)
        {
            EventHandler<LightSingleArgEventArgs<List<ThriftMessage>>> handler = ParseSucceed;
            if (handler != null) handler(this, e);
        }

        #endregion
    }
}