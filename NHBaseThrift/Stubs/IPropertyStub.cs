using System.Reflection;

namespace NHBaseThrift.Stubs
{
    /// <summary>
    ///     字段处理器存根接口
    /// </summary>
    public interface IPropertyStub
    {
        #region Methods.

        /// <summary>
        ///     初始化
        /// </summary>
        /// <param name="method">字段GetGet method</param>
        void Initialize(MethodInfo method);
        /// <summary>
        ///     获取字段值
        /// </summary>
        /// <typeparam name="T">字段类型</typeparam>
        /// <param name="target">字段所属类实例</param>
        /// <returns>返回字段值</returns>
        T Get<T>(object target);

        #endregion
    }
}