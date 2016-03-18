
namespace PersistenceMap.QueryBuilder.Commands
{
    /// <summary>
    /// A command containing a Query that will be executed at the commit
    /// </summary>
    public class QueryCommand : IQueryCommand
    {
        /// <summary>
        /// Creates a command object
        /// </summary>
        /// <param name="map">The query parts</param>
        public QueryCommand(IQueryPartsContainer map)
        {
            QueryParts = map;
        }

        /// <summary>
        /// Gets the query parts of the command
        /// </summary>
        public IQueryPartsContainer QueryParts { get; private set; }

        /// <summary>
        /// Compiles and executes the Query against the databasecontext
        /// </summary>
        /// <param name="context">The database context</param>
        public void Execute(IDatabaseContext context)
        {
            var expr = context.ConnectionProvider.QueryCompiler;

            var query = expr.Compile(QueryParts, context.Interceptors);
            context.ExecuteNonQuery(query);
        }
    }
}
