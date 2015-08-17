using System;
using KJFramework.EventArgs;
using KJFramework.Net.Channels;
using KJFramework.Net.Transaction;
using NHBaseThrift.Messages;
using NHBaseThrift.Network.Transactions;

namespace NHBaseThrift.Network.Agents
{
    /// <summary>
    ///     客户端代理器元接口
    /// </summary>
    public interface IThriftConnectionAgent : IConnectionAgent
    {
        /// <summary>
        ///     获取消息事务管理器
        /// </summary>
        ThriftMessageTransactionManager TransactionManager { get; }
        /// <summary>
        ///     获取内部的通信信道
        /// </summary>
        /// <returns></returns>
        IMessageTransportChannel<ThriftMessage> GetChannel();
        /// <summary>
        ///     创建一个新的事务
        /// </summary>
        /// <returns>返回一个针对客户端的新事物</returns>
        ThriftMessageTransaction CreateTransaction();
        /// <summary>
        ///     新的事物创建被创建时激活此事件
        /// </summary>
        event EventHandler<LightSingleArgEventArgs<IMessageTransaction<ThriftMessage>>> NewTransaction;
    }
}