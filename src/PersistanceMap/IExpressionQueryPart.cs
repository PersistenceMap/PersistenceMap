using System.Collections.Generic;

namespace PersistanceMap
{
    public interface IExpressionQueryPart : IEntityQueryPart
    {
        IEnumerable<IExpressionMapQueryPart> Operations { get; }
    }
}
