﻿using System;
using System.Linq;
using System.Linq.Expressions;

namespace NLinq
{
    public static partial class IQueryableX
    {
        /// <summary>
        /// Returns distinct elements from a sequence by using a specified properties to compare values.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public static IQueryable<TSource> DistinctBy<TSource>(this IQueryable<TSource> source, Func<TSource, object> compare)
        {
            return source.GroupBy(compare).Select(x => x.First()).AsQueryable();
        }

        ///// <summary>
        ///// Produces the set difference of two sequences by using the specified properties to compare values.
        ///// </summary>
        ///// <typeparam name="TSource"></typeparam>
        ///// <param name="first"></param>
        ///// <param name="second"></param>
        ///// <param name="compare"></param>
        ///// <returns></returns>
        //public static IQueryable<TSource> ExceptBy<TSource>(this IQueryable<TSource> first, IQueryable<TSource> second, Func<TSource, object> compare)
        //    => Queryable.Except(first, second, new ExactEqualityComparer<TSource>(compare));
    }

}
