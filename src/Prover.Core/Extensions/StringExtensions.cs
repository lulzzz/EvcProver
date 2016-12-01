﻿namespace Prover.Core.Extensions
{
    internal static class StringExtensions
    {
        /// <summary>
        ///     Get substring of specified number of characters on the right.
        /// </summary>
        public static string Right(this string value, int length)
        {
            return value.Substring(value.Length - length);
        }
    }
}