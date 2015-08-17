namespace NHBaseThrift.Exceptions
{
    /// <summary>
    ///     指定的key不存在异常
    /// </summary>
    public class SpecificKeyNotExistsException : System.Exception
    {
        #region Constructor

        /// <summary>
        ///     指定的key不存在异常
        /// </summary>
        /// <param name="message">错误消息</param>
        public SpecificKeyNotExistsException(string message)
            : base(message)
        {

        }

        #endregion
    }
}