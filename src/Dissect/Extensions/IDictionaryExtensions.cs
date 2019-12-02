using System;
using System.Collections.Generic;

namespace Dissect.Extensions.IDictionaryExtensions
{
    public static class IDictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> addValueFactory, out bool containedKey)
        {
            TValue value;
            containedKey = dictionary.TryGetValue(key, out value);
            if (containedKey) { return value; }
            value = addValueFactory();
            dictionary.Add(key, value);
            return value;
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> addValueFactory)
        {
            return dictionary.GetOrAdd(key, addValueFactory, out _);
        }

        public static TValue GetWithFallback<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> fallbackValue)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : fallbackValue();
        }

        public static void AddIfNotContainsKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue addValue)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, addValue);
            }
        }

        public static TValue GetOrAddNewIfNotContainsKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : new()
        {
            return dictionary.GetOrAdd(key, () => new TValue());
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

        public static void SetIfContainsKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
        }
    }
}