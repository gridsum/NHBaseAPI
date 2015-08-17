using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NHBaseThrift.Stubs
{
    /// <summary>
    ///     字段存根帮助器
    /// </summary>
    internal sealed class PropertyStubHelper
    {
        #region Methods

        /// <summary>
        ///     创建一个新的字段处理器存根
        /// </summary>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="outputType">字段类型</param>
        /// <param name="method">字段GetGet method</param>
        /// <returns>   </returns>
        public static IPropertyStub Create(Type targetType, Type outputType, MethodInfo method)
        {
            if (targetType == null) throw new ArgumentNullException("targetType");
            if (outputType == null) throw new ArgumentNullException("outputType");
            if (method == null) throw new ArgumentNullException("method");
            string funcTypeStr = string.Format("System.Func`2[[{0}], [{1}]]", targetType.AssemblyQualifiedName, outputType.AssemblyQualifiedName);
            Type funcType = Type.GetType(funcTypeStr);
            MethodInfo getTypeMethod = typeof(Type).GetMethod("GetType", new[] { typeof(string), typeof(bool) });
            MethodInfo invokeMethod = funcType.GetMethod("Invoke");
            MethodInfo createMethod = typeof(System.Delegate).GetMethod("CreateDelegate", new[] { typeof(Type), typeof(MethodInfo) });
            AssemblyBuilder assBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("assembly.New"), AssemblyBuilderAccess.RunAndSave);
            TypeBuilder typeBuilder = assBuilder.DefineDynamicModule("assembly.Module").DefineType("assembly.Type", TypeAttributes.Class | TypeAttributes.Public, typeof(object), new[] { typeof(IPropertyStub) });
            var funcFieldBuilder = typeBuilder.DefineField("_func", funcType, FieldAttributes.Private);
            var methodFieldBuilder = typeBuilder.DefineField("_method", typeof(MethodInfo), FieldAttributes.Private);
            var funcTypeFieldBuilder = typeBuilder.DefineField("_type", typeof(Type), FieldAttributes.Private);
            #region #Initialize method.

            MethodBuilder initMethod = typeBuilder.DefineMethod("Initialize", MethodAttributes.Public | MethodAttributes.Virtual, typeof(void), new[] { typeof(MethodInfo) });
            ILGenerator initGenerator = initMethod.GetILGenerator();
            initGenerator.Emit(OpCodes.Ldarg_0);
            initGenerator.Emit(OpCodes.Ldarg_1);
            initGenerator.Emit(OpCodes.Stfld, methodFieldBuilder);
            initGenerator.Emit(OpCodes.Ldarg_0);
            initGenerator.Emit(OpCodes.Ldstr, funcTypeStr);
            initGenerator.Emit(OpCodes.Ldc_I4_1);
            initGenerator.Emit(OpCodes.Call, getTypeMethod);
            initGenerator.Emit(OpCodes.Stfld, funcTypeFieldBuilder);
            initGenerator.Emit(OpCodes.Ldarg_0);
            initGenerator.Emit(OpCodes.Ldarg_0);
            initGenerator.Emit(OpCodes.Ldfld, funcTypeFieldBuilder);
            initGenerator.Emit(OpCodes.Ldarg_0);
            initGenerator.Emit(OpCodes.Ldfld, methodFieldBuilder);
            initGenerator.Emit(OpCodes.Call, createMethod);
            initGenerator.Emit(OpCodes.Stfld, funcFieldBuilder);
            initGenerator.Emit(OpCodes.Ret);

            #endregion

            #region #Get method.

            MethodBuilder methodBuilder = typeBuilder.DefineMethod("Get", MethodAttributes.Public | MethodAttributes.Virtual);
            methodBuilder.SetParameters(typeof(object));
            GenericTypeParameterBuilder[] genericTypeParameterBuilders = methodBuilder.DefineGenericParameters("T");
            methodBuilder.SetReturnType(genericTypeParameterBuilders[0]);
            ILGenerator ilGenerator = methodBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, funcFieldBuilder);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Callvirt, invokeMethod);
            ilGenerator.Emit(OpCodes.Ret);

            #endregion
            Type type = typeBuilder.CreateType();
            return (IPropertyStub)type.Assembly.CreateInstance(type.FullName);
        }

        #endregion
    }
}