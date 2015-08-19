namespace Gridsum.NHBaseThrift.Exceptions
{
    /// <summary>
    ///     定义无意义异常
    /// </summary>
    public class DefineNoMeaningException : System.Exception
    {
        #region Constructor

        /// <summary>
        ///     定义无意义异常
        /// </summary>
        /// <param name="message">错误消息</param>
        public DefineNoMeaningException(string message)
            : base(message)
        {

        }

        #endregion
    }
}