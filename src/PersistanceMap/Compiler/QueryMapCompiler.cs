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
    internal static class QueryMapCompiler
    {
        public static IQueryMap Compile(Expression<Func<IProcedureMapOption, IQueryMap>> predicate)
        {
            var options = new ProcedureMapOption();

            return predicate.Compile().Invoke(options);
        }

        public static IEnumerable<IQueryMap> Compile<T>(params Expression<Func<IProcedureMapOption<T>, IQueryMap>>[] predicates)
        {
            var parts = new List<IQueryMap>();
            var options = new ProcedureMapOption<T>();

            foreach (var predicate in predicates)
                parts.Add(predicate.Compile().Invoke(options));

            return parts;
        }

        public static IEnumerable<IQueryMap> Compile<T>(params Expression<Func<IJoinMapOption<T>, IQueryMap>>[] predicates)
        {
            var parts = new List<IQueryMap>();
            var options = new SelectMapOption<T>();

            foreach (var predicate in predicates)
                parts.Add(predicate.Compile().Invoke(options));

            return parts;
        }

        public static IEnumerable<IQueryMap> Compile<T, T2>(params Expression<Func<IJoinMapOption<T, T2>, IQueryMap>>[] predicates)
        {
            var parts = new List<IQueryMap>();
            var options = new SelectMapOption<T, T2>();

            foreach (var predicate in predicates)
                parts.Add(predicate.Compile().Invoke(options));

            return parts;
        }

        public static IEnumerable<IQueryMap> Compile<T>(params Expression<Func<ISelectMapOption<T>, IQueryMap>>[] predicates)
        {
            var parts = new List<IQueryMap>();
            var options = new SelectMapOption<T>();

            foreach (var predicate in predicates)
                parts.Add(predicate.Compile().Invoke(options));

            return parts;
        }
    }
}
