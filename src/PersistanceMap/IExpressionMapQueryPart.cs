using System;
using System.Linq.Expressions;

namespace PersistanceMap
{
    public interface IExpressionMapQueryPart : IQueryPart
    {
        MapOperationType MapOperationType { get; }

        LambdaExpression Expression { get; }

        string Compile();
    }
}
