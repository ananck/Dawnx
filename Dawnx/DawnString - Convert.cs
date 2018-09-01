﻿using System;
using System.Net;
using System.Text;

namespace Dawnx
{
    public static partial class DawnString
    {
        /// <summary>
        /// Encodes all the characters in the specified string into a sequence of bytes (Unicode, UTF-16), then returns it.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this string @this)
            => GetBytes(@this, Encoding.Unicode);

        /// <summary>
        /// Encodes all the characters in the specified string into a sequence of bytes, then returns it.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this string @this, Encoding encoding)
            => encoding.GetBytes(@this);

        /// <summary>
        /// Encodes all the characters in the specified string into a sequence of bytes, then returns it.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this string @this, string encoding)
            => Encoding.GetEncoding(encoding).GetBytes(@this);

        /// <summary>
        /// Converts the specified string, which encodes binary data as base-64 digits, to
        ///     an equivalent 8-bit unsigned integer array.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static byte[] Base64Decode(this string @this)
            => Convert.FromBase64String(@this);

        /// <summary>
        /// Converts the specified string, which encodes binary data as base-64 digits, to a new string.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static string Base64Decode(this string @this, Encoding encoding)
            => Base64Decode(@this).GetString(encoding);

        /// <summary>
        /// Converts the specified string, which encodes binary data as hex digits, to
        ///     an equivalent 8-bit unsigned integer array.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static byte[] HexDecode(this string @this, string separator = "")
        {
            if (@this.IsNullOrEmpty()) return new byte[0];

            var hexString = @this;
            if (!separator.IsEmpty())
                hexString = hexString.Replace(separator, "");

            var length = @this.Length;
            if (length.IsOdd())
                throw new FormatException("The specified string's length must be even.");

            var ret = new byte[length / 2];
            for (int i = 0; i < ret.Length; i++)
                ret[i] = Convert.ToByte(@this.Substring(i * 2, 2), 16);

            return ret;
        }

        /// <summary>
        /// Converts the specified string, which encodes binary data as hex digits, to a new string.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string HexDecode(this string @this, Encoding encoding, string separator = "")
            => HexDecode(@this).GetString(encoding);

        /// <summary>
        /// Converts the specified string into a URL-encoded string.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static string UrlEncode(this string @this) => WebUtility.UrlEncode(@this);

        /// <summary>
        /// Converts the specified string that has been encoded for transmission in a URL into a decoded string.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static string UrlDecode(this string @this) => WebUtility.UrlDecode(@this);

        /// <summary>
        /// Converts the specified string to an HTML-encoded string.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static string HtmlEncode(this string @this) => WebUtility.HtmlEncode(@this);

        /// <summary>
        /// Converts the specified string that has been HTML-encoded for HTTP transmission into a decoded string.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static string HtmlDecode(this string @this) => WebUtility.HtmlDecode(@this);

    }
}
