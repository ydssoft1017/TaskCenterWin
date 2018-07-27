using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
namespace TaskUtility
{
    public class AssemblyHelper
    {

        /// <summary>
        /// 已加载文件列表
        /// </summary>
        private static readonly List<string> LoadedFileList = new List<string>();

        /// <summary>
        /// 获取程序集中继承了指定类型的类或者接口(搜索一层继承)
        /// </summary>
        /// <param name="assembly">程序集对象</param>
        /// <param name="baseType">基类型</param>
        /// <returns>返回继承了指定类型的类或接口</returns>
        public static Type[] GetSubClassOrInterface(Assembly assembly, Type baseType)
        {
            return GetSubClassOrInterface(assembly.GetTypes(), baseType);
        }

        /// <summary>
        /// 获取程序集中继承了指定类型的类或者接口(搜索一层继承)
        /// </summary>
        /// <param name="assemblyPath">程序集名称</param>
        /// <param name="baseType">基类型</param>
        /// <returns>返回继承了指定类型的类或接口</returns>
        public static Type[] GetSubClassOrInterface(string assemblyPath, Type baseType)
        {
            Assembly assembly = Assembly.Load(assemblyPath);
            return GetSubClassOrInterface(assembly, baseType);
        }

        /// <summary>
        /// 获取程序集中继承了指定类型的类或者接口(搜索一层继承)
        /// </summary>
        /// <param name="types">类型集合</param>
        /// <param name="baseType">基类型</param>
        /// <returns>返回继承了指定类型的类或接口</returns>
        public static Type[] GetSubClassOrInterface(IEnumerable<Type> types, Type baseType)
        {
            return types.Where(typeItem => typeItem.BaseType == baseType).ToArray();
        }

        /// <summary>
        /// 获取程序集中继承了指定接口的接口
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="interfaces">接口类型</param>
        /// <returns>返回继承了指定接口的接口</returns>
        public static Type[] GetSubInterface(Assembly assembly, params Type[] interfaces)
        {
            return GetSubInterface(assembly.GetTypes(), interfaces);
        }

        /// <summary>
        /// 获取程序集中继承了指定接口的接口
        /// </summary>
        /// <param name="assemblyPath">程序集路径</param>
        /// <param name="interfaces">接口类型</param>
        /// <returns>返回继承了指定接口的接口</returns>
        public static Type[] GetSubInterface(string assemblyPath, params Type[] interfaces)
        {
            Assembly assembly = Assembly.Load(assemblyPath);
            return GetSubInterface(assembly, interfaces);
        }

        /// <summary>
        /// 获取程序集中继承了指定接口的接口
        /// </summary>
        /// <param name="types">类型集合</param>
        /// <param name="interfaces">接口类型</param>
        /// <returns>返回继承了指定接口的接口</returns>
        public static Type[] GetSubInterface(IEnumerable<Type> types, params Type[] interfaces)
        {
            return types.Where(type => ValidInterface(type, interfaces)).ToArray();
        }

        /// <summary>
        /// 获取程序集中实现了指定接口的类
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="containAbstract">是否包含抽象类</param>
        /// <param name="interfaces">接口类型</param>
        /// <returns>返回实现了指定接口的类</returns>
        public static Type[] GetInterfaceSubClass(Assembly assembly, bool containAbstract,
                                                             params Type[] interfaces)
        {
            return GetInterfaceSubClass(assembly.GetTypes(), containAbstract, interfaces);
        }

        /// <summary>
        /// 获取程序集中实现了指定接口的类
        /// </summary>
        /// <param name="assemblyPath">程序集路径</param>
        /// <param name="containAbstract">是否包含抽象类</param>
        /// <param name="interfaces">接口类型</param>
        /// <returns>返回实现了指定接口的类</returns>
        public static Type[] GetInterfaceSubClass(string assemblyPath, bool containAbstract,
                                                             params Type[] interfaces)
        {
            Assembly assembly = Assembly.Load(assemblyPath);
            return GetInterfaceSubClass(assembly, containAbstract, interfaces);
        }

        /// <summary>
        /// 获取程序集中实现了指定接口的类
        /// </summary>
        /// <param name="types">程序集</param>
        /// <param name="containAbstract">是否包含抽象类</param>
        /// <param name="interfaces">接口类型</param>
        /// <returns>返回实现了指定接口的类</returns>
        public static Type[] GetInterfaceSubClass(IEnumerable<Type> types, bool containAbstract,
                                                             params Type[] interfaces)
        {
            return types.Where(type => ValidInterfaceClass(type, containAbstract, interfaces)).ToArray();
        }

