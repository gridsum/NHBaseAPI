using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Gridsum.NHBaseThrift.Attributes;
using Gridsum.NHBaseThrift.Contracts;
using Gridsum.NHBaseThrift.Exceptions;
using KJFramework.Helpers;

namespace Gridsum.NHBaseThrift.Analyzing
{
    /// <summary>
    ///     可转换为元数据的Thrift协议类型分析器，提供了相关的基本操作。
    /// </summary>
    internal class ToBytesIntellectTypeAnalyser : ThriftProtocolTypeAnalyser<ToBytesAnalyseResult[], IThriftObject>
    {
        #region Methods.

        /// <summary>
        ///     分析一个类型中的所有Thrift协议属性
        /// </summary>
        /// <param name="obj">要分析的类型</param>
        /// <returns>返回分析的结果</returns>
        public override ToBytesAnalyseResult[] Analyse(IThriftObject obj)
        {
            if (obj == null) return null;
            Type t = obj.GetType();
            ToBytesAnalyseResult[] result = GetObject(t.FullName);
            if (result != null) return result;
            #region Analyse process.

            IList<ToBytesAnalyseResult> temp = new List<ToBytesAnalyseResult>();
            PropertyInfo[] propertyInfos = t.GetProperties();
            ThriftPropertyAttribute attribute;
            bool nullable;
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                attribute = AttributeHelper.GetCustomerAttribute<ThriftPropertyAttribute>(propertyInfo);
                if (attribute == null) continue;
                nullable = Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null;
                if (!propertyInfo.PropertyType.IsValueType && nullable)
                    throw new DefineNoMeaningException(string.Format(ExceptionMessage.EX_NO_MEANING_VALUE, attribute.Id, propertyInfo.Name, propertyInfo.PropertyType));
                temp.Add(new ToBytesAnalyseResult
                    {
                        VTStruct = GetVT(propertyInfo.PropertyType),
                        Property = propertyInfo,
                        Attribute = attribute,
                        TargetType = t,
                        Nullable = nullable
                    }.Initialize());
            }
            if (temp.Count == 0) return null;
            result = temp.OrderBy(p => p.Attribute.Id).ToArray();
            RegistAnalyseResult(t.FullName, result);
            return result;

            #endregion
        }

        #endregion
    }
}