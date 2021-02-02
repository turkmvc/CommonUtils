using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue GetValueDirect<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
        {
            if (dictionary.TryGetValue(key, out TValue outValue))
            {
                return outValue;
            }
            return defaultValue;

        }
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }

        }
    }
}
