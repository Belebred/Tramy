using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tramy.Backend.Extensions
{
    /// <summary>
    /// Extensions to string
    /// </summary>
    internal static class StringExtensions
    {
        /// <summary>
        /// Make text camelCase
        /// </summary>
        /// <param name="value">Text to convert</param>
        /// <returns>Camel case text></returns>
        internal static string ToCamelCase(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return char.ToLowerInvariant(value[0]) + value.Substring(1);
        }
    }
}
