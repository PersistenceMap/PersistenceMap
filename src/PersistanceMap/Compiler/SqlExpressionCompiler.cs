using PersistanceMap.Expressions;
using PersistanceMap.QueryBuilder;
using System.Linq;

namespace PersistanceMap.Compiler
{
    public class SqlExpressionCompiler : IExpressionCompiler
    {
        public virtual CompiledQuery Compile<T>(QueryPartsContainer queryParts)
        {
            var from = queryParts.From;
            if (from == null)
            {
                queryParts.Add(typeof(T).ToFromQueryPart());
                from = queryParts.From;
            }

            var members = typeof(T).GetSelectionMembers();

            foreach (var field in members.Select(m => m.ToFieldQueryPart(from.Identifier, from.Entity)))
                queryParts.Add(field, false);

            var builder = new QueryCompiler(queryParts);
            return builder.Compile();
        }
    }
}
