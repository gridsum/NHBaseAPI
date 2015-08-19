using Gridsum.NHBaseThrift.Enums;

namespace Gridsum.NHBaseThrift.Attributes
{
    /// <summary>
    ///     Thrift Protocol Member's Attribute
    /// </summary>
    public class ThriftPropertyAttribute : System.Attribute
    {
        #region Constructor.

        /// <summary>
        ///     Thrift Protocol Member's Attribute
        /// </summary>
        /// <param name="id">Property ID</param>
        /// <param name="propertyType">Property Type</param>
        /// <param name="needWriteOverheads">A flag indicated that whether applies Thrift's TField binary format while serializing.</param>
        public ThriftPropertyAttribute(short id, PropertyTypes propertyType, bool needWriteOverheads = true)
        {
            _propertyType = propertyType;
            _id = id;
            NeedWriteOverheads = needWriteOverheads;
        }

        #endregion

        #region Members.

        private readonly short _id;
        private readonly PropertyTypes _propertyType;

        /// <summary>
        ///     Gets unique property id for current Thrift object.
        /// </summary>
        public short Id
        {
            get { return _id; }
        }

        /// <summary>
        ///     Gets property type
        /// </summary>
        public PropertyTypes PropertyType
        {
            get { return _propertyType; }
        }

        /// <summary>
        ///     Gets or sets a value indicated that whether this value is required.
        /// </summary>
        public bool Optional { get; set; }

        /// <summary>
        ///     Gets or sets a flag indicated that whether applies Thrift's TField binary format while serializing.
        ///     <para>By default this value is: true</para>
        /// </summary>
        /// <remarks>
        ///             Thrift protocol TField's overhead (3 bytes).
        ///             -----------------------------------
        /// 
        ///     		  Field-Type  |   Field-ID  
        ///              \---- 1 ---/\---- 2 ----/
        /// </remarks>
        public bool NeedWriteOverheads { get; set; }

        #endregion
    }
}