using System.Collections.Generic;

namespace PersistanceMap
{
    public interface IExpressionQueryPart : IQueryPart
    {
        IEnumerable<IExpressionMapQueryPart> Operations { get; }
    }
}
