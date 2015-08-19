using System;
using System.Net;
using Gridsum.NHBaseThrift.Messages;
using Gridsum.NHBaseThrift.Network.Agents;
using KJFramework.Net.ProtocolStacks;

namespace Gridsum.NHBaseThrift.Network
{
    /// <summary>
    ///    支持顺序算法的连接容器
    /// </summary>
    /// <typeparam name="T">存放的连接类型</typeparam>
    internal class SequentialConnectionSet<T> : ConnectionSet<T>
    {
        #region Constructor.

        /// <summary>
        ///    支持顺序算法的连接容器
        /// </summary>
        /// <typeparam name="T">存放的连接类型</typeparam>
        public SequentialConnectionSet(int min, int max, Tuple<IPEndPoint, IProtocolStack<ThriftMessage>, object> tuple, Func<IPEndPoint, IProtocolStack<ThriftMessage>, object, IThriftConnectionAgent> createFunc)
            : base(min, max, tuple, createFunc)
        {
        }

        #endregion

        #region Members.

        private int _sequenceIndex;

        #endregion

        /// <summary>
        ///    根据不同算法，获取一个当前连接容器中的存活连接
        /// </summary>
        /// <returns>返回一个当前连接容器中的存活连接</returns>
        public override IThriftConnectionAgent InnerGetConnection()
        {
            lock (_lockObj)
            {
                if (_sequenceIndex < _connections.Count) return _connections[_sequenceIndex++];
                if (_connections.Count == 0) return null;
                return _connections[(_sequenceIndex = 0)];
            }
        }
    }
}