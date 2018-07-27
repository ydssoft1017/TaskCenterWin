using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Text;

namespace TaskUtility
{
    /// <summary>
    /// 对象帮助类
    /// </summary>
    public static class ObjectHelper
    {
        /// <summary>
        /// 把对象转为字符串表示形式
        /// </summary>
        /// <param name="obj">对象</param>
        public static string GetObjectString(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            var convert = TypeDescriptor.GetConverter(obj.GetType());
            return convert.ConvertToString(obj);
        }

        /// <summary>
        /// 将指定文本转换为对象
        /// </summary>
        /// <param name="objString">对象字符串表示形式</param>
        /// <param name="type">对象类型</param>
        public static object GetObjectFromString(string objString, Type type)
        {
            var convert = TypeDescriptor.GetConverter(type);
            return convert.ConvertFromString(objString);
        }

        /// <summary>
        /// 返回具有指定 System.Type 而且其值等效于指定对象的 System.Object
        /// </summary>
        /// <param name="value">对象值</param>
        /// <param name="type">对象类型</param>
        /// <returns>返回具有指定 System.Type 而且其值等效于指定对象的 System.Object</returns>
        public static object ConvertObjectValue(object value, Type type)
        {
            if (value == DBNull.Value || null == value)
                return null;

            if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var nullableConverter = new NullableConverter(type);

