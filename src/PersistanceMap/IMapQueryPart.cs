using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistanceMap
{
    public interface IMapQueryPart : IQueryPart
    {
        MapOperationType MapOperationType { get; }

        LambdaExpression Expression { get; }
    }

    public interface IIdentifierMapQueryPart : IMapQueryPart
    {
        Dictionary<Type, string> IdentifierMap { get; }
    }
}
