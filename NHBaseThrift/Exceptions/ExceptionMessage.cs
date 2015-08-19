namespace Gridsum.NHBaseThrift.Exceptions
{
    /// <summary>
    ///     异常信息静态存储结构
    /// </summary>
    internal static class ExceptionMessage
    {
        /// <summary>
        ///     字段值为空的错误提示语
        /// </summary>
        public const string EX_PROPERTY_VALUE = "#Id: {0}\r\n#Property: {1}\r\n#Type: {2}\r\ncannot be serialize, because Attribute.IsRequire = true and property.value is null!";
        public const string EX_NOT_SUPPORTED_VALUE = "#Id: {0}\r\n#Property: {1}\r\n#Type: {2}\r\ncurrent message framework cannot support this type!";
        public const string EX_NOT_SUPPORTED_VALUE_TEMPORARY = "#Type: {0}\r\ncurrent message framework cannot support this type!";
        public const string EX_NO_MEANING_VALUE = "#Id: {0}\r\n#Property: {1}\r\n#Type: {2}\r\nthere is no meaning for set AllowDefaultNull = true with a NON value type or NULLABLE type property!";
        public const string EX_VT_FIND_NOT_PROCESSOR = "#Id: {0}\r\n#Property: {1}\r\n#Type: {2}\r\nVT didn't find processor!";
        public const string EX_METHOD_ACCESS = "#Detect class permission failed! Pls make sure current class {0} has *PUBLIC* declare permission";
        public const string EX_UNEXPRECTED_VALUE = "#Id: {0}\r\n#Property: {1}.\r\nThere was a unexpected NULL value occurred in Blob Type.";
    }
}