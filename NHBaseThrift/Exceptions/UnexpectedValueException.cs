namespace NHBaseThrift.Exceptions
{
    /// <summary>
    ///     未期待的结果异常
    /// </summary>
    public class UnexpectedValueException : System.Exception
    {
        #region Constructor

        /// <summary>
        ///     未期待的结果异常
        /// </summary>
        /// <param name="message">错误消息</param>
        public UnexpectedValueException(string message)
            : base(message)
        {

        }

        #endregion
    }
}