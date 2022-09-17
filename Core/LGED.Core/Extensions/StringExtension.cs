using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LGED.Core.Extensions
{
    public static class StringExtension
    {
        public static string ToCamelCase(this string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > 1)
            {
                return char.ToLowerInvariant(str[0]) + str.Substring(1);
            }
            return str;
        }

        public static T ToObject<T>(this IDictionary<string, object> source)
            where T : class, new()
        {
            var someObject = new T();
            var someObjectType = someObject.GetType();

            foreach (var item in source)
            {
                var fieldType = someObjectType.GetProperty(item.Key)?.PropertyType;

                if (fieldType == typeof(DateTime?) || fieldType == typeof(DateTime))
                {
                    DateTime.TryParse(item.Value.ToString(), out var v);
                    someObjectType.GetProperty(item.Key)?.SetValue(someObject, v, null);
                }
                else if (fieldType == typeof(int?) || fieldType == typeof(int))
                {
                    int.TryParse(item.Value.ToString(), out var v);
                    someObjectType.GetProperty(item.Key)?.SetValue(someObject, v, null);
                }
                else if (fieldType == typeof(decimal?) || fieldType == typeof(decimal))
                {
                    decimal.TryParse(item.Value.ToString(), out var v);
                    someObjectType.GetProperty(item.Key)?.SetValue(someObject, v, null);
                }
                else if (fieldType == typeof(bool?) || fieldType == typeof(bool))
                {
                    bool.TryParse(item.Value.ToString(), out var v);
                    someObjectType.GetProperty(item.Key)?.SetValue(someObject, v, null);
                }
                else if (fieldType == typeof(Guid?) || fieldType == typeof(Guid))
                {
                    Guid.TryParse(item.Value.ToString(), out var v);
                    someObjectType.GetProperty(item.Key)?.SetValue(someObject, v, null);
                }
                else if (fieldType == typeof(string))
                {
                    someObjectType.GetProperty(item.Key)?.SetValue(someObject, item.Value?.ToString(), null);
                }
                else
                {
                    someObjectType
                        .GetProperty(item.Key)
                        ?.SetValue(someObject, item.Value, null);
                }
            }

            return someObject;
        }

         public static string ReplaceLastOccurrence(this string source, string find, string replace)
        {
            var place = source.LastIndexOf(find, StringComparison.Ordinal);

            if (place == -1)
                return source;

            var result = source.Remove(place, find.Length).Insert(place, replace);
            return result;
        }

         public static IDictionary<string, object> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );
        }

         public static DateTime ToDateTime(this string value)
            => DateTime.TryParse(value, out var result) ? result : default;

        public static short ToInt16(this string value)
            => short.TryParse(value, out var result) ? result : default;

        public static int ToInt32(this string value)
            => int.TryParse(value, out var result) ? result : default;

        public static long ToInt64(this string value)
            => long.TryParse(value, out var result) ? result : default;

        public static bool ToBoolean(this string value)
            => bool.TryParse(value, out var result) ? result : default;

        public static float ToFloat(this string value)
            => float.TryParse(value, out var result) ? result : default;

        public static decimal ToDecimal(this string value)
            => decimal.TryParse(value, out var result) ? result : default;

        public static double ToDouble(this string value)
            => double.TryParse(value, out var result) ? result : default;

        public static bool IsNumber(this string value)
            => Regex.IsMatch(value, @"^\d+$");

        public static bool IsWholeNumber(this string value)
            => long.TryParse(value, out _);

        public static bool IsDecimalNumber(this string value)
            => decimal.TryParse(value, out _);

        public static bool IsBoolean(this string value)
            => bool.TryParse(value, out var _);
        
    }
}