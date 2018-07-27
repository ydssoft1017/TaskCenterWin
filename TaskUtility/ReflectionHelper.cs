using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
namespace TaskUtility
{
    public static class ReflectionHelper
    {
        /// <summary>
        /// 获取对象包装方法列表
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <returns>对象包装方法列表</returns>
        public static List<FastMethod> GetFastMethods(Type type)
        {
            List<FastMethod> fastMethods = new List<FastMethod>();
            MethodInfo[] methods = type.GetMethods();
            foreach (MethodInfo method in methods)
            {
                fastMethods.Add(new FastMethod(method));
            }
            return fastMethods;
        }

        /// <summary>
        /// 获取对象包装属性列表
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <returns>对象包装属性列表</returns>
        public static List<FastProperty> GetFastProperties(Type type)
        {
            List<FastProperty> fastProperties = new List<FastProperty>();
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                fastProperties.Add(new FastProperty(property));
            }
            return fastProperties;
        }

        /// <summary>
        /// 获取对象包装属性
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns>对象包装属性</returns>
        public static FastProperty GetFastProperty(Type type, string propertyName)
        {
            PropertyInfo property = type.GetProperty(propertyName);
            return new FastProperty(property);
        }

        /// <summary>
        /// 获取对象包装方法
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="methodName">方法名称</param>
        /// <returns>对象包装方法</returns>
        public static FastMethod GetFastMethod(Type type, string methodName)
        {
            MethodInfo method = type.GetMethod(methodName);
            return new FastMethod(method);
        }

        /// <summary>
        /// 创建对象实例 (使用无参数的构造函数创建)
        /// </summary>
        /// <typeparam name="TResult">对象类型</typeparam>
        /// <remarks>
        /// 如果类没有无参数的构造函数则执行会失败
        /// </remarks>
        /// <returns>对象实例</returns>
        public static TResult CreateInstance<TResult>() where TResult : class
        {
            NewExpression newExpression = Expression.New(typeof(TResult));
            Expression<Func<TResult>> newLambda = Expression.Lambda<Func<TResult>>(
                newExpression, null);
            return newLambda.Compile()();
        }

        /// <summary>
        /// 创建对象实例 (使用无参数的构造函数创建)
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="type">对象类型</param>
        public static TResult CreateInstance<TResult>(Type type)
        {
            NewExpression newExpression = Expression.New(type);
            Expression<Func<TResult>> newLambda = Expression.Lambda<Func<TResult>>(
                newExpression, null);
            return newLambda.Compile()();
        }

        /// <summary>
        /// 创建对象实例 (使用无参数的构造函数创建)
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <remarks>
        /// 如果类没有无参数的构造函数则执行会失败
        /// </remarks>
        /// <returns>对象实例</returns>
        public static object CreateInstance(Type type)
        {
            NewExpression newExpression = Expression.New(type);
            Expression<Func<object>> newLambda = Expression.Lambda<Func<object>>(newExpression, null);
            return newLambda.Compile()();
        }

        /// <summary>
        /// 创建对象实例 (使用无参数的构造函数创建)
        /// </summary>
        /// <param name="provider">类型字符串(类,程序集)</param>
        /// <returns>对象实例</returns>
        public static object CreateInstance(string provider)
        {
            if (string.IsNullOrEmpty(provider)) return null;
            Type type = Type.GetType(provider, false);
            if (type == null) return null;

            return CreateInstance(type);
        }

        /// <summary>
        /// 创建泛型对象实例 (使用无参数的构造函数创建)
        /// </summary>
        /// <param name="provider">类型字符串(类,程序集)</param>
        /// <param name="genericType">泛型类型</param>
        /// <returns>对象实例</returns>
        public static object CreateGenericInstance(string provider, params Type[] genericType)
        {
            if (string.IsNullOrEmpty(provider)) return null;
            Type type = Type.GetType(provider, false);
            return CreateGenericInstance(type, genericType);
        }

