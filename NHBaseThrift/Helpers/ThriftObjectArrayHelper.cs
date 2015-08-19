using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;
using Gridsum.NHBaseThrift.Contracts;

namespace Gridsum.NHBaseThrift.Helpers
{
    /// <summary>
    ///     智能类型数组帮助器
    /// </summary>
    internal static class ThriftObjectArrayHelper
    {
        #region Members

        private static readonly ConcurrentDictionary<string, object> _methods = new ConcurrentDictionary<string, object>();

        #endregion

        #region Methods

        /// <summary>
        ///     获取指定类型的功能函数
        /// </summary>
        /// <param name="type">数组类型</param>
        /// <typeparam name="T">数组元素类型</typeparam>
        /// <returns>返回一个功能函数</returns>
        public static Func<int, T[]> GetFunc<T>(Type type)
            where T : ThriftObject
        {
            object obj;
            string fullname = type.FullName;
            if (_methods.TryGetValue(fullname, out obj)) return (Func<int, T[]>) obj;
            //create cache method.
            DynamicMethod dynamicMethod = new DynamicMethod(string.Format("CreateArrayInstnaceBy: {0}", fullname),
                                                            MethodAttributes.Public | MethodAttributes.Static,
                                                            CallingConventions.Standard, type,
                                                            new[] {typeof (int)}, typeof (object), true);
            ILGenerator generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Newarr, type.GetElementType());
            generator.Emit(OpCodes.Ret);
            Func<int, T[]> func = (Func<int, T[]>) dynamicMethod.CreateDelegate(typeof (Func<int, T[]>));
            if (!_methods.TryAdd(fullname, func))
                throw new InvalidOperationException("#Cannot add specific func to ConcurrentDictionary");
            return func;
        }

        #endregion
    }
}