                type = nullableConverter.UnderlyingType;
            }
            if (type == typeof(DateTime) && string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }
            if (type.IsEnum)
            {
                return Convert.ChangeType(Enum.Parse(type, value.ToString(), true), type);
            }
            if (type == typeof(bool) && !string.IsNullOrEmpty(value.ToString()))
            {
                if (value.ToString().Equals("1"))
                {
                    return true;
                }
                if (value.ToString().Equals("true", StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
                return false;
            }
            return Convert.ChangeType(value, type);
        }

        /// <summary>
        /// 把数组转换为逗号分隔的字符串
        /// </summary>
        /// <param name="array">数组</param>
        /// <returns>返回逗号分隔的字符串</returns>
        public static string ConvertArrayToString(Array array)
        {
            StringBuilder sb = new StringBuilder();
            foreach (object item in array)
            {
                sb.Append(item);
                sb.Append(",");
            }
            return sb.ToString().TrimEnd(',');
        }

        /// <summary>
        /// 复制属性
        /// </summary>
        /// <param name="source">源对象</param>
        /// <param name="target">目标对象</param>
        public static void CopyProperty(object source, object target)
        {
            var sourceType = source.GetType();
            var pros = target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
            foreach (PropertyInfo info in pros)
            {
                string proName = info.Name;
                var sourcePro = sourceType.GetProperty(proName);
                if (sourcePro == null || !info.CanWrite)
                {
                    continue;
                }
                info.SetValue(target, ConvertObjectValue(sourcePro.GetValue(source, null), info.PropertyType), null);
            }
        }

        /// <summary>
        /// 把逗号分隔的字符串分隔成数组对象
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="str">逗号分隔的字符串</param>
        /// <returns>返回指定类型的数组</returns>
        public static Array ConvertStringToArray<T>(string str)
        {
            string[] sz = str.Split(',');
            List<T> list = new List<T>();
            foreach (string item in sz)
            {
                list.Add((T)ObjectHelper.ConvertObjectValue(item, typeof(T)));
            }
            return list.ToArray();
        }

        /// <summary>
        /// 检测此类型是否是可空类型
        /// </summary>
        /// <param name="type">测试的类型</param>
        /// <returns>如果是可空类型 返回true</returns>
        public static bool IsNullableType(Type type)
        {
            if (!type.IsValueType) return true;
            if (!type.UnderlyingSystemType.IsGenericType) return false;
            var def = type.UnderlyingSystemType.GetGenericTypeDefinition();
            if (def != null)
            {
                return def == typeof(Nullable<>);
            }
            return false;
        }

        /// <summary>
        /// 设置数据项的属性值
        /// </summary>
        /// <param name="item">数据项</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="propertyValue">属性值</param>
        public static void SetObjectValue(object item, string propertyName, object propertyValue)
        {
            if (item == null || string.IsNullOrEmpty(propertyName)) return;
            var row = item as DataRow;
            if (row != null)
            {
                row[propertyName] = propertyValue;
                return;
            }
            var propertyInfo = item.GetType().GetProperty(propertyName);
            if (propertyInfo == null) return;
            propertyInfo.SetValue(item, ConvertObjectValue(propertyValue, propertyInfo.PropertyType), null);

            //FastInvokeHandler fastInvokerGet = ReflectionHelper.GetFastInvoker(propertyInfo.GetSetMethod());
            //fastInvokerGet.Invoke(item, ObjectHelper.ConvertObjectValue(propertyValue, propertyInfo.PropertyType));
            //PropertyDescriptorx descriptor = TypeDescriptor.GetProperties(item).Find(propertyName, true);
            //if (descriptor == null) return;
            //if (!IsNullableType(descriptor.PropertyType) && propertyValue == null) return;
            //descriptor.SetValue(item, ObjectHelper.ConvertObjectValue(propertyValue, descriptor.PropertyType));
        }

        /// <summary>
        /// 设置数据项的属性值
        /// </summary>
        /// <param name="type">数据类</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="propertyValue">属性值</param>
        public static void SetObjectValue(Type type, string propertyName, object propertyValue)
        {
            if (string.IsNullOrEmpty(propertyName)) return;
            var propertyInfo = type.GetProperty(propertyName);
            if (propertyInfo == null) return;
            propertyInfo.SetValue(null, ConvertObjectValue(propertyValue, propertyInfo.PropertyType), null);
            //FastInvokeHandler fastInvokerGet = ReflectionHelper.GetFastInvoker(propertyInfo.GetSetMethod());
            //fastInvokerGet.Invoke(null, ConvertObjectValue(propertyValue, propertyInfo.PropertyType));
            //PropertyDescriptorx descriptor = TypeDescriptor.GetProperties(item).Find(propertyName, true);
            //if (descriptor == null) return;
            //if (!IsNullableType(descriptor.PropertyType) && propertyValue == null) return;
            //descriptor.SetValue(item, ObjectHelper.ConvertObjectValue(propertyValue, descriptor.PropertyType));
        }

        /// <summary>
        /// 设置对象属性
        /// </summary>
        /// <param name="item">对象实例</param>
        /// <param name="propertyDic">属性字典,键存放属性名称 值存放属性值</param>
        public static void SetObjectValue(object item, Dictionary<string, string> propertyDic)
        {
            if (propertyDic.Count == 0) return;
            var pros = item.GetType().GetProperties();
            foreach (PropertyInfo info in pros)
            {
                if (info.CanWrite)
                {
                    string proName = info.Name;
                    if (!propertyDic.ContainsKey(proName)) continue;
                    object proValue = ObjectHelper.ConvertObjectValue(propertyDic[proName], info.PropertyType);
                    if (propertyDic.ContainsKey(proName))
                    {
                        if (!ObjectHelper.IsNullableType(info.PropertyType) && proValue == null) continue;
                        info.SetValue(item, proValue, null);
                    }
                }
            }
        }

        /// <summary>
        /// 获取数据项的属性值
        /// </summary>
        /// <param name="item">数据项</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns>返回指定属性的值</returns>
        public static object GetObjectValue(object item, string propertyName)
        {
            if (item == null || string.IsNullOrEmpty(propertyName)) return null;
            var row = item as DataRow;
            if (row != null)
            {
                return row[propertyName];
            }
            var propertyInfo = item.GetType().GetProperty(propertyName);
            if (propertyInfo == null) return null;
            return propertyInfo.GetValue(item, null);
            //FastInvokeHandler fastInvokerGet = ReflectionHelper.GetFastInvoker(propertyInfo.GetGetMethod());
            //return fastInvokerGet.Invoke(item);
            //PropertyDescriptorx descriptor = TypeDescriptor.GetProperties(item).Find(propertyName, true);
            //if (descriptor == null) return null;
            //return descriptor.GetValue(item);
        }

        /// <summary>
        /// 获取数据项的属性值
        /// </summary>
        /// <param name="type">数据类</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns>返回指定属性的值</returns>
        public static object GetObjectValue(Type type, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return null;
            var propertyInfo = type.GetProperty(propertyName);
            if (propertyInfo == null) return null;
            return propertyInfo.GetValue(null, null);
            //FastInvokeHandler fastInvokerGet = ReflectionHelper.GetFastInvoker(propertyInfo.GetGetMethod());
            //return fastInvokerGet.Invoke(null);
            //PropertyDescriptorx descriptor = TypeDescriptor.GetProperties(item).Find(propertyName, true);
            //if (descriptor == null) return null;
            //return descriptor.GetValue(item);
        }

        /// <summary>
        /// 获取对象属性值
        /// </summary>
        /// <param name="item">对象实例</param>
        /// <returns>返回属性\值字典</returns>
        public static Dictionary<string, string> GetObjectValue(object item)
        {
            Dictionary<string, string> propertyDic = new Dictionary<string, string>();
            var pros = item.GetType().GetProperties();
            foreach (PropertyInfo info in pros)
            {
                if (info.CanRead)
                {
                    string proName = info.Name;
                    object proValueObject = info.GetValue(item, null);
                    string proValue = string.Empty;
                    if (proValueObject != null)
                    {
                        proValue = proValueObject.ToString();
                    }
                    propertyDic.Add(proName, proValue);
                }
            }
            return propertyDic;
        }

        /// <summary>
        /// 获取对象属性名称/属性值 键值对
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>返回属性名称/属性值字典</returns>
        public static Dictionary<string, object> GetPropertyKeyValuePair(object obj)
        {
            var propertyDic = new Dictionary<string, object>();
            if (obj == null)
            {
                return propertyDic;
            }
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(obj))
            {
                propertyDic.Add(propertyDescriptor.Name, propertyDescriptor.GetValue(obj));
            }
            return propertyDic;
        }

