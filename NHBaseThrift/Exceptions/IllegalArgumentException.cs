using System;

namespace NHBaseThrift.Exceptions
{
	/// <summary>
	///		An IllegalArgument exception indicates an illegal or invalid argument was passed into a procedure. 
	/// </summary>
	class IllegalArgumentException : Exception
	{
		#region Constructor

        /// <summary>
		///		An IllegalArgument exception indicates an illegal or invalid argument was passed into a procedure.    
		/// </summary>
        /// <param name="message">error message</param>
		public IllegalArgumentException(string message)
            : base(message)
        {

        }

        #endregion
	}
}