        /// <summary>
        /// 获取程序集中继承了指定类型的类或者实现了指定的接口(搜索整个继承继承链)。
        /// </summary>
        /// <param name="types">类型集合</param>
        /// <param name="containAbstract">是否包含抽象类</param>
        /// <param name="containInterface">是否包含接口</param>
        /// <param name="baseType">基类型</param>
        /// <returns>返回当前应用程序中的所有类型中检索继承自指定类型或者实现了指定类型的类型列表。</returns>
        public static Type[] GetSubClass(IEnumerable<Type> types, bool containAbstract, bool containInterface,
                                                    Type baseType)
        {
            if (baseType == null) return null;
            var list = new List<Type>();
            foreach (Type t in types)
            {
                if (!containAbstract && t.IsAbstract)
                {
                    continue;
                }
                if (!containInterface && t.IsInterface)
                {
                    continue;
                }
                if (CheckSubClass(t, baseType))
                {
                    list.Add(t);
                }
            }
            return list.ToArray();
        }

        private static bool CheckSubClass(Type type, Type checkType)
        {
            if (type.BaseType != null)
            {
                if (type.BaseType == checkType)
                {
                    return true;
                }
                return CheckSubClass(type.BaseType, checkType);
            }
            return false;
        }

        /// <summary>
        /// 检测type是否是baseType的子类(搜索整个继承链)
        /// </summary>
        /// <param name="type">检查的类型</param>
        /// <param name="baseType">基类</param>
        /// <returns>如果type是否是baseType的子类,返回true</returns>
        public static bool IsSubClass(Type type, Type baseType)
        {
            return CheckSubClass(type, baseType);
        }

        /// <summary>
        /// 验证接口是否继承自指定的接口(继承任意一个接口即可)
        /// </summary>
        /// <param name="type">待验证的接口类型</param>
        /// <param name="interfaces">接口类型</param>
        /// <returns>如果继承自指定的接口返回true</returns>
        public static bool ValidInterface(Type type, params Type[] interfaces)
        {
            return type.IsInterface && interfaces.Any(p => type.GetInterface(p.FullName) != null);
        }

        /// <summary>
        /// 验证类是否实现指定的接口(继承任意一个接口即可)
        /// </summary>
        /// <param name="type">待验证的类型</param>
        /// <param name="containAbstract">是否包含抽象类</param>
        /// <param name="interfaces">接口类型</param>
        /// <returns>如果类实现指定的接口返回true</returns>
        public static bool ValidInterfaceClass(Type type, bool containAbstract, params Type[] interfaces)
        {
            if (!type.IsClass || !type.IsPublic// || type.ContainsGenericParameters
                || (!containAbstract && type.IsAbstract))
            {
                return false;
            }

            return interfaces.Any(p => type.GetInterface(p.FullName) != null);
        }

        /// <summary>
        /// 验证类是否实现指定的接口(继承任意一个接口即可)
        /// </summary>
        /// <param name="type">待验证的类型</param>
        /// <param name="containAbstract">是否包含抽象类</param>
        /// <param name="classes">接口类型</param>
        /// <returns>如果类实现指定的接口返回true</returns>
        public static bool ValidClass(Type type, bool containAbstract, params Type[] classes)
        {
            if (!type.IsClass || !type.IsPublic// || type.ContainsGenericParameters
                || (!containAbstract && type.IsAbstract))
            {
                return false;
            }
            return classes.Any(p => type.BaseType == p.BaseType);
        }

        /// <summary>
        /// 获取类型全名包含程序集名称
        /// </summary>
        /// <param name="type">类型</param>
        public static string GetTypeFullName(Type type)
        {
            if (type.AssemblyQualifiedName != null)
            {
                string[] qualisz = type.AssemblyQualifiedName.Split(',');
                return String.Concat(qualisz[0], ",", qualisz[1]);
            }
            return String.Empty;
        }

        /// <summary>
        /// 获取程序集中指定名称的文本资源
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <param name="assembly">程序集</param>
        /// <returns>返回Text,如果不存在指定的资源名称则返回空字符串</returns>
        public static string GetTextFromResources(string name, Assembly assembly)
        {
            Stream stream = assembly.GetManifestResourceStream(name);
            return stream == null ? string.Empty : new StreamReader(stream).ReadToEnd();
        }

