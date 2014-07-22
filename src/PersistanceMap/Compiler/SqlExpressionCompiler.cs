using PersistanceMap.QueryBuilder;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PersistanceMap.QueryBuilder.Decorators;

namespace PersistanceMap.Compiler
{
    public class SqlExpressionCompiler : IExpressionCompiler
    {
        public virtual CompiledQuery Compile<T>(SelectQueryPartsMap queryParts)
        {
            var from = queryParts.Joins.FirstOrDefault(j => j.MapOperationType == MapOperationType.From);
            if (from == null)
            {
                from = QueryPartsFactory.CreateEntityQueryPart<T>(queryParts, MapOperationType.From);
            }

            // get all members on the type to be composed
            var members = typeof(T).GetSelectionMembers();

            // don't set identifier to prevent fields being set with a default identifier of the from expression
            //TODO: should entity also not be set?
            foreach (var field in members.Select(m => m.ToFieldQueryPart(null, from.Entity)))
                queryParts.Add(field, false);

            return queryParts.Compile();
        }

        public virtual CompiledQuery Compile(ProcedureQueryPartsMap queryParts)
        {
            //var builder = new ProcedureQueryCompiler(queryParts);
            //return builder.Compile();
            return queryParts.Compile();
        }
    }
}
