using System;
using System.Collections.Generic;

namespace PersistenceMap.QueryParts
{
    public interface IItemsQueryPart : IQueryPart
    {
        /// <summary>
        /// Add a IQueryPart to the items collection
        /// </summary>
        /// <param name="part"></param>
        void Add(IQueryPart part);

        /// <summary>
        /// Remove a IQueryPart from the items collection
        /// </summary>
        /// <param name="part"></param>
        void Remove(IQueryPart part);

        /// <summary>
        /// The colleciton of child parts
        /// </summary>
        IEnumerable<IQueryPart> Parts { get; }

        /// <summary>
        /// Gets or sets a value indicating if parts can be added to the decorator
        /// </summary>
        bool IsSealed { get; set; }
    }
}
