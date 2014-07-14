using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.Compiler
{
    /// <summary>
    /// Compiles all elements of a QueryPartsContainer to a CompiledQuery
    /// </summary>
    public class QueryCompiler
    {
        readonly IQueryPartsMap _queryParts;

        public QueryCompiler(IQueryPartsMap queryParts)
        {
            _queryParts = queryParts;
        }

        public CompiledQuery Compile()
        {
            var query = Compile(_queryParts as SelectQueryPartsMap);

            if (query == null)
                query = Compile(_queryParts as ProcedureQueryPartsMap);

            return query;
        }

        private CompiledQuery Compile(SelectQueryPartsMap map)
        {
            if (map == null)
                return null;

            var sb = new StringBuilder(100);
            sb.Append("select ");

            // add resultset fields
            foreach (var field in map.Fields)
                sb.AppendFormat("{0}{1} ", field.Compile(), map.Fields.Last() == field ? "" : ",");

            // add from
            sb.AppendFormat("{0} \r\n", map.From.Compile());

            // add joins
            foreach (var join in map.Joins)
                sb.Append(join.Compile());

            // where

            // order...

            return new CompiledQuery
            {
                QueryString = sb.ToString(),
                QueryParts = map
            };
        }

        private CompiledQuery Compile(ProcedureQueryPartsMap map)
        {
            if (map == null)
                return null;

            var sb = new StringBuilder(100);
            sb.Append(string.Format("exec {0} ", map.ProcedureName));

            var conv = new LambdaExpressionToSqlCompiler();
            conv.PrefixFieldWithTableName = false;

            foreach (var param in map.Parameters)
            {
                //TODO: operations can also be MapOperationType.Identifier!
                var valuePredicate = param.Operations.FirstOrDefault(o => o.MapOperationType == MapOperationType.Value);
                if (valuePredicate != null)
                    sb.Append(string.Format("{0} ", conv.Compile(valuePredicate.Expression)));
            }

            return new CompiledQuery
            {
                QueryString = sb.ToString(),
                QueryParts = map
            };
        }
    }
}
