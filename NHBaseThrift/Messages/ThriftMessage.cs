using NHBaseThrift.Attributes;
using NHBaseThrift.Contracts;
using NHBaseThrift.Enums;
using NHBaseThrift.Objects;

namespace NHBaseThrift.Messages
{
    /// <summary>
    ///     Thrift network communication message.
    /// </summary>
    public class ThriftMessage : ThriftObject
    {
        #region Members.

        protected const uint VERSION_1 = 0x80010000;

        /// <summary>
        ///     Gets current Thrift communication message's identity.
        /// </summary>
        [ThriftProperty(-1, PropertyTypes.Struct, false)]
        public MessageIdentity Identity { get; internal set; }

        #endregion
    }
}