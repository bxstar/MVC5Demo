using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iclickpro.AccessCommon
{
    /// <summary>
    /// 扩展方法类
    /// </summary>
    public static class ExtendMethod
    {

        /// <summary>
        /// 基本上和List<T>的ForEach方法一致。
        /// </summary>
        public static void Each<T>(this IEnumerable<T> col, Action<T> handler)
        {
            foreach (var item in col)
                handler(item);
        }
        /// <summary>
        /// 带索引的遍历方法。
        /// </summary>
        public static void Each<T>(this IEnumerable<T> col, Action<T, int> handler)
        {
            int index = 0;
            foreach (var item in col)
                handler(item, index++);
        }
        /// <summary>
        /// 可以半途中断执行的遍历方法。
        /// </summary>
        public static void Each<T>(this IEnumerable<T> col, Func<T, bool> handler)
        {
            foreach (var item in col)
                if (!handler(item)) break;
        }
        /// <summary>
        /// 可以半途中段的带索引的遍历方法。
        /// </summary>
        public static void Each<T>(this IEnumerable<T> col, Func<T, int, bool> handler)
        {
            int index = 0;
            foreach (var item in col)
                if (!handler(item, index++)) break;
        }


        #region 以下为IEnumerable<T>的非泛型实现
        public static void Each<T>(this IEnumerable col, Action<object> handler)
        {
            foreach (var item in col)
                handler(item);
        }
        public static void Each<T>(this IEnumerable col, Action<object, int> handler)
        {
            int index = 0;
            foreach (var item in col)
                handler(item, index++);
        }
        public static void Each<T>(this IEnumerable col, Func<object, bool> handler)
        {
            foreach (var item in col)
                if (!handler(item)) break;
        }
        public static void Each<T>(this IEnumerable col, Func<object, int, bool> handler)
        {
            int index = 0;
            foreach (var item in col)
                if (!handler(item, index++)) break;
        }
        #endregion


        /// <summary>
        /// 给非强类型的IEnumerable返回头一个元素。
        /// </summary>
        public static object First(this IEnumerable col)
        {
            foreach (var item in col)
                return item;
            throw new IndexOutOfRangeException();
        }

    }
}
