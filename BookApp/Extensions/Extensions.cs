using System.Collections.Generic;

namespace BookApp.Extensions;

public static partial class Extensions
{
    public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
        TValue obj;

        if (dictionary.TryGetValue(key, out obj))
        {
            return obj;
        }
        
        return default(TValue);
    }
}