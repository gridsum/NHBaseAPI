using System;

namespace Gridsum.NHBaseThrift.Network.Agents
{
    /// <summary>
    ///     连接代理器，提供了相关的基本操作
    /// </summary>
    public interface IConnectionAgent
    {
        /// <summary>
        ///     获取或设置附属属性
        /// </summary>
        Object Tag { get; set; }
        /// <summary>
        ///     主动关闭连接代理器
        ///     * 主动关闭的行为将会关闭内部的通信信道连接
        /// </summary>
        void Close();
        /// <summary>
        ///     断开事件
        /// </summary>
        event EventHandler Disconnected;
    }
}