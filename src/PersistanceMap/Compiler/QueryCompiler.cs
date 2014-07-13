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
        readonly QueryPartsContainer _queryParts;

        public QueryCompiler(QueryPartsContainer queryParts)
        {
            _queryParts = queryParts;
        }

        public CompiledQuery Compile()
        {
            var sb = new StringBuilder(100);
            sb.Append("select ");

            // add resultset fields
            foreach (var field in _queryParts.Fields)
                sb.AppendFormat("{0}{1} ", field.Compile(), _queryParts.Fields.Last() == field ? "" : ",");

            // add from
            sb.AppendFormat("{0} ", _queryParts.From.Compile());

            // add joins
            foreach (var join in _queryParts.Joins)
                sb.Append(join.Compile());

            // where

            // order...

            return new CompiledQuery
            {
                QueryString = sb.ToString(),
                QueryParts = _queryParts
            };
        }
    }
}
