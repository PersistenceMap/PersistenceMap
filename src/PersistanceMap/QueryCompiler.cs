using PersistanceMap.QueryBuilder;

namespace PersistanceMap
{
    /// <summary>
    /// Compiels the QueryParts to a sql string
    /// </summary>
    public class QueryCompiler : IQueryCompiler
    {
        /// <summary>
        /// Compile IQueryPartsMap to a QueryString
        /// </summary>
        /// <param name="queryParts"></param>
        /// <returns></returns>
        public virtual CompiledQuery Compile(IQueryPartsMap queryParts)
        {
            return queryParts.Compile();
        }

        /// <summary>
        /// Compile IQueryPartsMap to a QueryString
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryParts"></param>
        /// <returns></returns>
        public virtual CompiledQuery Compile<T>(IQueryPartsMap queryParts)
        {
            ////TODO: field parts should be added before the compiler
            //var selectQueryPart = queryParts as SelectQueryPartsMap;
            //if (selectQueryPart != null)
            //{
            //    // get all members on the type to be composed
            //    var members = typeof(T).GetSelectionMembers();

            //    // don't set entity alias to prevent fields being set with a default alias of the from expression
            //    //TODO: should entity also not be set?
            //    var fields = members.Select(m => m.ToFieldQueryPart(null, null /*from.Entity*/));

            //    FieldQueryPart.FiedlPartsFactory(selectQueryPart, fields.ToArray());
            //}

            return queryParts.Compile();
        }
    }
}
