using PersistanceMap.QueryBuilder;
using System;
using System.Linq.Expressions;

namespace PersistanceMap.QueryProvider
{
    public interface ISelectMapOption<T>
    {
        IQueryMap MapTo<TAlias, TOut>(Expression<Func<TAlias, TOut>> source, Expression<Func<T, TOut>> alias);

        //IQueryMap MapTo<TSource, TAlias, TOut>(Expression<Func<TAlias, TOut>> source, Expression<Func<TSource, TOut>> alias);
    }
}
