using System;

namespace Gridsum.NHBaseThrift.Exceptions
{
    /// <summary>
    ///     通信超时异常
    /// </summary>
    public class CommunicationTimeoutException : Exception
	{
		#region Constructor.

		/// <summary>
		///     通信超时异常
		/// </summary>
		public CommunicationTimeoutException(int seqId) : base(String.Format("#Transation SEQ ID: {0} had timeout.", seqId))
		{

		}

		#endregion
	}
}