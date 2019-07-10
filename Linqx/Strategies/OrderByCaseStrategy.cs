﻿using Dawnx;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Linqx.Strategies
{
    public class OrderByCaseStrategy<TEntity, TRet> : IOrderStrategy<TEntity>
    {
        public Expression<Func<TEntity, int>> StrategyExpression { get; }

        public OrderByCaseStrategy(
            Expression<Func<TEntity, TRet>> memberExp,
            TRet[] orderValues)
        {
            var valueLenth = orderValues.Length;
            var lambdaExp = orderValues.Reverse().AsVI().Aggregate(null as Expression, (acc, vi) =>
            {
                var compareExp = Expression.Equal(memberExp.Body, Expression.Constant(vi.Value));

                if (acc is null)
                {
                    return
                        Expression.Condition(
                            compareExp,
                            Expression.Constant(valueLenth - 1 - vi.Index),
                            Expression.Constant(valueLenth));
                }
                else
                {
                    return
                        Expression.Condition(
                            compareExp,
                            Expression.Constant(valueLenth - 1 - vi.Index),
                            acc);
                }
            });

            StrategyExpression =
                Expression.Lambda<Func<TEntity, int>>(lambdaExp, memberExp.Parameters);
        }

    }
}