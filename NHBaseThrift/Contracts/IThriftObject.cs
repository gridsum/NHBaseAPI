namespace Gridsum.NHBaseThrift.Contracts
{
    /// <summary>
    ///     Thrift object interface
    /// </summary>
    public interface IThriftObject
    {
        #region Members.

        /// <summary>
        ///     Gets serialized value.
        /// </summary>
        byte[] Body { get; }
        /// <summary>
        ///     Gets a status value which indicated whether it had completed serialization.
        /// </summary>
        bool IsBind { get; }

        #endregion

        #region Methods.

        /// <summary>
        ///     Serialize current Thrift object into data buffer.
        /// </summary>
        void Bind();

        #endregion
    }
}