        /// <summary>
        /// 创建泛型对象实例 (使用无参数的构造函数创建)
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="genericType">泛型类型</param>
        /// <returns>对象实例</returns>
        public static object CreateGenericInstance(Type type, params Type[] genericType)
        {
            if (type == null) return null;
            return Activator.CreateInstance(type.MakeGenericType(genericType));
        }

        /// <summary>
        /// 创建对象实例 (使用无参数的构造函数创建)
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="provider">类型字符串(类,程序集)</param>
        /// <returns>对象实例</returns>
        public static T CreateInstance<T>(string provider) where T : class
        {
            var instance = CreateInstance(provider);
            if (instance == null) return default(T);
            return (T)instance;
        }

        /// <summary>
        /// 获取方法调用句柄
        /// </summary>
        /// <param name="methodInfo">方法信息</param>
        /// <returns>方法调用句柄</returns>
        public static FastInvokeHandler GetFastInvoker(MethodInfo methodInfo)
        {
            if (methodInfo.DeclaringType == null)
            {
                throw new ArgumentException("methodInfo.DeclaringType不能为空");
            }
            DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, typeof(object),
                                                            new[] { typeof(object), typeof(object[]) },
                                                            methodInfo.DeclaringType.Module);
            ILGenerator il = dynamicMethod.GetILGenerator();
            ParameterInfo[] ps = methodInfo.GetParameters();
            Type[] paramTypes = new Type[ps.Length];
            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (ps[i].ParameterType.IsByRef)
                    paramTypes[i] = ps[i].ParameterType.GetElementType();
                else
                    paramTypes[i] = ps[i].ParameterType;
            }
            LocalBuilder[] locals = new LocalBuilder[paramTypes.Length];

            for (int i = 0; i < paramTypes.Length; i++)
            {
                locals[i] = il.DeclareLocal(paramTypes[i], true);
            }
            for (int i = 0; i < paramTypes.Length; i++)
            {
                il.Emit(OpCodes.Ldarg_1);
                EmitFastInt(il, i);
                il.Emit(OpCodes.Ldelem_Ref);
                EmitCastToReference(il, paramTypes[i]);
                il.Emit(OpCodes.Stloc, locals[i]);
            }
            if (!methodInfo.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
            }
            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (ps[i].ParameterType.IsByRef)
                    il.Emit(OpCodes.Ldloca_S, locals[i]);
                else
                    il.Emit(OpCodes.Ldloc, locals[i]);
            }
            il.EmitCall(methodInfo.IsStatic ? OpCodes.Call : OpCodes.Callvirt, methodInfo, null);
            if (methodInfo.ReturnType == typeof(void))
                il.Emit(OpCodes.Ldnull);
            else
                EmitBoxIfNeeded(il, methodInfo.ReturnType);

            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (ps[i].ParameterType.IsByRef)
                {
                    il.Emit(OpCodes.Ldarg_1);
                    EmitFastInt(il, i);
                    il.Emit(OpCodes.Ldloc, locals[i]);
                    var localType = locals[i].LocalType;
                    if (localType != null && localType.IsValueType)
                        il.Emit(OpCodes.Box, localType);
                    il.Emit(OpCodes.Stelem_Ref);
                }
            }

            il.Emit(OpCodes.Ret);
            FastInvokeHandler invoder = (FastInvokeHandler)dynamicMethod.CreateDelegate(typeof(FastInvokeHandler));
            return invoder;
        }

        #region Emit操作

        private static void EmitCastToReference(ILGenerator il, Type type)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, type);
            }
            else
            {
                il.Emit(OpCodes.Castclass, type);
            }
        }

        private static void EmitBoxIfNeeded(ILGenerator il, Type type)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Box, type);
            }
        }

        private static void EmitFastInt(ILGenerator il, int value)
        {
            switch (value)
            {
                case -1:
                    il.Emit(OpCodes.Ldc_I4_M1);
                    return;
                case 0:
                    il.Emit(OpCodes.Ldc_I4_0);
                    return;
                case 1:
                    il.Emit(OpCodes.Ldc_I4_1);
                    return;
                case 2:
                    il.Emit(OpCodes.Ldc_I4_2);
                    return;
                case 3:
                    il.Emit(OpCodes.Ldc_I4_3);
                    return;
                case 4:
                    il.Emit(OpCodes.Ldc_I4_4);
                    return;
                case 5:
                    il.Emit(OpCodes.Ldc_I4_5);
                    return;
                case 6:
                    il.Emit(OpCodes.Ldc_I4_6);
                    return;
                case 7:
                    il.Emit(OpCodes.Ldc_I4_7);
                    return;
                case 8:
                    il.Emit(OpCodes.Ldc_I4_8);
                    return;
            }

            if (value > -129 && value < 128)
            {
                il.Emit(OpCodes.Ldc_I4_S, (SByte)value);
            }
            else
            {
                il.Emit(OpCodes.Ldc_I4, value);
            }
        }

        #endregion
    }

    /// <summary>
    /// 方法调用句柄
    /// </summary>
    /// <param name="target">对象实例</param>
    /// <param name="paramters">参数</param>
    /// <returns>返回值</returns>
    public delegate object FastInvokeHandler(object target, params object[] paramters);

    /// <summary>
    /// 对象包装属性操作
    /// </summary>
    public sealed class FastProperty
    {
        /// <summary>
        /// Get委托
        /// </summary>
        private FastInvokeHandler _getHandler;

        /// <summary>
        /// Set委托
        /// </summary>
        private FastInvokeHandler _setHandler;

        /// <summary>
        /// 初始化对象包装属性
        /// </summary>
        /// <param name="property">属性元数据</param>
        public FastProperty(PropertyInfo property)
        {
            Property = property;
        }

        /// <summary>
        /// 属性对象
        /// </summary>
        public PropertyInfo Property { get; private set; }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="instanse">对象实例</param>
        /// <returns>属性值</returns>
        public object Get(object instanse)
        {
            if (Property == null) return null;
            if (_getHandler == null)
            {
                _getHandler = ReflectionHelper.GetFastInvoker(Property.GetGetMethod());
            }
            return _getHandler(instanse, null);
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="instanse">对象实例</param>
        /// <param name="value">属性值</param>
        public void Set(object instanse, object value)
        {
            if (Property == null) return;
            if (!ObjectHelper.IsNullableType(Property.PropertyType) && (value == DBNull.Value || value == null))
            {
                throw new Exception(string.Format("属性{0}不允许为空,但给定的值为空值,无法赋值", Property.Name));
            }
            if (_setHandler == null)
            {
                _setHandler = ReflectionHelper.GetFastInvoker(Property.GetSetMethod());
            }
            if (_setHandler == null) return;
            try
            {
                _setHandler(instanse, ObjectHelper.ConvertObjectValue(value, Property.PropertyType));
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("设置属性{0}的值时发生异常,{1}", Property.Name, e.Message), e);
            }
        }
    }

    /// <summary>
    /// 对象包装方法
    /// </summary>
    public sealed class FastMethod
    {
        /// <summary>
        /// 方法调用委托
        /// </summary>
        private FastInvokeHandler _callHandler;

        /// <summary>
        /// 初始化对象包装方法
        /// </summary>
        /// <param name="method">方法元数据</param>
        public FastMethod(MethodInfo method)
        {
            Method = method;
        }

        /// <summary>
        /// 方法对象
        /// </summary>
        public MethodInfo Method { get; private set; }

        /// <summary>
        /// 调用
        /// </summary>
        /// <param name="instanse">对象实例</param>
        /// <param name="parameters">参数</param>
        /// <returns>调用返回值</returns>
        public object Call(object instanse, params object[] parameters)
        {
            if (_callHandler == null)
            {
                _callHandler = ReflectionHelper.GetFastInvoker(Method);
            }
            return _callHandler(instanse, parameters);
        }
    }
}
