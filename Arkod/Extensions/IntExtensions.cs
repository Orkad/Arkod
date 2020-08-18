using System.Collections.Generic;
using System.Text;

namespace Arkod.Extensions
{
    public static class IntExtensions
    {
        /// <summary>
        /// Get each digit composing the number from left to right
        /// </summary>
        /// <example>423594340 => 4,2,3,5,9,4,3,4,0</example>
        public static byte[] GetDigits(this int number)
        {
            var numbers = new Stack<byte>();
            for (; number > 0; number /= 10)
            {
                numbers.Push((byte)(number % 10));
            }
            return numbers.ToArray();
        }
    }
}
