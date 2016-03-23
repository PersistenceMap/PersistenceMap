using System;
using System.Collections.Generic;
using System.Linq;

namespace PersistenceMap.QueryParts
{
    /// <summary>
    /// Base Class for IQueryPartsContainer implementations
    /// </summary>
    public class QueryPartsContainer : IQueryPartsContainer
    {
        private IList<IQueryPart> _parts;

        public virtual void Add(IQueryPart part)
        {
            Parts.Add(part);

            if (AggregatePart == null)
            {
                AggregatePart = part;
                AggregateType = part.EntityType;
            }
        }

        public virtual void AddBefore(IQueryPart part, OperationType operation)
        {
            var first = Parts.FirstOrDefault(p => p.OperationType == operation);
            var index = Parts.IndexOf(first);
            if (index < 0)
            {
                index = 0;
            }

            Parts.Insert(index, part);
        }

        public virtual void AddAfter(IQueryPart part, OperationType operation)
        {
            var first = Parts.LastOrDefault(p => p.OperationType == operation);
            var index = Parts.IndexOf(first) + 1;

            Parts.Insert(index, part);
        }

        public void AddToLast(IQueryPart part, OperationType operation)
        {
            var last = Parts.LastOrDefault(p => p.OperationType == operation);
            if (last == null)
            {
                return;
            }

            last.Add(part);
        }

        public void AddToLast(IQueryPart part, Func<IQueryPart, bool> predicate)
        {
            var last = Parts.LastOrDefault(predicate);
            if (last == null)
            {
                return;
            }

            last.Add(part);
        }

        /// <summary>
        /// Remove the part from the Query tree
        /// </summary>
        /// <param name="part">The part to be removed</param>
        public void Remove(IQueryPart part)
        {
            if (Parts.Contains(part))
            {
                Parts.Remove(part);
            }
        }

        /// <summary>
        /// The list of queryparts in the container
        /// </summary>
        IEnumerable<IQueryPart> IQueryPartsContainer.Parts
        {
            get
            {
                return Parts;
            }
        }

        /// <summary>
        /// The list of queryparts in the container
        /// </summary>
        public IList<IQueryPart> Parts
        {
            get
            {
                if (_parts == null)
                { 
                    _parts = new List<IQueryPart>();
                }

                return _parts;
            }
        }

        /// <summary>
        /// Gets the aggregate part for the query
        /// </summary>
        public IQueryPart AggregatePart { get; set; }

        /// <summary>
        /// Gets or sets the aggregate type for this query
        /// </summary>
        public Type AggregateType { get; set; }
        
        #region IEnumerable<IQueryPart> Implementation

        public IEnumerator<IQueryPart> GetEnumerator()
        {
            return Parts.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
