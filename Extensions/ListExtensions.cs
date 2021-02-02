using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.Extensions
{
    public static class ListExtensions
    {

        
        public static void AddRangeUntil<T>(this IList<T> inner, IEnumerable<T> items, Func<IList<T>, T, bool> predicate = null)
        {
            foreach (var item in items)
            {
                if(predicate == null)
                {
                    inner.Add(item);
                    continue;
                }
                if (predicate(inner, item)) break;
                inner.Add(item);
            }
        }
        public static void AddRangeLoop<T>(this IList<T> inner, IEnumerable<T> items, Func<IList<T>, T, bool> predicate = null)
        {
            foreach (var item in items)
            {
                if (predicate != null && !predicate(inner, item)) continue;
                inner.Add(item);
            }
        }
    }
}
