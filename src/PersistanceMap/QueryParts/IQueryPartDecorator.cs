using System;
using System.Collections.Generic;

namespace PersistanceMap.QueryParts
{
    public interface IQueryPartDecorator : IQueryPart
    {
        void Add(IQueryPart part);

        void AddToLast(IQueryPart part, OperationType operation);

        void AddToLast(IQueryPart part, Func<IQueryPart, bool> predicate);

        void Remove(IQueryPart part);

        IEnumerable<IQueryPart> Parts { get; }

        /// <summary>
        /// Gets or sets a value indicating if parts can be added to the decorator
        /// </summary>
        bool IsSealded { get; set; }
    }
}
