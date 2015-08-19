using System;
using Gridsum.NHBaseThrift.Messages;
using KJFramework.Net.Channels;
using KJFramework.Net.Transaction;
using KJFramework.Tracing;

namespace Gridsum.NHBaseThrift.Network.Transactions
{
    /// <summary>
    ///     Thrift协议消息事务，提供了相关的基本操作
    /// </summary>
    public class ThriftMessageTransaction : MessageTransaction<ThriftMessage>
    {
        #region Constructor.

        /// <summary>
        ///     Thrift协议消息事务，提供了相关的基本操作
        /// </summary>
        public ThriftMessageTransaction()
        {
            CreateTime = DateTime.Now;
        }

        /// <summary>
        ///     Thrift协议消息事务，提供了相关的基本操作
        /// </summary>
        /// <param name="channel">消息通信信道</param>
        public ThriftMessageTransaction(IMessageTransportChannel<ThriftMessage> channel)
            : base(channel)
        {
            CreateTime = DateTime.Now;
        }

        /// <summary>
        ///     Thrift协议消息事务，提供了相关的基本操作
        /// </summary>
        /// <param name="lease">事务生命租期租约</param>
        /// <param name="channel">消息通信信道</param>
        public ThriftMessageTransaction(ILease lease, IMessageTransportChannel<ThriftMessage> channel)
            : base(lease, channel)
        {
            CreateTime = DateTime.Now;
        }

        #endregion

        #region Members.

        internal ThriftMessageTransactionManager TransactionManager { get; set; }
        protected static readonly ITracing _tracing = TracingManager.GetTracing(typeof(ThriftMessageTransaction));

        /// <summary>
        ///     获取或设置当前事务的唯一标示
        /// </summary>
        public int SequenceId { get; set; }

        #endregion

        #region Methods.

        internal void SetTimeout()
        {
            if (_channel != null) _channel.Disconnect();
            TimeoutHandler(null);
        }

        #endregion

        #region Overrides of MessageTransaction<BaseMessage>

        /// <summary>
        ///     发送一个请求消息
        /// </summary>
        /// <param name="message">请求消息</param>
        public override void SendRequest(ThriftMessage message)
        {
            if (message == null) return;
            message.Identity.SequenceId = (uint) SequenceId;
            if (!_channel.IsConnected)
            {
                _tracing.Error(string.Format("Cannot send a Thrift protocol REQ message to {0}, because target msg channel has been disconnected.", _channel.RemoteEndPoint));
                return;
            }
            try
            {
                //send failed.
                if (_channel.Send(message) < 0)
                {
                    _tracing.Error("#REQ message send count < 0. #Identity: {0}", message.Identity);
                    TransactionManager.Remove(Identity);
                    _channel.Disconnect();
                    FailedHandler(null);
                    return;
                }
                //calc REQ time.
                RequestTime = DateTime.Now;
                //30s
                GetLease().Change(DateTime.Now.AddSeconds(30));
                string msgInfo;
                if (!(message is InsertNewRowsRequestMessage)) msgInfo = message.ToString();
                else msgInfo = (Global.NeedLogBatchInsert ? message.ToString() : (Global.BatchInsertNotification + "\r\n" + message.Identity.SequenceId));
                _tracing.Info("L: {0}\r\nR: {1}\r\n{2}", _channel.LocalEndPoint, _channel.RemoteEndPoint, msgInfo);
            }
            catch(Exception ex)
            {
                _tracing.Error(ex, null);
                TransactionManager.Remove(Identity);
                _channel.Disconnect();
                FailedHandler(null);
            }
        }

        /// <summary>
        ///     发送一个响应消息
        /// </summary>
        /// <param name="message">响应消息</param>
        public override void SendResponse(ThriftMessage message)
        {
            message.Identity.SequenceId = (uint) SequenceId;
            TransactionManager.Remove(Identity);
            if (!_channel.IsConnected)
            {
                _tracing.Error("Cannot send a Thrift protocol RSP message to {0}, because target msg channel has been disconnected.", _channel.RemoteEndPoint);
                return;
            }
            try
            {
                int sendCount = _channel.Send(message);
                if (sendCount == 0)
                {
                    _tracing.Error("Cannot send a Thrift protocol RSP message {0} to {1}, serialized data maybe is null.", message.Identity.Command, _channel.RemoteEndPoint);
                    return;
                }
                //send failed.
                if (sendCount < 0)
                {
                    _channel.Disconnect();
                    FailedHandler(null);
                    return;
                }
                CommonCounter.Instance.RateOfClientResponse.Increment();
                //calc RSP time.
                ResponseTime = DateTime.Now;
                _tracing.Info("SendCount: {0}\r\nL: {1}\r\nR: {2}\r\n{3}", sendCount, _channel.LocalEndPoint, _channel.RemoteEndPoint, message.ToString());
            }
            catch
            {
                _channel.Disconnect();
                FailedHandler(null);
            }
        }

        #endregion
    }
}