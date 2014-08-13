using PersistanceMap.QueryBuilder;
using System;

namespace PersistanceMap
{
    public interface IExpressionCompiler
    {
        CompiledQuery Compile<T>(SelectQueryPartsMap queryParts);

        CompiledQuery Compile(ProcedureQueryPartsMap queryParts);
    }
}
