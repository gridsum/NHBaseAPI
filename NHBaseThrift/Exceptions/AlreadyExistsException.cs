namespace Gridsum.NHBaseThrift.Exceptions
{
    /// <summary>
    ///     已经存在指定资源异常
    /// </summary>
    public class AlreadyExistsException : System.Exception
    {
        #region Constructor

        /// <summary>
        ///     已经存在指定资源异常
        /// </summary>
        /// <param name="message">错误消息</param>
        public AlreadyExistsException(string message)
            : base(message)
        {

        }

        #endregion
    }
}