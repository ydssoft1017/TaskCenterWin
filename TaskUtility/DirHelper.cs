using System;
using System.IO;
using System.Web;
namespace TaskUtility
{
    /// <summary>
    /// 应用程序路径操作类
    /// </summary>
    public static class DirHelper
    {

        /// <summary>
        /// 项目根目录,对于Web项目是网站根目录,对于Win项目是启动exe所在目录
        /// </summary>
        public static string RootDirectory
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }

        /// <summary>
        /// 类库输出目录,对于Web项目是网站bin目录,对于Win项目是启动exe所在目录
        /// </summary>
        public static string OutDirectory
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
            }
        }

        /// <summary>
        /// 比较目录路径是否相同
        /// </summary>
        /// <param name="dir1">目录1</param>
        /// <param name="dir2">目录2</param>
        /// <returns>如果是同一个目录,返回true</returns>
        public static bool EqualsDirectory(string dir1, string dir2)
        {
            var d1 = new DirectoryInfo(dir1);
            var d2 = new DirectoryInfo(dir2);
            return d1.Name.Equals(d2.Name);
        }

        /// <summary>
        /// 获取根目录下指定名称的目录路径
        /// </summary>
        /// <param name="dirName">目录名称</param>
        public static string GetRootDirectory(string dirName)
        {
            return Path.Combine(RootDirectory, dirName);
        }

        /// <summary>
        /// 获取根目录下指定名称的文件路径
        /// </summary>
        /// <param name="fileName">文件名称</param>
        public static string GetRootFilePath(string fileName)
        {
            return Path.Combine(RootDirectory, fileName);
        }

        /// <summary>
        /// 获取根目录下指定目录和指定名称的文件路径
        /// </summary>
        /// <param name="dirName">目录名称</param>
        /// <param name="fileName">文件名称</param>
        public static string GetRootFilePath(string dirName, string fileName)
        {
            return Path.Combine(RootDirectory, dirName, fileName);
        }

        /// <summary>
        /// 获取类库输出目录下指定名称的目录路径
        /// </summary>
        /// <param name="dirName">目录名称</param>
        public static string GetOutDirectory(string dirName)
        {
            return Path.Combine(OutDirectory, dirName);
        }

        /// <summary>
        /// 获取类库输出目录下指定名称的文件路径
        /// </summary>
        /// <param name="fileName">文件名称</param>
        public static string GetOutFilePath(string fileName)
        {
            return Path.Combine(OutDirectory, fileName);
        }

        /// <summary>
        /// 获取类库输出目录下指定目录和指定名称的文件路径
        /// </summary>
        /// <param name="dirName">目录名称</param>
        /// <param name="fileName">文件名称</param>
        public static string GetOutFilePath(string dirName, string fileName)
        {
            return Path.Combine(OutDirectory, dirName, fileName);
        }

    }
}
