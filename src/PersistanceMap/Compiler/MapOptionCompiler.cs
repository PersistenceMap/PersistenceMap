using PersistanceMap.QueryProvider;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using PersistanceMap.QueryBuilder;

namespace PersistanceMap.Compiler
{
    /// <summary>
    /// Helper class that compiles Expression{Func{MapOption{T}, IExpressionMapQueryPart}}[] to IEnumerable{IExpressionMapQueryPart}
    /// </summary>
    internal static class MapOptionCompiler
    {
        public static IQueryMap Compile(Expression<Func<ParameterMapOption, IQueryMap>> predicate)
        {
            var options = new ParameterMapOption();

            return predicate.Compile().Invoke(options);
        }

        public static IEnumerable<IQueryMap> Compile<T>(params Expression<Func<SelectMapOption<T>, IQueryMap>>[] predicates)
        {
            var parts = new List<IQueryMap>();
            var options = new SelectMapOption<T>();

            foreach (var predicate in predicates)
                parts.Add(predicate.Compile().Invoke(options));

            return parts;
        }

        public static IEnumerable<IQueryMap> Compile<T, T2>(params Expression<Func<SelectMapOption<T, T2>, IQueryMap>>[] predicates)
        {
            var parts = new List<IQueryMap>();
            var options = new SelectMapOption<T, T2>();

            foreach (var predicate in predicates)
                parts.Add(predicate.Compile().Invoke(options));

            return parts;
        }
    }
}
