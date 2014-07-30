using System.Collections.Generic;
using PersistanceMap.QueryBuilder;

namespace PersistanceMap
{
    public interface IQueryPartsMap
    {
        void Add(IQueryPart part);

        void AddBefore(MapOperationType operation, IQueryPart part);

        void AddAfter(MapOperationType operation, IQueryPart part);

        IEnumerable<IQueryPart> Parts { get; }

        CompiledQuery Compile();
    }
}
