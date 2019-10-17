﻿using System;
using System.Linq;

namespace Dawnx
{
    public static partial class DawnArray
    {
        /// <summary>
        /// Gets the index of the first element in the array.
        ///     Usually, LBound() returns 0, since arrays are zero-based by default.
        ///     but in some rare cases they are not.
        ///     For example, you use Array.CreateInstance(Type, int[], int[]) to create an Array.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static int LBound(this Array @this) => @this.GetLowerBound(0);

        /// <summary>
        /// Gets the index of the last element in the array.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static int UBound(this Array @this) => @this.GetUpperBound(0);

        /// <summary>
        /// Use a method to initailize each element of an array.
        /// </summary>
        /// <typeparam name="TSelf"></typeparam>
        /// <param name="this"></param>
        /// <param name="initMethod"></param>
        /// <returns></returns>
        public static TSelf[] Init<TSelf>(this TSelf[] @this, Func<int, TSelf> initMethod)
        {
            int i = 0;
            foreach (var item in @this)
                @this[i] = initMethod(i);
            return @this;
        }

        /// <summary>
        /// Do a task for itself.
        /// </summary>
        /// <typeparam name="TSelf"></typeparam>
        /// <param name="this"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        public static TSelf[] Self<TSelf>(this TSelf[] @this, Action<TSelf[], int> task)
        {
            int i = 0;
            foreach (var item in @this)
                task(@this, i++);
            return @this;
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified element in this array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static int IndexOf<T>(this T[] @this, T element)
        {
            int i = 0;
            foreach (var e in @this)
            {
                if (e.Equals(element))
                    return i;
                i++;
            }
            return -1;
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified element in this array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static int IndexOf<T>(this T[] @this, Func<T, bool> predicate)
        {
            int i = 0;
            foreach (var e in @this)
            {
                if (predicate(e))
                    return i;
                i++;
            }
            return -1;
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified element in this string.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="predicate"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static int IndexOf<T>(this T[] @this, Func<T, bool> predicate, int startIndex)
        {
            int i = startIndex;
            foreach (var e in @this.Skip(startIndex))
            {
                if (predicate(e))
                    return i;
                i++;
            }
            return -1;
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified element in this string.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="predicate"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static int IndexOf<T>(this T[] @this, Func<T, bool> predicate, int startIndex, int count)
        {
            int i = startIndex;
            int iEnd = startIndex + count;
            if (iEnd < 1) return -1;

            foreach (var e in @this.Skip(startIndex))
            {
                if (predicate(e))
                    return i;
                i++;
                if (i == iEnd) break;
            }
            return -1;
        }

        /// <summary>
        /// Retrieves an array from this instance. The new array starts at a specified
        ///     element position and continues to the end of the array.
        ///     (If the parameter is negative, the search will start on the right.)
        /// </summary>
        /// <param name="this"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static T[] Slice<T>(this T[] @this, int start) => Slice(@this, start, @this.Length);

        /// <summary>
        /// Retrieves an array from this instance. The new array starts at a specified
        ///     element position and ends with a specified element position.
        ///     (If the parameters is negative, the search will start on the right.)
        /// </summary>
        /// <param name="this"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        public static T[] Slice<T>(this T[] @this, int start, int stop)
        {
            start = GetElementPosition(ref @this, start);
            stop = GetElementPosition(ref @this, stop);

            var length = stop - start;
            if (length > 0)
            {
                var ret = new T[length];
                Array.Copy(@this, start, ret, 0, length);
                return ret;
            }
            else if (length == 0) return new T[0];
            else throw new IndexOutOfRangeException($"'{nameof(start)}:{start}' can not greater than '{nameof(stop)}:{stop}'.");
        }
        private static int GetElementPosition<T>(ref T[] str, int pos) => pos < 0 ? str.Length + pos : pos;

        /// <summary>
        /// Shuffles a array and returns itself.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static T[] Shuffle<T>(this T[] @this)
        {
            var length = @this.Length;
            var random = new Random();

            for (int i = 0; i < length; i++)
            {
                var rnd = random.Next(length);
                var take = @this[i];
                @this[i] = @this[rnd];
                @this[rnd] = take;
            }
            return @this;
        }

    }
}
