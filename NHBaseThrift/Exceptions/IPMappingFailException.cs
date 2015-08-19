namespace Gridsum.NHBaseThrift.Exceptions
{
    /// <summary>
    ///     找不到主机名对应的IP地址异常
    /// </summary>
    public class IPMappingFailException : System.Exception
    {
        #region Constructor

        /// <summary>
        ///     找不到主机名对应的IP地址异常
        /// </summary>
        /// <param name="message">错误消息</param>
        public IPMappingFailException(string message)
            : base(message)
        {

        }

        #endregion
    }
}