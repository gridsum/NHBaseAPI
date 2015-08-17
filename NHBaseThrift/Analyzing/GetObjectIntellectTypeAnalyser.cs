using System;
using System.Collections.Generic;
using System.Linq;
using KJFramework.Helpers;
using NHBaseThrift.Attributes;

namespace NHBaseThrift.Analyzing
{
    /// <summary>
    ///     可转换为对象的智能类型分析器，提供了相关的基本操作。
    /// </summary>
    internal class GetObjectIntellectTypeAnalyser : ThriftProtocolTypeAnalyser<Dictionary<short, GetObjectAnalyseResult>, Type>
    {
        #region Methods.

        /// <summary>
        ///     分析一个类型中的所有智能属性
        /// </summary>
        /// <param name="type">要分析的类型</param>
        /// <returns>返回分析的结果</returns>
        public override Dictionary<short, GetObjectAnalyseResult> Analyse(Type type)
        {
            if (type == null) return null;
            Dictionary<short, GetObjectAnalyseResult> result = GetObject(type.FullName);
            if (result != null) return result;
            var targetProperties = type.GetProperties().AsParallel().Where(property => AttributeHelper.GetCustomerAttribute<ThriftPropertyAttribute>(property) != null);
            if (!targetProperties.Any()) return null;
            result = targetProperties.Select(property => new GetObjectAnalyseResult
                                                        {
                                                            VTStruct = GetVT(property.PropertyType),
                                                            Property = property,
                                                            TargetType = type,
                                                            Nullable = Nullable.GetUnderlyingType(property.PropertyType) != null,
                                                            Attribute = AttributeHelper.GetCustomerAttribute<ThriftPropertyAttribute>(property)
                                                        }.Initialize()).DefaultIfEmpty().OrderBy(property => property.Attribute.Id).ToDictionary(property => property.Attribute.Id);
            RegistAnalyseResult(type.FullName, result);
            return result;
        }

        #endregion
    }
}