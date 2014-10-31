
namespace PersistanceMap.QueryBuilder.Commands
{
    internal class UpdateQueryCommand : IQueryCommand
    {
        public UpdateQueryCommand(IQueryPartsMap map)
        {
            QueryPartsMap = map;
        }

        public IQueryPartsMap QueryPartsMap { get; private set; }

        public void Execute(IDatabaseContext context)
        {
            var expr = context.ContextProvider.QueryCompiler;
            var query = expr.Compile(QueryPartsMap);
            context.Kernel.Execute(query);
        }
    }
}
