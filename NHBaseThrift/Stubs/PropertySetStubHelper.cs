using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NHBaseThrift.Stubs
{
    /// <summary>
    ///     字段存根帮助器
    /// </summary>
    public class PropertySetStubHelper
    {
        #region Methods.

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceType">目标对象类型</param>
        /// <param name="paramsType">参数类型</param>
        /// <returns>返回创建后的字段处理器存根</returns>
        public static IPropertySetStub Create(Type instanceType, Type paramsType)
        {
            string delegateTypeFullName = string.Format("System.Action`2[[{0}],[{1}]]", instanceType.AssemblyQualifiedName, paramsType.AssemblyQualifiedName);
            Type delegateType = Type.GetType(delegateTypeFullName);
            if (delegateType == null) throw new System.Exception("delegateTypeFullName is Invalid!");
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("KJFramework"),
                AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("KJFramework.DynamicModule");
            TypeBuilder typeBuilder = moduleBuilder.DefineType("KJFramework.DynamicModule.Sub",
                                                               TypeAttributes.Class | TypeAttributes.Public,
                                                               typeof(object)
                                                               , new[] { typeof(IPropertySetStub) });
            FieldBuilder subAction = typeBuilder.DefineField("_action", delegateType, FieldAttributes.Private);
            FieldBuilder subMethod = typeBuilder.DefineField("_method", typeof(MethodInfo), FieldAttributes.Private);
            FieldBuilder subType = typeBuilder.DefineField("_type", typeof(Type), FieldAttributes.Private);

            MethodBuilder methodBuilder = typeBuilder.DefineMethod("Initialize", MethodAttributes.Public | MethodAttributes.Virtual, typeof(void), new[] { typeof(MethodInfo) });
            ILGenerator ilGenerator = methodBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Stfld, subMethod);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldstr, delegateTypeFullName);
            ilGenerator.Emit(OpCodes.Ldc_I4_1);
            MethodInfo getTypeMethod = typeof(Type).GetMethod("GetType", new[] { typeof(string), typeof(bool) });
            ilGenerator.Emit(OpCodes.Call, getTypeMethod);
            ilGenerator.Emit(OpCodes.Stfld, subType);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, subType);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, subMethod);
            MethodInfo createDelegate = typeof(System.Delegate).GetMethod("CreateDelegate", new[] { typeof(Type), typeof(MethodInfo) });
            ilGenerator.Emit(OpCodes.Call, createDelegate);
            ilGenerator.Emit(OpCodes.Stfld, subAction);
            ilGenerator.Emit(OpCodes.Ret);

            MethodBuilder setMethod = typeBuilder.DefineMethod("Set", MethodAttributes.Public | MethodAttributes.Virtual);
            setMethod.SetParameters(new[] { typeof(object), setMethod.DefineGenericParameters("T")[0] });
            ILGenerator generator = setMethod.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, subAction);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Ldarg_2);
            MethodInfo invokeMethod = delegateType.GetMethod("Invoke");
            generator.Emit(OpCodes.Callvirt, invokeMethod);
            generator.Emit(OpCodes.Ret);

            Type type = typeBuilder.CreateType();
            return (IPropertySetStub)typeBuilder.Assembly.CreateInstance(type.FullName);
        }

        #endregion
    }
}