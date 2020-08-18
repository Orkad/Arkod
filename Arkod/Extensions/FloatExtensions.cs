using System;

namespace Arkod.Extensions
{
    public static class FloatExtensions
    {
        /// <summary>
        /// Get the fractionnal part of the number
        /// </summary>
        /// <example>3.14 => 0.14</example>
        public static double GetFractionalPart(this double number)
        {
            return (number - Math.Truncate(number));
        }

        /// <summary>
        /// Get the integer part of the number
        /// </summary>
        /// <example>3.14 => 3</example>
        public static int GetIntegerPart(this double number)
        {
            return (int)Math.Truncate(number);
        }
    }
}
