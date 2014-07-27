using PersistanceMap.QueryBuilder;
using System;
using System.Linq.Expressions;

namespace PersistanceMap.QueryProvider
{
    public interface IJoinMapOption<T>
    {
        IQueryMap As(Expression<Func<string>> predicate);

        IQueryMap Include<T2, T3, T4>(Expression<Func<T, T2>> alias, Expression<Func<T3, T4>> source);

        IQueryMap Include<T2>(Expression<Func<T, T2>> alias, string source);

        IQueryMap Include<T2>(Expression<Func<T, T2>> field);

        IQueryMap MapTo<TAlias, TOut>(Expression<Func<T, TOut>> source, Expression<Func<TAlias, TOut>> alias);

        IQueryMap MapTo<TOut>(Expression<Func<T, TOut>> source, string alias);
    }

    public interface IJoinMapOption<T, T2> : IJoinMapOption<T>
    {
        IQueryMap And(Expression<Func<T, T2, bool>> predicate);

        IQueryMap And(string alias, Expression<Func<T, T2, bool>> predicate);

        IQueryMap And<T3>(Expression<Func<T, T3, bool>> predicate);

        IQueryMap And<T3>(string alias, Expression<Func<T, T3, bool>> predicate);

        IQueryMap On(Expression<Func<T, T2, bool>> predicate);

        IQueryMap On(string alias, Expression<Func<T, T2, bool>> predicate);

        IQueryMap On<T3>(Expression<Func<T, T3, bool>> predicate);

        IQueryMap On<T3>(string alias, Expression<Func<T, T3, bool>> predicate);

        IQueryMap Or(Expression<Func<T, T2, bool>> predicate);

        IQueryMap Or<T3>(Expression<Func<T, T3, bool>> predicate);
    }
}
