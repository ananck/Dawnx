﻿
namespace Dawnx
{
    public static class DawnUInt64
    {
        /// <summary>
        /// Returns whether the specified number is odd.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static bool IsOdd(this ulong @this) => (@this & 1) == 1;

        /// <summary>
        /// Returns whether the specified number is even.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static bool IsEven(this ulong @this) => (@this & 1) == 0;

        /// <summary>
        /// Gets the positive integer modulus. (Unlike the operator %, this method always returns a positive number)
        /// </summary>
        /// <param name="this"></param>
        /// <param name="mod"></param>
        /// <returns></returns>
        public static ulong Mod(this ulong @this, ulong mod)
        {
            if (@this < 0)
                return @this % mod + mod;
            else return @this % mod;
        }

    }
}
