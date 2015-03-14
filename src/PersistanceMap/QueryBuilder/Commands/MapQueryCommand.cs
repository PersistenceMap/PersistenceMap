
namespace PersistanceMap.QueryBuilder.Commands
{
    public class MapQueryCommand : IQueryCommand
    {
        public MapQueryCommand(IQueryPartsContainer map)
        {
            QueryParts = map;
        }

        public IQueryPartsContainer QueryParts { get; private set; }

        public void Execute(IDatabaseContext context)
        {
            var expr = context.ConnectionProvider.QueryCompiler;
            var query = expr.Compile(QueryParts);
            context.Kernel.Execute(query);
        }
    }
}
