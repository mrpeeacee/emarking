using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Saras.eMarking.Domain.Extensions
{
    public static class DictionaryExtensions
    {
        public static void Trim(this HashSet<string> items, Predicate<string> itemsToRemove, Predicate<string> itemsToAlwaysInclude, int maxLength)
        {
            if (items == null)
                return;

            items.RemoveWhere(itemsToRemove);
            if (maxLength > 0 && items.Count > maxLength)
            {
                foreach (string item in items.ToList())
                {
                    if (items.Count <= maxLength)
                        break;

                    if (itemsToAlwaysInclude(item))
                        continue;

                    items.Remove(item);
                }
            }
        }

        public static void AddItemIfNotEmpty(this IDictionary<string, string> dictionary, string key, string value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (!String.IsNullOrEmpty(value))
                dictionary[key] = value;
        }

        /// <summary>
        /// Adds or overwrites the existing value.
        /// </summary>
        public static void AddOrUpdate<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            dictionary.AddOrUpdate(key, value, (oldkey, oldvalue) => value);
        }

        public static bool ContainsKeyWithValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, params TValue[] values)
        {
            if (dictionary == null || values == null || values.Length == 0)
                return false;

            TValue temp;
            try
            {
                if (!dictionary.TryGetValue(key, out temp))
                    return false;
            }
            catch (ArgumentNullException)
            {
                return false;
            }

            return values.Any(v => v.Equals(temp));
        }

        public static TValue TryGetAndReturn<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (!dictionary.TryGetValue(key, out var value))
                value = default;

            return value;
        }

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            dictionary.TryGetValue(key, out var obj);
            return obj;
        }

        public static bool CollectionEquals<TValue>(this IDictionary<string, TValue> source, IDictionary<string, TValue> other)
        {
            if (source.Count != other.Count)
                return false;

            foreach (string key in source.Keys)
            {
                var sourceValue = source[key];

                if (!other.TryGetValue(key, out var otherValue))
                    return false;

                if (sourceValue.Equals(otherValue))
                    return false;
            }

            return true;
        }


        public static int GetCollectionHashCode<TValue>(this IDictionary<string, TValue> source, IList<string> exclusions = null)
        {
            string assemblyQualifiedName = typeof(TValue).AssemblyQualifiedName;
            int hashCode = assemblyQualifiedName?.GetHashCode() ?? 0;

            var keyValuePairHashes = new List<int>(source.Keys.Count);

            foreach (string key in source.Keys.OrderBy(x => x))
            {
                if (exclusions != null && exclusions.Contains(key))
                    continue;

                var item = source[key];
                unchecked
                {
                    int kvpHash = key.GetHashCode();
                    kvpHash = (kvpHash * 397) ^ item.GetHashCode();
                    keyValuePairHashes.Add(kvpHash);
                }
            }

            keyValuePairHashes.Sort();
            foreach (int kvpHash in keyValuePairHashes)
            {
                unchecked
                {
                    hashCode = (hashCode * 397) ^ kvpHash;
                }
            }

            return hashCode;
        }

        public static T GetValueOrDefault<T>(this IDictionary<string, string> source, string key, T defaultValue = default)
        {
            if (!source.ContainsKey(key))
                return defaultValue;

            object data = source[key];
            if (data is T variable)
                return variable;

            if (data == null)
                return defaultValue;

            return data.ToType<T>();

        }

        public static string GetString(this IDictionary<string, string> source, string name)
        {
            return source.GetString(name, String.Empty);
        }

        public static string GetString(this IDictionary<string, string> source, string name, string @default)
        {
            if (!source.TryGetValue(name, out string value))
                return @default;

            return value ?? @default;
        }

        public static bool IsNumeric(this Type type)
        {
            if (type.IsArray)
                return false;

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Empty:
                    break;
                case TypeCode.Object:
                    break;
                case TypeCode.DBNull:
                    break;
                case TypeCode.Boolean:
                    break;
                case TypeCode.Char:
                    break;
                case TypeCode.DateTime:
                    break;
                case TypeCode.String:
                    break;
            }

            return false;
        }
        public static T ToType<T>(this object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var targetType = typeof(T);
            var converter = TypeDescriptor.GetConverter(targetType);
            var valueType = value.GetType();

            if (targetType.IsAssignableFrom(valueType))
                return (T)value;

            if ((valueType.IsEnum || value is string) && targetType.IsEnum)
            {
                // attempt to match enum by name.
                if (EnumHelper.TryEnumIsDefined(targetType, value.ToString()))
                {
                    object parsedValue = Enum.Parse(targetType, value.ToString(), false);
                    return (T)parsedValue;
                }

                string message = $"The Enum value of '{value}' is not defined as a valid value for '{targetType.FullName}'.";
                throw new ArgumentException(message);
            }

            if (valueType.IsNumeric() && targetType.IsEnum)
                return (T)Enum.ToObject(targetType, value);

            if (converter != null && converter.CanConvertFrom(valueType))
            {
                object convertedValue = converter.ConvertFrom(value);
                return (T)convertedValue;
            }

            if (value is IConvertible)
            {
                try
                {
                    object convertedValue = Convert.ChangeType(value, targetType);
                    return (T)convertedValue;
                }
                catch (Exception e)
                {
                    throw new ArgumentException($"An incompatible value specified.  Target Type: {targetType.FullName} Value Type: {value.GetType().FullName}", nameof(value), e);
                }
            }

            throw new ArgumentException($"An incompatible value specified.  Target Type: {targetType.FullName} Value Type: {value.GetType().FullName}", nameof(value));
        }

    }
}
