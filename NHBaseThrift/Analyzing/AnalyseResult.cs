using System.Reflection;
using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Stubs;

namespace Gridsum.NHBaseThrift.Analyzing
{
    /// <summary>
    ///     只能类型分析结果，提供了相关的基本操作。
    /// </summary>
    public class AnalyseResult : IAnalyseResult
    {
        private VT _vtStruct;
        /// <summary>
        ///     获取值类型标示
        /// </summary>
        internal bool VT { get; private set; }
        /// <summary>
        ///     获取或设置属性信息
        /// </summary>
        internal PropertyInfo Property { get; set; }
        /// <summary>
        ///     获取或设置智能属性标签
        /// </summary>
        internal ThriftPropertyAttribute Attribute { get; set; }
        /// <summary>
        ///     获取或设置内部VT结构
        /// </summary>
        internal VT VTStruct
        {
            get { return _vtStruct; }
            set 
            {
                _vtStruct = value;
                VT = _vtStruct != null;
            }
        }

        /// <summary>
        ///     获取或设置一个值，该指标是了当前属性是否已经达到了完整缓存
        /// </summary>
        internal bool HasCacheFinished { get; set; }

        /// <summary>
        ///     获取或设置字段是否为可空字段类型
        /// </summary>
        public bool Nullable { get; set; }
    }
}