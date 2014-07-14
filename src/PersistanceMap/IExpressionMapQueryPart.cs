using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistanceMap
{
    public interface IExpressionMapQueryPart : IQueryPart
    {
        MapOperationType MapOperationType { get; }

        LambdaExpression Expression { get; }

        Dictionary<Type, string> IdentifierMap { get; }

        string Compile();
    }
}
