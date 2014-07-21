using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder
{
    public interface IQueryMap : IQueryPart
    {
        //MapOperationType MapOperationType { get; }

        LambdaExpression Expression { get; }

        Dictionary<Type, string> IdentifierMap { get; }

        //void Register(IQueryProvider provider);
    }
}
