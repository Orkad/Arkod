using System;
using System.Collections.Generic;
using System.Text;

namespace Arkod.Extensions
{
    /// <summary>
    /// Defines extensions method on strings
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns a new string in which all occurences of a specified string in this instance are replaced with a runtime determined string 
        /// </summary>
        /// <param name="oldValue">the string to be replaced</param>
        /// <param name="newValue">the function used to replace the string called one time per occurence</param>
        public static string Replace(this string text, string oldValue, Func<string> newValue)
        {
            var replacedText = new StringBuilder();
            int pos = text.IndexOf(oldValue);
            while (pos >= 0)
            {
                var nv = newValue();
                replacedText.Append(text.Substring(0, pos) + nv);
                text = text.Substring(pos + oldValue.Length);
                pos = text.IndexOf(oldValue);
            }
            return replacedText + text;
        }
    }
}
