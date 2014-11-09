
namespace PersistanceMap.QueryBuilder.Commands
{
    public class MapQueryCommand : IQueryCommand
    {
        public MapQueryCommand(IQueryPartsMap map)
        {
            QueryPartsMap = map;
        }

        public IQueryPartsMap QueryPartsMap { get; private set; }

        public void Execute(IDatabaseContext context)
        {
            var expr = context.ConnectionProvider.QueryCompiler;
            var query = expr.Compile(QueryPartsMap);
            context.Kernel.Execute(query);
        }
    }
}
