using System;
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

        void AddToLast(IQueryPart part, Func<IQueryPart, bool> predicate);

        IEnumerable<IQueryPart> Parts { get; }

        CompiledQuery Compile();
    }
}
