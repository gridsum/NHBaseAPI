﻿using NHBaseThrift.Attributes;
using NHBaseThrift.Contracts;
using NHBaseThrift.Enums;

namespace NHBaseThrift.Objects
{
    /// <summary>
    ///     参数错误信息对象，来自于Thrift协议的RSP消息
    /// </summary>
    public class IllegalArgumentError : ThriftObject
    {
        #region Members.

        /// <summary>
        ///     获取或设置错误原因
        /// </summary>
        [ThriftProperty(1, PropertyTypes.String)]
        public string Reason { get; set; }

        #endregion
    }
}