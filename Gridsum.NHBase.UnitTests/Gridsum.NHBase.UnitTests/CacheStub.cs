using System;
using KJFramework.Cache;
using KJFramework.Cache.Cores;

namespace Gridsum.NHBaseThrift.UnitTests
{
    /// <summary>
    ///     缓存存根，提供了相关的基本操作
    /// </summary>
    /// <typeparam name="T">缓存类型</typeparam>
    internal class CacheStub<T> : ICacheStub<T>, IReadonlyCacheStub<T>, IFixedCacheStub<T>
    {
        #region Constructor

        /// <summary>
        ///     缓存存根，提供了相关的基本操作
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        public CacheStub()
            : this(DateTime.MaxValue)
        {
            
        }

        /// <summary>
        ///     缓存存根，提供了相关的基本操作
        /// </summary>
        /// <param name="expireTime">过期时间</param>
        /// <typeparam name="T">缓存类型</typeparam>
        public CacheStub(DateTime expireTime)
        {
            _lease = new CacheLease(expireTime);
            _cache = new CacheItem<T>();
        }

        #endregion

        #region Implementation of ICacheStub<T>

        protected bool _fixed;
        protected ICacheItem<T> _cache;
        protected ICacheLease _lease;

        /// <summary>
        ///     获取或设置一个值，该值表示了当前缓存存根是否表示为一种固态的缓存状态
        /// </summary>
        public bool Fixed
        {
            get { return _fixed; }
            set { _fixed = value; }
        }

        /// <summary>
        ///     获取或设置附属属性
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        ///     获取或设置缓存项
        /// </summary>
        public virtual ICacheItem<T> Cache
        {
            get { return _cache; }
            set { _cache = value; }
        }

        T IFixedCacheStub<T>.Cache
        {
            get { return _cache.GetValue(); }
        }

        /// <summary>
        ///     获取缓存的生命周期
        /// </summary>
        public ICacheLease Lease
        {
            get { return _lease; }
        }

        /// <summary>
        ///     清除所有内部资料
        /// </summary>
        public virtual void Clear()
        {
            //nothing to do default.
        }

        /// <summary>
        ///     获取缓存生命周期
        /// </summary>
        /// <returns></returns>
        public ICacheLease GetLease()
        {
            return _lease;
        }

        #endregion

        #region Implementation of IReadonlyCacheStub<T>

        /// <summary>
        ///     获取缓存
        /// </summary>
        T IReadonlyCacheStub<T>.Cache
        {
            get
            {
                if (_cache == null || GetLease().IsDead)
                {
                    return default(T);
                }
                return _cache.GetValue();
            }
        }

        #endregion
    }
}