using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;

namespace Paracord.Shared.Extensions
{
    public static class NameValueCollectionExtensions
    {
        /// <summary>
        /// Determines whether the <see cref="NameValueCollection" /> contains the specified value.
        /// </summary>
        /// <param name="collection">The collection to operate on.</param>
        /// <param name="key">The key to locate.</param>
        /// <returns><c>true</c> if the <see cref="NameValueCollection" /> contains the value. Otherwise, false.</returns>
        public static bool ContainsKey(this NameValueCollection collection, string key)
            => collection.Get(key) != null;

        /// <summary>
        /// Determines whether the <see cref="NameValueCollection" /> contains the specified value.
        /// </summary>
        /// <param name="collection">The collection to operate on.</param>
        /// <param name="value">The value to locate.</param>
        /// <returns><c>true</c> if the <see cref="NameValueCollection" /> contains the value. Otherwise, false.</returns>
        public static bool ContainsValue(this NameValueCollection collection, string value)
        {
            foreach(string? element in collection.AllKeys)
            {
                if(element != null && element == value)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="collection">The collection to operate on.</param>
        /// <param name="key">The key to locate.</param>
        /// <param name="value">When this method returns true, contains the value at the specified key. Otherwise, null.</param>
        /// <returns><c>true</c> if the <see cref="NameValueCollection" /> contains the value. Otherwise, default.</returns>
        public static bool TryGetValue(this NameValueCollection collection, string key, [NotNullWhen(true)] out string value)
        {
            string? _value = collection.Get(key);

            if(_value == null)
            {
                value = default!;
                return false;
            }

            value = _value;
            return true;
        }
    }
}