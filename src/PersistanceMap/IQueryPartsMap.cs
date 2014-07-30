using System.Collections.Generic;
using PersistanceMap.QueryBuilder;

namespace PersistanceMap
{
    public interface IQueryPartsMap
    {
        void Add(IQueryPart part);

        void AddBefore(IQueryPart part, OperationType operation);

        void AddAfter(IQueryPart part, OperationType operation);

        void AddToLast(IQueryPart part, OperationType operation);

        IEnumerable<IQueryPart> Parts { get; }

        CompiledQuery Compile();
    }
}
