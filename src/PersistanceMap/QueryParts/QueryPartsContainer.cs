using System;
using System.Collections.Generic;
using System.Linq;

namespace PersistanceMap.QueryParts
{
    /// <summary>
    /// Base Class for IQueryPartsContainer implementations
    /// </summary>
    public class QueryPartsContainer : IQueryPartsContainer
    {
        #region IQueryPartsContainer Implementation

        public virtual void Add(IQueryPart part)
        {
            Parts.Add(part);
        }

        public virtual void AddBefore(IQueryPart part, OperationType operation)
        {
            var first = Parts.FirstOrDefault(p => p.OperationType == operation);
            var index = Parts.IndexOf(first);
            if (index < 0)
                index = 0;

            Parts.Insert(index, part);
        }

        public virtual void AddAfter(IQueryPart part, OperationType operation)
        {
            var first = Parts.LastOrDefault(p => p.OperationType == operation);
            var index = Parts.IndexOf(first) + 1;
            //if (index > Parts.Count)
            //    index = 0;

            Parts.Insert(index, part);
        }

        public void AddToLast(IQueryPart part, OperationType operation)
        {
            var last = Parts.OfType<IItemsQueryPart>().LastOrDefault(p => p.OperationType == operation);
            if (last == null)
                return;

            last.Add(part);
        }

        public void AddToLast(IQueryPart part, Func<IQueryPart, bool> predicate)
        {
            var last = Parts.OfType<IItemsQueryPart>().LastOrDefault(predicate) as IItemsQueryPart;
            if (last == null)
                return;

            last.Add(part);
        }

        public void Remove(IQueryPart part)
        {
            if (Parts.Contains(part))
            {
                Parts.Remove(part);
            }
        }

        IEnumerable<IQueryPart> IQueryPartsContainer.Parts
        {
            get
            {
                return Parts;
            }
        }

        private IList<IQueryPart> _parts;
        public IList<IQueryPart> Parts
        {
            get
            {
                if (_parts == null)
                    _parts = new List<IQueryPart>();
                return _parts;
            }
        }

        public bool IsSealed
        {
            get
            {
                return Parts.OfType<IItemsQueryPart>().Any(p => p.IsSealed);
            }
        }

        #endregion

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
