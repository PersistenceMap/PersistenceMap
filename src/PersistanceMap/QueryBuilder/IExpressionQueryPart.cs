using System.Collections.Generic;

namespace PersistanceMap.QueryBuilder
{
    public interface IExpressionQueryPart : IQueryPart
    {
        IEnumerable<IMapQueryPart> Operations { get; }
    }
}