        /// <summary>
        /// 获取对象属性名称数组
        /// </summary>
        /// <param name="obj">对象</param>
        public static string[] GetPropertyNames(object obj)
        {
            if (obj == null) return null;
            List<string> list = new List<string>();
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(obj))
            {
                list.Add(propertyDescriptor.Name);
            }
            return list.ToArray();
        }

        #region 属性

        /// <summary>
        /// 设置对象属性
        /// </summary>
        /// <param name="instance">对象实例</param>
        /// <param name="propertyStrings">属性名称</param>
        /// <param name="propertyValue">属性值</param>
        public static void SetObjectPropertys(object instance, string propertyStrings, object propertyValue)
        {
            if (propertyStrings.Length <= 0)
            {
                return;
            }
            string[] propertys = propertyStrings.Split(';');
            foreach (string property in propertys)
            {
                SetObjectProperty(instance, property, propertyValue);
            }
        }

        /// <summary>
        /// 设置对象属性
        /// </summary>
        /// <param name="instance">对象实例</param>
        /// <param name="colls">值名对集合</param>
        public static void SetObjectPropertys(object instance, NameValueCollection colls)
        {
            if (colls.Count <= 0)
            {
                return;
            }
            var pros = instance.GetType().GetProperties();
            foreach (var pro in pros)
            {
                var name = pro.Name;
                var value = colls[name];
                try
                {
                    pro.SetValue(instance, ConvertObjectValue(value, pro.PropertyType));
                }
                catch (Exception ex)
                {
                    var ss = name;
                    var sss = value;
                }
            }
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="instance">对象实例</param>
        /// <param name="propertyString">属性字符串</param>
        /// <param name="propertyValue">属性值</param>
        public static void SetObjectProperty(object instance, string propertyString, object propertyValue)
        {
            if (propertyString.Length <= 0)
            {
                return;
            }

            string[] propertys = propertyString.Split('.');
            if (propertys.Length == 0) return;
            PropertyDescriptor proDesc = TypeDescriptor.GetProperties(instance).Find(propertys[0], true);
            object tempObject = instance;
            for (int i = 1; i < propertys.Length; i++)
            {
                string propName = propertys[i];
                if (tempObject == null || proDesc == null) break;
                tempObject = proDesc.GetValue(tempObject);
                proDesc = proDesc.GetChildProperties().Find(propName, true);
                //Type objectType = tempObject.GetType();
                //PropertyInfo propertyInfo;
                //if (i == propertys.Length - 1)
                //{
                //    propertyInfo = GetPropertyInfo(objectType, propName);
                //    if (null != propertyInfo && propertyInfo.CanWrite)
                //    {
                //        propertyInfo.SetValue(tempObject, GetObjectFromString(propertyValue.ToString(), propertyInfo.PropertyType), null);
                //    }
                //}
                //else
                //{
                //    propertyInfo = GetPropertyInfo(objectType, propName);
                //    if (null != propertyInfo)
                //    {
                //        tempObject = propertyInfo.GetValue(tempObject, null);
                //    }
                //}
            }
            if (proDesc != null && tempObject != null)
            {
                proDesc.SetValue(tempObject, GetObjectFromString(propertyValue.ToString(), proDesc.PropertyType));
            }
        }


        /// <summary>
        /// 获取属性对象
        /// </summary>
        /// <param name="instance">对象实例</param>
        /// <param name="propertyStrings">属性字符串</param>
        public static object GetObjectPropertys(object instance, string propertyStrings)
        {
            if (propertyStrings.Length <= 0)
            {
                return null;
            }
            string[] propertys = propertyStrings.Split(';');
            foreach (string property in propertys)
            {
                return GetObjectProperty(instance, property);
            }
            return null;
        }

        /// <summary>
        /// 获取属性对象
        /// </summary>
        /// <param name="instance">对象实例</param>
        /// <param name="propertyString">属性字符串</param>
        public static object GetObjectProperty(object instance, string propertyString)
        {
            if (propertyString.Length <= 0)
            {
                return null;
            }
            string[] propertys = propertyString.Split('.');
            if (propertys.Length == 0) return null;
            object tempObject = instance;
            PropertyDescriptor proDesc = TypeDescriptor.GetProperties(tempObject).Find(propertys[0], true);

            for (int i = 1; i < propertys.Length; i++)
            {
                if (tempObject == null || proDesc == null) break;
                string propName = propertys[i];
                tempObject = proDesc.GetValue(tempObject);
                proDesc = proDesc.GetChildProperties().Find(propName, true);

                //Type objectType = tempObject.GetType();
                //PropertyInfo propertyInfo;
                //if (i == propertys.Length - 1)
                //{
                //    propertyInfo = GetPropertyInfo(objectType, propName);
                //    if (null != propertyInfo && propertyInfo.CanRead)
                //    {
                //        return propertyInfo.GetValue(tempObject, null);
                //    }
                //}
                //else
                //{
                //    propertyInfo = GetPropertyInfo(objectType, propName);
                //    if (null != propertyInfo)
                //    {
                //        tempObject = propertyInfo.GetValue(tempObject, null);
                //    }
                //}
            }
            if (proDesc != null && tempObject != null)
            {
                return proDesc.GetValue(tempObject);
            }
            return null;
        }

        ///// <summary>
        ///// 获取对象快速属性
        ///// </summary>
        ///// <param name="objectType">对象类型</param>
        ///// <param name="propName">属性名称</param>
        //private static PropertyInfo GetPropertyInfo(Type objectType, string propName)
        //{
        //    PropertyInfo property = null;
        //    try
        //    {
        //        property = objectType.GetProperty(propName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.DeclaredOnly);
        //    }
        //    catch (AmbiguousMatchException)
        //    {
        //    }

        //    if (property == null)
        //    {
        //        try
        //        {
        //            property = objectType.GetProperty(propName);
        //        }
        //        catch (AmbiguousMatchException)
        //        {
        //        }
        //    }
        //    return property;
        //}

        #endregion
    }
}
