using System;

namespace NHBaseThrift.Exceptions
{
	/// <summary>
	///		An IOError exception signals that an error occurred communicating to the Hbase master or an Hbase region server. Also used to return more general Hbase error conditions. 
	/// </summary>
	class IOErrorException : Exception
	{
		#region Constructor

        /// <summary>
		///     An IOError exception signals that an error occurred communicating to the Hbase master or an Hbase region server. Also used to return more general Hbase error conditions. 
        /// </summary>
        /// <param name="message">error message</param>
		public IOErrorException(string message)
            : base(message)
        {

        }

        #endregion
	}
}
