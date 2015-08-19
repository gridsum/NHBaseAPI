namespace Gridsum.NHBaseThrift.Proxies
{
    /// <summary>
    ///     内存段代理器工厂
    /// </summary>
    public static class MemorySegmentProxyFactory
    {
        #region Methods

        /// <summary>
        ///     创建一个新的内存段代理器
        /// </summary>
        /// <returns>返回</returns>
        public static IMemorySegmentProxy Create()
        {
            return new MemorySegmentProxy();
        }

        #endregion
    }
}