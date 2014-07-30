using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PersistanceMap
{
    /// <summary>
    /// Base Class for IQueryPartsMap implementations
    /// </summary>
    public abstract class QueryPartsMap : IQueryPartsMap
    {
        #region IQueryPartsMap Implementation

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
            var last = Parts.Last(p => p.OperationType == operation && p is IQueryPartDecorator) as IQueryPartDecorator;
            if (last == null)
                return;

            last.Add(part);
        }

        IEnumerable<IQueryPart> IQueryPartsMap.Parts
        {
            get
            {
                return Parts;
            }
        }

        private IList<IQueryPart> _parts;
        internal IList<IQueryPart> Parts
        {
            get
            {
                if (_parts == null)
                    _parts = new List<IQueryPart>();
                return _parts;
            }
        }

        public abstract CompiledQuery Compile();

        #endregion
    }
}
