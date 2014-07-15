using System.Collections.Generic;

namespace PersistanceMap
{
    public interface IExpressionQueryPart : IQueryPart
    {
        IEnumerable<IMapQueryPart> Operations { get; }
    }
}
