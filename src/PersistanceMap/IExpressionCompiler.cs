using PersistanceMap.QueryBuilder;
using System;
using PersistanceMap.QueryParts;

namespace PersistanceMap
{
    public interface IExpressionCompiler
    {
        CompiledQuery Compile(IQueryPartsMap queryParts);

        CompiledQuery Compile<T>(IQueryPartsMap queryParts);

        //CompiledQuery Compile<T>(SelectQueryPartsMap queryParts);

        //CompiledQuery Compile(ProcedureQueryPartsMap queryParts);
    }
}
