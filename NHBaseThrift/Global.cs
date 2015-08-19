using System.Configuration;

namespace Gridsum.NHBaseThrift
{
    /// <summary>
    ///    内部全局变量
    /// </summary>
    internal static class Global
    {
        #region Members.

        /// <summary>
        ///    标记一个值，说明了当前Thrift协议框架在产生网络通信时是否需要记录批量插入请求的信息到日志文件中
        ///    <para>* 我们强烈不建议开启此项，因为在批量插入的消息中可能包含了非常大的数据，这些数据所产生的IO量是很大的，这将会直接的拖慢系统运行速度</para>
        /// </summary>
        public static bool NeedLogBatchInsert = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["LOG_BATCH_INSERT"]);
        /// <summary>
        ///     是否允许在内部出现错误时在日志中打印出网络错误所包含的详细信息
        /// </summary>
        public static bool AllowedPrintDumpInfo;
        /// <summary>
        ///    批量插入消息的简易版提示
        /// </summary>
        public static string BatchInsertNotification = "/*Batch Insert Message (We've ignored concrete information here for saving IO.)*/";

        #endregion
    }
}