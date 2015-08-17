using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Timers;
using KJFramework.EventArgs;
using KJFramework.Net.Channels;
using KJFramework.Net.Transaction;
using KJFramework.Net.Transaction.Comparers;
using KJFramework.Net.Transaction.Helpers;
using KJFramework.Net.Transaction.Identities;
using KJFramework.Tracing;
using NHBaseThrift.Messages;
using Timer = System.Timers.Timer;

namespace NHBaseThrift.Network.Transactions
{
    /// <summary>
    ///     消息事务管理器，提供了相关的基本操作
    /// </summary>
    public class ThriftMessageTransactionManager
    {
        #region Constructor

        /// <summary>
        ///     消息事务管理器，提供了相关的基本操作
        ///     * 默认时间：30s.
        /// </summary>
        /// <param name="interval">事务检查时间间隔</param>
        public ThriftMessageTransactionManager(int interval = 30000)
        {
            if (interval <= 0) throw new ArgumentException("Illegal check time interval!");
            _interval = interval;
            _transactions = new ConcurrentDictionary<TransactionIdentity, ThriftMessageTransaction>(new TransactionIdentityComparer());
            _timer = new Timer {Interval = _interval};
            _timer.Elapsed += TimerElapsed;
            _timer.Start();
        }

        #endregion

        #region Members

        protected readonly int _interval;
        protected readonly Timer _timer;
        protected readonly ConcurrentDictionary<TransactionIdentity, ThriftMessageTransaction> _transactions;
        private static readonly ITracing _tracing = TracingManager.GetTracing(typeof (ThriftMessageTransactionManager));

        #endregion

        #region Methods

        /// <summary>
        ///     创建一个新的消息事务，并将其加入到当前的事务列表中
        /// </summary>
        /// <param name="sequenceId">本次新事务的唯一编号</param>
        /// <param name="channel">消息通信信道</param>
        /// <returns>返回一个新的消息事务</returns>
        /// <exception cref="ArgumentNullException">通信信道不能为空</exception>
        public ThriftMessageTransaction Create(int sequenceId, IMessageTransportChannel<ThriftMessage> channel)
        {
            if (channel == null) throw new ArgumentNullException("channel");
            ThriftMessageTransaction transaction = new ThriftMessageTransaction(new Lease(DateTime.MaxValue), channel) { TransactionManager = this, SequenceId = sequenceId };
            TransactionIdentity identity = IdentityHelper.Create((IPEndPoint) channel.LocalEndPoint, sequenceId, false);
            transaction.Identity = identity;
            return (Add(transaction) ? transaction : null);
        }

        private bool Add(ThriftMessageTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            ThriftMessageTransaction temp;
            if (_transactions.TryGetValue(transaction.Identity, out temp))
            {
                _tracing.Error("#Cannot add ThriftMessageTransaction to current T-Manager, because the target identity has been dup. \r\nDetails below:\r\nIdentity: {0}", transaction.SequenceId);
                return false;
            }
            if (!_transactions.TryAdd(transaction.Identity, transaction))
            {
                _tracing.Error("#Add ThriftMessageTransaction to current T-Manager failed. #SequenceId: " + transaction.SequenceId);
                return false;
            }
            return true;
        }

        /// <summary>
        ///     激活一个事务，并尝试处理该事务的响应消息流程
        /// </summary>
        /// <param name="identity">网络事务唯一标识</param>
        /// <param name="response">应答消息</param>
        public void Active(TransactionIdentity identity, ThriftMessage response)
        {
            ThriftMessageTransaction transaction;
            if (!_transactions.TryRemove(identity, out transaction)) return;
            transaction.SetResponse(response);
        }

        /// <summary>
        ///    移除一个不需要管理的事务
        /// </summary>
        /// <param name="key">事务唯一键值</param>
        public virtual void Remove(TransactionIdentity key)
        {
            ThriftMessageTransaction v;
            _transactions.TryRemove(key, out v);
        }

        #endregion

        #region Events

        /// <summary>
        ///    事务过期事件
        /// </summary>
        public event EventHandler<LightSingleArgEventArgs<ThriftMessageTransaction>> TransactionExpired;
        protected virtual void TransactionExpiredHandler(LightSingleArgEventArgs<ThriftMessageTransaction> e)
        {
            EventHandler<LightSingleArgEventArgs<ThriftMessageTransaction>> handler = TransactionExpired;
            if (handler != null) handler(this, e);
        }

        protected void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_transactions.Count == 0) return;
            IList<TransactionIdentity> expireValues = new List<TransactionIdentity>();
            //check dead flag for transaction.
            foreach (KeyValuePair<TransactionIdentity, ThriftMessageTransaction> pair in _transactions)
                if (pair.Value.GetLease().IsDead) expireValues.Add(pair.Key);
            if (expireValues.Count == 0) return;
            //remove expired transactions.
            foreach (TransactionIdentity expireValue in expireValues)
            {
                ThriftMessageTransaction transaction;
                if (_transactions.TryRemove(expireValue, out transaction))
                {
                    transaction.SetTimeout();
                    TransactionExpiredHandler(new LightSingleArgEventArgs<ThriftMessageTransaction>(transaction));
                }
            }
        }

        #endregion
    }
}