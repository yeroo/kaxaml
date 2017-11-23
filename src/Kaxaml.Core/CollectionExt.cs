using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Kaxaml.Core
{
    /// <summary>
    /// This class provides some helper methods for Collections. -> PixelLab
    /// https://github.com/thinkpixellab/bot
    /// </summary>
    public static class CollectionExt
    {
        public static TValue EnsureItem<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueFactory)
        {
            Contract.Requires(dictionary != null);
            Contract.Requires(valueFactory != null);
            TValue value;
            if (!dictionary.TryGetValue(key, out value))
            {
                value = valueFactory();
                dictionary.Add(key, value);
            }
            return value;
        }
    }
}