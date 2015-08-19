namespace Gridsum.NHBaseThrift.Objects
{
    /// <summary>
    ///     Thrift message head.
    /// </summary>
    /// <remarks>
    /// <para>Version  |  Command-Length  | Command  |  Sequence-ID</para>
    /// <para>\-- 4 --/\--------- 4 -------/ \--- N ----/\------ 4 -----/</para>
    /// </remarks>
    public class MessageIdentity
    {
        #region Members.

        /// <summary>
        ///     Gets or sets current applied Thrift protocol version.
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        ///     Gets or sets current Thrift protocol command total length.
        /// </summary>
        public uint CommandLength { get; set; }
        /// <summary>
        ///     Gets or sets current used Thrift communication command.
        /// </summary>
        public string Command { get; set; }
        /// <summary>
        ///     Gets or sets a automatically increments value that which indicated a unqiue message id on the same TCP connection.
        /// </summary>
        public uint SequenceId { get; set; }

        #endregion

        #region Methods.

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("(VER={0}, CMD-LEN={1}, CMD={2}, SEQID={3})", Version, CommandLength, Command, SequenceId);
        }

        #endregion
    }
}