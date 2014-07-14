using PersistanceMap.QueryBuilder;

namespace PersistanceMap
{
    public interface IExpressionCompiler
    {
        CompiledQuery Compile<T>(SelectQueryPartsMap queryParts);

        CompiledQuery Compile<T>(ProcedureQueryPartsMap queryParts);

        CompiledQuery Compile(ProcedureQueryPartsMap queryParts);
    }
}
