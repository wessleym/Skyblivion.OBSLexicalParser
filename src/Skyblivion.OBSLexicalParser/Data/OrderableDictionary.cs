using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Data
{
    class OrderableDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> where TKey : notnull where TValue: class
    {
        private readonly Dictionary<TKey, TValue> dictionary;
        private List<TKey> orderedKeys;
        public OrderableDictionary()
        {
            dictionary = new Dictionary<TKey, TValue>();
            orderedKeys = new List<TKey>();
        }
        public OrderableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> dictionary)
            : this()
        {
            foreach (var kvp in dictionary)
            {
                this.dictionary.Add(kvp.Key, kvp.Value);
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                return dictionary[key];
            }
        }

        public bool TryGetValue(TKey key, out TValue? value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        public void Add(TKey key, TValue value)
        {
            orderedKeys.Add(key);
            dictionary.Add(key, value);
        }

        public void OrderBy<T>(Func<KeyValuePair<TKey, TValue>, T> selector)
        {
            orderedKeys = dictionary.OrderBy(selector).Select(kvp => kvp.Key).ToList();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return orderedKeys.Select(k=>new KeyValuePair<TKey, TValue>(k, dictionary[k])).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
