using System.Collections.Generic;

namespace PersistanceMap.QueryBuilder
{
    public interface IExpressionQueryPart : IQueryPart
    {
        IEnumerable<IQueryMap> Operations { get; }
    }
}
