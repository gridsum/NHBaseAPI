namespace NHBaseThrift.Exceptions
{
    /// <summary>
    ///     网络数据内部计算出错异常
    /// </summary>
    public class IncorrectCalculationException : System.Exception
    {
        #region Constructor

        /// <summary>
        ///     网络数据内部计算出异常
        /// </summary>
        /// <param name="message">错误消息</param>
        public IncorrectCalculationException(string message)
            : base(message)
        {

        }

        #endregion
    }
}