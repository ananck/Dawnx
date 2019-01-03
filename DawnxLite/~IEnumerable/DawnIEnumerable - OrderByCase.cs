﻿using Dawnx.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Dawnx
{
    public static partial class DawnIEnumerable
    {
        public static IOrderedEnumerable<TEntity> OrderByCase<TEntity, TRet>(this IEnumerable<TEntity> @this,
            Expression<Func<TEntity, TRet>> memberExp,
            TRet[] orderValues)
            => @this.OrderByCaseStrategy(new OrderByCaseStrategy<TEntity, TRet>(memberExp, orderValues));

        public static IOrderedEnumerable<TEntity> OrderByCaseDescending<TEntity, TRet>(this IEnumerable<TEntity> @this,
            Expression<Func<TEntity, TRet>> memberExp,
            TRet[] orderValues)
            => @this.OrderByCaseDescendingStrategy(new OrderByCaseStrategy<TEntity, TRet>(memberExp, orderValues));

    }
}
