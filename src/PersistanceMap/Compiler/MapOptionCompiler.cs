using PersistanceMap.Expressions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistanceMap.Compiler
{
    /// <summary>
    /// Helper class that compiles Expression{Func{MapOption{T}, IExpressionMapQueryPart}}[] to IEnumerable{IExpressionMapQueryPart}
    /// </summary>
    internal static class MapOptionCompiler
    {
        public static IEnumerable<IExpressionMapQueryPart> Compile(params Expression<Func<ProcedureMapOption, IExpressionMapQueryPart>>[] predicates)
        {
            var parts = new List<IExpressionMapQueryPart>();
            var options = new ProcedureMapOption();

            foreach (var predicate in predicates)
                parts.Add(predicate.Compile().Invoke(options));

            return parts;
        }

        public static IEnumerable<IExpressionMapQueryPart> Compile<T>(params Expression<Func<SelectMapOption<T>, IExpressionMapQueryPart>>[] predicates)
        {
            var parts = new List<IExpressionMapQueryPart>();
            var options = new SelectMapOption<T>();

            foreach (var predicate in predicates)
                parts.Add(predicate.Compile().Invoke(options));

            return parts;
        }

        public static IEnumerable<IExpressionMapQueryPart> Compile<T, T2>(params Expression<Func<SelectMapOption<T, T2>, IExpressionMapQueryPart>>[] predicates)
        {
            var parts = new List<IExpressionMapQueryPart>();
            var options = new SelectMapOption<T, T2>();

            foreach (var predicate in predicates)
                parts.Add(predicate.Compile().Invoke(options));

            return parts;
        }
    }
}
