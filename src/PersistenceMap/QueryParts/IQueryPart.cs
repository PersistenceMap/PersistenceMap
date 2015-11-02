
using System;
using System.Collections.Generic;

namespace PersistenceMap.QueryParts
{
    public interface IQueryPart
    {
        /// <summary>
        /// The ID of the QueryPart
        /// </summary>
        string ID { get; set; }

        /// <summary>
        /// Defines the type of operation that this part is
        /// </summary>
        OperationType OperationType { get; }

        /// <summary>
        /// Gets the type of the entity/table
        /// </summary>
        Type EntityType { get; }

        /// <summary>
        /// The colleciton of child parts
        /// </summary>
        IEnumerable<IQueryPart> Parts { get; }

        /// <summary>
        /// Gets or sets a value indicating if parts can be added to the decorator
        /// </summary>
        bool IsSealed { get; set; }

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
        /// Compile the part to a query string
        /// </summary>
        /// <returns></returns>
        string Compile();
    }
}