        /// <summary>
        /// 获取类自定义特性对象(如果有多个返回第一个,如果一个也没有返回null)
        /// </summary>
        /// <typeparam name="T">自定义特性类型</typeparam>
        /// <param name="type">对象类型</param>
        public static T GetAttribute<T>(Type type) where T : Attribute
        {
            object[] attsz = type.GetCustomAttributes(typeof(T), false);
            if (attsz.Length > 0)
            {
                return attsz[0] as T;
            }
            return default(T);
        }

        /// <summary>
        /// 获取属性自定义特性对象(如果有多个返回第一个,如果一个也没有返回null)
        /// </summary>
        /// <typeparam name="T">自定义特性类型</typeparam>
        /// <param name="info">对象类型</param>
        public static T GetAttribute<T>(PropertyInfo info) where T : Attribute
        {
            object[] attsz = info.GetCustomAttributes(typeof(T), true);
            if (attsz.Length > 0)
            {
                return attsz[0] as T;
            }
            return default(T);
        }

        /// <summary>
        /// 获取字段自定义特性对象(如果有多个返回第一个,如果一个也没有返回null)
        /// </summary>
        /// <typeparam name="T">自定义特性类型</typeparam>
        /// <param name="info">对象类型</param>
        public static T GetAttribute<T>(FieldInfo info) where T : Attribute
        {
            object[] attsz = info.GetCustomAttributes(typeof(T), true);
            if (attsz.Length > 0)
            {
                T att = (T)attsz[0];
                if (att != null)
                {
                    return att;
                }
            }
            return default(T);
        }

        /// <summary>
        /// 获取程序集标题(AssemblyInfo.cs文件中AssemblyTitle属性定义的标题)
        /// </summary>
        public static string GetAssemblyTitle()
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            if (attributes.Length > 0)
            {
                var titleAttribute = (AssemblyTitleAttribute)attributes[0];
                if (titleAttribute.Title != "")
                {
                    return titleAttribute.Title;
                }
            }
            return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
        }


        /// <summary>
        /// 加载指定目录中的程序集
        /// </summary>
        /// <param name="dirs">目录列表</param>
        public static Type[] LoadType(IEnumerable<string> dirs)
        {
            var fileList = new List<string>();
            foreach (string p in dirs)
            {
                if (Directory.Exists(p) && !LoadedFileList.Any(x => DirHelper.EqualsDirectory(x, p)))
                {
                    fileList.AddRange(Directory.GetFiles(p));
                    LoadedFileList.Add(p);
                }
            }
            var typeList = new List<Type>();
            typeList.AddRange(LoadTypeByFile(fileList.ToArray()));
            return typeList.ToArray();
        }

        /// <summary>
        /// 加载指定文件中的程序集
        /// </summary>
        /// <param name="files">文件列表</param>
        public static Type[] LoadTypeByFile(string[] files)
        {
            var typeList = new List<Type>();
            foreach (string item in files)
            {
                if (!ValidateFileExtension(item)) continue;
                string assemblyName = Path.GetFileNameWithoutExtension(item);
                if (string.IsNullOrEmpty(assemblyName)) continue;
                try
                {
                    Assembly assembly = Assembly.Load(assemblyName);
                    foreach (var t in assembly.GetTypes())
                    {
                        if (typeList.IndexOf(t) == -1)
                        {
                            typeList.Add(t);
                        }
                    }
                }
                catch (Exception e)
                {
                    //DebugHelper.Debug(e.Message);
                }
            }

            return typeList.ToArray();
        }

        /// <summary>
        /// 验证文件扩展名
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>如果文件扩展是exe或者dll返回True</returns>
        private static bool ValidateFileExtension(string filePath)
        {
            string fileExtension = Path.GetExtension(filePath);
            if (string.IsNullOrEmpty(fileExtension)) return false;

            bool result = (fileExtension.Equals(".dll") || fileExtension.Equals(".exe"))
                          && filePath.IndexOf("vshost", StringComparison.Ordinal) == -1;
            return result;
        }

        /// <summary>
        /// 是否能成功加载指定的程序集
        /// </summary>
        /// <param name="assemblyFullNames">程序集全名数组</param>
        public static BoolMessage IsCanLoadAssembly(string[] assemblyFullNames)
        {
            try
            {
                foreach (var item in assemblyFullNames)
                {
                    AppDomain.CurrentDomain.Load(item);
                }
                return BoolMessage.True;
            }
            catch (Exception e)
            {
                return new BoolMessage(false, e.Message);
            }
        }
    }
}
