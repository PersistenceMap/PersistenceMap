using PersistanceMap.QueryBuilder;
using System.Linq;
using PersistanceMap.QueryBuilder.QueryPartsBuilders;
using PersistanceMap.QueryParts;

namespace PersistanceMap.Compiler
{
    public class ExpressionCompiler : IExpressionCompiler
    {
        public virtual CompiledQuery Compile(IQueryPartsMap queryParts)
        {
            return queryParts.Compile();
        }

        public virtual CompiledQuery Compile<T>(SelectQueryPartsMap queryParts)
        {
            // get all members on the type to be composed
            var members = typeof(T).GetSelectionMembers();

            // don't set entity alias to prevent fields being set with a default alias of the from expression
            //TODO: should entity also not be set?
            var fields = members.Select(m => m.ToFieldQueryPart(null, null/*from.Entity*/));
            
            SelectQueryPartsBuilder.Instance.AddFiedlParts(queryParts, fields.ToArray());

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
