using PersistanceMap.QueryBuilder;

namespace PersistanceMap
{
    public interface IExpressionCompiler
    {
        CompiledQuery Compile<T>(/*ISelectExpression<T> sqlExpr, */QueryPartsContainer queryParts);
    }
}
