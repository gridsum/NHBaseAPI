using KJFramework.Cache.Cores;

namespace Gridsum.NHBaseThrift.UnitTests
{
    /// <summary>
    ///     缓存项，提供了最基础的缓存提取和存储功能
    /// </summary>
    /// <typeparam name="T">缓存类型</typeparam>
    internal class CacheItem<T> : ICacheItem<T>
    {
        #region Members

        protected T _value;

        #endregion

        #region Implementation of ICacheItem<T>

        /// <summary>
        ///     获取缓存内容
        /// </summary>
        /// <returns>返回缓存内容</returns>
        public T GetValue()
        {
            return _value;
        }

        /// <summary>
        ///     设置缓存内容
        /// </summary>
        /// <param name="obj">缓存对象</param>
        public void SetValue(T obj)
        {
            _value = obj;
        }

        #endregion
    }
}