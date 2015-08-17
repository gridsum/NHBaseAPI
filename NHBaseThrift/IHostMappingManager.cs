namespace NHBaseThrift
{
    /// <summary>
    ///     HOST名称映射管理器接口
    /// </summary>
    internal interface IHostMappingManager
    {
        #region Methods.

        /// <summary>
        ///     根据一个主机名获取IP映射关系
        /// </summary>
        /// <param name="hostName">主机名</param>
        /// <returns>返回对应的IP地址</returns>
        string GetIPAddressByHostName(string hostName);

        #endregion
    }
}