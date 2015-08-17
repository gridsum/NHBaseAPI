using System.Reflection;

namespace NHBaseThrift.Stubs
{
    /// <summary>
    ///     字段处理器存根接口
    /// </summary>
    public interface IPropertySetStub
    {
        #region Methods.

        /// <summary>
        ///     初始化
        /// </summary>
        /// <param name="method">字段GetGet method</param>
        void Initialize(MethodInfo method);

        /// <summary>
        ///     设置字段值
        /// </summary>
        /// <typeparam name="T">字段类型</typeparam>
        /// <param name="target">字段所属类实例</param>
        /// <param name="value"> 字段的值</param>
        /// <returns>返回字段值</returns>
        void Set<T>(object target, T value);

        #endregion
    }
}