namespace NHBaseThrift.Exceptions
{
    /// <summary>
    ///     Region无法被计算的异常
    /// </summary>
    public class RegionNotFoundException : System.Exception
    {
        #region Constructor

        /// <summary>
        ///     已经存在指定资源异常
        /// </summary>
        /// <param name="message">错误消息</param>
        public RegionNotFoundException(string message)
            : base(message)
        {

        }

        #endregion
    }
}