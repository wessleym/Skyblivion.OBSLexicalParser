using System;
using System.Collections.Generic;

namespace Dissect.Extensions
{
    public static class IDictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> addValueFactory, out bool containedKey)
        {
            TValue value;
            if (dictionary.TryGetValue(key, out value))
            {
                containedKey = true;
                return value;
            }
            containedKey = false;
            value = addValueFactory();
            dictionary.Add(key, value);
            return value;
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> addValueFactory)
        {
            return dictionary.GetOrAdd(key, addValueFactory, out _);
        }

        public static List<TValue> AddNewListIfNotContainsKey<TKey, TValue>(this IDictionary<TKey, List<TValue>> dictionary, TKey key)
        {
            return dictionary.GetOrAdd(key, () => new List<TValue>());
        }

        public static void AddNewListIfNotContainsKeyAndAddValueToList<TKey, TValue>(this IDictionary<TKey, List<TValue>> dictionary, TKey key, TValue valueToAdd)
        {
            List<TValue> list = dictionary.AddNewListIfNotContainsKey(key);
            list.Add(valueToAdd);
        }
    }
}