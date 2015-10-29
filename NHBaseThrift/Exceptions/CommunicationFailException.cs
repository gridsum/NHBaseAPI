using System;

namespace Gridsum.NHBaseThrift.Exceptions
{
    /// <summary>
    ///     通信失败异常
    /// </summary>
    public class CommunicationFailException : Exception
    {
		#region Constructor.

		/// <summary>
		///     通信失败异常
		/// </summary>
		public CommunicationFailException(int seqId)
			: base(String.Format("#Transation SEQ ID: {0} Communication Failed.", seqId))
		{

		}

		#endregion
    }
}