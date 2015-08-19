using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Contracts;
using Gridsum.NHBaseThrift.Enums;

namespace Gridsum.NHBaseThrift.Objects
{
    /// <summary>
    ///     Column Descriptor
    /// </summary>
    public class ColumnDescriptor : ThriftObject
    {
        #region Constructor.

        /// <summary>
        ///     Column Descriptor
        /// </summary>
        public ColumnDescriptor()
        {
            MaxVersions = 3;
            Compression = "NONE";
            BloomFilterTypes = "NONE";
            TimeToLive = 0x7fffffff;
        }

        #endregion

        #region Members.

        /// <summary>
        ///     Gets or sets column name
        /// </summary>
        [ThriftProperty(1, PropertyTypes.String)]
        public string Name { get; set; }
        /// <summary>
		///     Gets or sets MaxVersions
        /// </summary>
        [ThriftProperty(2, PropertyTypes.I32)]
        public int MaxVersions { get; set; }
        /// <summary>
		///     Gets or sets Compression
        /// </summary>
        [ThriftProperty(3, PropertyTypes.String)]
        public string Compression { get; set; }
        /// <summary>
		///     Gets or sets InMemory
        /// </summary>
        [ThriftProperty(4, PropertyTypes.Bool)]
        public bool InMemory { get; set; }
        /// <summary>
		///     Gets or sets BloomFilterTypes
        /// </summary>
        [ThriftProperty(5, PropertyTypes.String)]
        public string BloomFilterTypes { get; set; }
        /// <summary>
		///     Gets or sets BloomFilterVectorSize
        /// </summary>
        [ThriftProperty(6, PropertyTypes.I32)]
        public int BloomFilterVectorSize { get; set; }
        /// <summary>
		///     Gets or sets BloomFilterNbHashes
        /// </summary>
        [ThriftProperty(7, PropertyTypes.I32)]
        public int BloomFilterNbHashes { get; set; }
        /// <summary>
		///     Gets or sets BlockCacheEnabled
        /// </summary>
        [ThriftProperty(8, PropertyTypes.Bool)]
        public bool BlockCacheEnabled { get; set; }
        /// <summary>
		///     Gets or sets TimeToLive
        /// </summary>
        [ThriftProperty(9, PropertyTypes.I32)]
        public int TimeToLive { get; set; }

        #endregion
    }
}