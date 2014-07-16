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
        public static IMapQueryPart Compile(Expression<Func<ProcedureMapOption, IMapQueryPart>> predicate)
        {
            var options = new ProcedureMapOption();

            return predicate.Compile().Invoke(options);
        }

        public static IEnumerable<IMapQueryPart> Compile<T>(params Expression<Func<SelectMapOption<T>, IMapQueryPart>>[] predicates)
        {
            var parts = new List<IMapQueryPart>();
            var options = new SelectMapOption<T>();

            foreach (var predicate in predicates)
                parts.Add(predicate.Compile().Invoke(options));

            return parts;
        }

        public static IEnumerable<IMapQueryPart> Compile<T, T2>(params Expression<Func<SelectMapOption<T, T2>, IMapQueryPart>>[] predicates)
        {
            var parts = new List<IMapQueryPart>();
            var options = new SelectMapOption<T, T2>();

            foreach (var predicate in predicates)
                parts.Add(predicate.Compile().Invoke(options));

            return parts;
        }
    }
}
