using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using KJFramework.EventArgs;
using KJFramework.Net.ProtocolStacks;
using KJFramework.Tracing;
using NHBaseThrift.Messages;
using NHBaseThrift.Network.Agents;

namespace NHBaseThrift.Network
{
    /// <summary>
    ///    存放连接的容器
    /// </summary>
    /// <typeparam name="T">存放的连接类型</typeparam>
    internal abstract class ConnectionSet<T>
    {
        #region Constructor.

        /// <summary>
        ///    
        /// </summary>
        /// <param name="min">当前池中每个KEY所需要承载的最小的连接数</param>
        /// <param name="max">当前池中每个KEY所需要承载的最大的连接数</param>
        /// <param name="tuple">连接容器的上下文对象信息</param>
        /// <param name="createFunc">创建一个新连接代理的函数委托</param>
        protected ConnectionSet(int min, int max, Tuple<IPEndPoint, IProtocolStack<ThriftMessage>, object> tuple, Func<IPEndPoint, IProtocolStack<ThriftMessage>, object, IThriftConnectionAgent> createFunc)
        {
            _min = min;
            _max = max;
            _tuple = tuple;
            _createFunc = createFunc;
        }

        #endregion

        #region Members.

        protected readonly int _min;
        protected readonly int _max;
        private Thread _backendThread;
        protected int _usedBackendThread = 0;
        protected readonly object _lockObj = new object();
        protected readonly Tuple<IPEndPoint, IProtocolStack<ThriftMessage>, object> _tuple;
        protected static readonly ITracing _tracing = TracingManager.GetTracing(typeof(ConnectionSet<T>));
        protected readonly Func<IPEndPoint, IProtocolStack<ThriftMessage>, object, IThriftConnectionAgent> _createFunc;
        protected readonly IList<IThriftConnectionAgent> _connections = new List<IThriftConnectionAgent>();

        /// <summary>
        ///    获取一个值，该值表示了当前的连接容器中是否还至少有1个连接代理器是存活的
        /// </summary>
        public bool HasConnectionLive
        {
            get { return _connections.Count > 0; }
        }
        /// <summary>
        ///    获取或设置附属属性
        /// </summary>
        public string Tag { get; set; }

        #endregion

        #region Methods.

        /// <summary>
        ///    根据不同算法，获取一个当前连接容器中的存活连接
        /// </summary>
        /// <returns>返回一个当前连接容器中的存活连接</returns>
        public IThriftConnectionAgent GetConnection()
        {
            if (!MakeSureConnections()) return null;
            return InnerGetConnection();
        }

        /// <summary>
        ///    根据不同算法，获取一个当前连接容器中的存活连接
        /// </summary>
        /// <returns>返回一个当前连接容器中的存活连接</returns>
        public abstract IThriftConnectionAgent InnerGetConnection();

        /// <summary>
        ///    向当前的连接容器中注册一个存活的连接
        /// </summary>
        /// <param name="agent"></param>
        public virtual void Register(IThriftConnectionAgent agent)
        {
            if (agent == null) return;
            if (!agent.GetChannel().IsConnected)
            {
                AgentDisconnectedHandler(new LightSingleArgEventArgs<IThriftConnectionAgent>(agent));
                return;
            }
            agent.Disconnected += ServerAgentDisconnected;
            lock (_lockObj) _connections.Add(agent);
        }

        /// <summary>
        ///    连接容器内部方法，子类可以重写此方法。
        ///    <para>*此方法主要用于确保在使用指定算法获取容器中连接之前，连接容器内部拥有足够多的连接</para>
        ///    <para>*具体策略为优先保证在同步执行代码的情况下，当前连接容器中最少有一个存活的连接，至于剩下的连接需要异步进行创建</para>
        /// </summary>
        protected virtual bool MakeSureConnections()
        {
            if (_connections.Count >= _min || _connections.Count == 1) return true;
            lock (_lockObj)
            {
                try
                {
                    IThriftConnectionAgent agent = _createFunc(_tuple.Item1, _tuple.Item2, _tuple.Item3);
                    //如果当前时间点无法正常创建一个连接代理器，则放弃此次同步机会并改为异步进行
                    if (agent == null || !agent.GetChannel().IsConnected) return false;
                    Register(agent);
                    return true;
                }
                catch (Exception ex)
                {
                    _tracing.Error(ex, null);
                    return false;
                }
                finally
                {
                    if (_connections.Count < _min && Interlocked.CompareExchange(ref _usedBackendThread, 1, 0) == 0) CreateAgentAsync();
                }
            }
        }

        /// <summary>
        ///    使用后台线程的方式异步地创建连接代理器
        /// </summary>
        private void CreateAgentAsync()
        {
            if (_backendThread != null) return;
            _backendThread = new Thread(delegate()
            {
                int retryCount = 0;
                while (_connections.Count < _min)
                {
                    try
                    {
                        IThriftConnectionAgent agent = _createFunc(_tuple.Item1, _tuple.Item2, _tuple.Item3);
                        //如果当前时间点无法正常创建一个连接代理器，则默认行为是重试3次
                        if (agent == null || !agent.GetChannel().IsConnected)
                        {
                            if (retryCount < 3)
                            {
                                retryCount++;
                                continue;
                            }
                            Interlocked.Exchange(ref _usedBackendThread, 0);
                            _backendThread = null;
                            break;
                        }
                        //reset retry counter.
                        retryCount = 0;
                        Register(agent);
                    }
                    catch (Exception ex) { _tracing.Error(ex, null); }
                    Interlocked.Exchange(ref _usedBackendThread, 0);
                    _backendThread = null;
                }
            }) { IsBackground = true, Name = "ConnectionSet-Backend-Thread", Priority = ThreadPriority.BelowNormal };
            _backendThread.Start();
        }

        #endregion

        #region Events.

        //agent disconnect event handler.
        private void ServerAgentDisconnected(object sender, EventArgs e)
        {
            IThriftConnectionAgent agent = (IThriftConnectionAgent)sender;
            agent.Disconnected -= ServerAgentDisconnected;
            lock (_lockObj) _connections.Remove(agent);
            AgentDisconnectedHandler(new LightSingleArgEventArgs<IThriftConnectionAgent>(agent));
        }

        /// <summary>
        ///    连接代理器已断开
        /// </summary>
        public event EventHandler<LightSingleArgEventArgs<IThriftConnectionAgent>> AgentDisconnected;
        protected virtual void AgentDisconnectedHandler(LightSingleArgEventArgs<IThriftConnectionAgent> e)
        {
            EventHandler<LightSingleArgEventArgs<IThriftConnectionAgent>> handler = AgentDisconnected;
            if (handler != null) handler(this, e);
        }

        #endregion
    }
}