using System;
using ZooKeeperNet;

namespace Gridsum.NHBaseThrift
{
    /// <summary>
    ///    ZooKeeper所使用的通知观察器
    /// </summary>
    public class ZooKeeperWatcher : IWatcher
    {
        #region Constructor.

        /// <summary>
        ///    ZooKeeper所使用的通知观察器
        /// </summary>
        public ZooKeeperWatcher(Action<WatchedEvent> callback)
        {
            if(callback == null) throw new ArgumentNullException("callback");
            _callback = callback;
        }

        #endregion

        #region Members.

        private readonly Action<WatchedEvent> _callback;

        #endregion

        #region Methods.

        /// <summary>
        ///    通知
        /// </summary>
        /// <param name="watchedEvent">被观察的事件</param>
        public void Process(WatchedEvent watchedEvent)
        {
            _callback(watchedEvent);
        }

        #endregion
    }
}