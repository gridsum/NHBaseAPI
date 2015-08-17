using System;

namespace NHBaseThrift.Exceptions
{
    /// <summary>
    ///     字段值为空异常
    ///     <para>* 仅当Attribute.IsRequire = true && Value == null 时触发</para>
    /// </summary>
    [Serializable]
    public class PropertyNullValueException : System.Exception
    {
        #region Constructor

        /// <summary>
        ///     字段值为空异常
        ///     <para>* 仅当Attribute.IsRequire = true && Value == null 时触发</para>
        /// </summary>
        public PropertyNullValueException(string message)
            : base(message)
        {

        }

        #endregion
    }
}