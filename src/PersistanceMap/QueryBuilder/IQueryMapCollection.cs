using System.Collections.Generic;

namespace PersistanceMap.QueryBuilder
{
    public interface IQueryMapCollection : IQueryPart
    {
        IEnumerable<IQueryMap> MapCollection { get; }
    }
}
