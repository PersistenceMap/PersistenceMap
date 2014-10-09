
namespace PersistanceMap.QueryBuilder.Commands
{
    internal class InsertQueryCommand : IQueryCommand
    {
        public InsertQueryCommand(IQueryPartsMap map)
        {
            QueryPartsMap = map;
        }

        public IQueryPartsMap QueryPartsMap { get; private set; }

        public void Execute(IDatabaseContext context)
        {
            var expr = context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile(QueryPartsMap);
            context.Kernel.Execute(query);
        }
    }
}
