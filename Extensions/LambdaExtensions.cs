using CommonUtils.Util;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace CommonUtils.Extensions
{
    public static class LambdaExtensions
    {
        public static Expression<Func<TInput, bool>> LamdaOr<TInput>
   (this Expression<Func<TInput, bool>> input, Expression<Func<TInput, bool>> expression)
        {
            return PredicateBuilder.Or<TInput>(input, expression);
        }
        public static Expression<Func<TInput, bool>> LamdaAnd<TInput>
           (this Expression<Func<TInput, bool>> input, Expression<Func<TInput, bool>> expression)
        {
            return PredicateBuilder.And<TInput>(input, expression);
        }
    }
}
