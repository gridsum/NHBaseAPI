using System;
using System.Net;
using Gridsum.NHBaseThrift.Messages;
using Gridsum.NHBaseThrift.Network.Agents;
using KJFramework.Net.ProtocolStacks;

namespace Gridsum.NHBaseThrift.Network
{
    /// <summary>
    ///    支持随机算法的连接容器
    /// </summary>
    /// <typeparam name="T">存放的连接类型</typeparam>
    internal class RamdomConnectionSet<T> : ConnectionSet<T>
    {
        #region Constructor.

        /// <summary>
        ///    支持随机算法的连接容器
        /// </summary>
        /// <typeparam name="T">存放的连接类型</typeparam>
        public RamdomConnectionSet(int min, int max, Tuple<IPEndPoint, IProtocolStack<ThriftMessage>, object> tuple, Func<IPEndPoint, IProtocolStack<ThriftMessage>, object, IThriftConnectionAgent> createFunc)
            : base(min, max, tuple, createFunc)
        {
        }

        #endregion

        #region Methods.

        /// <summary>
        ///    返回一个当前连接容器中的存活连接
        /// </summary>
        /// <returns></returns>
        public override IThriftConnectionAgent InnerGetConnection()
        {
            lock (_lockObj)
            {
                ushort value = (ushort) ((DateTime.Now.Ticks & 0x3FFF)%_connections.Count);
                return _connections[value];
            }
        }

        #endregion
    }
}