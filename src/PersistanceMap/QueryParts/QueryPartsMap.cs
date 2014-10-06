﻿using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistanceMap.QueryParts
{
    /// <summary>
    /// Base Class for IQueryPartsMap implementations
    /// </summary>
    public class QueryPartsMap : IQueryPartsMap
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

        public void AddToLast(IQueryPart part, Func<IQueryPart, bool> predicate)
        {
            var last = Parts.Where(p => p is IQueryPartDecorator).Last(predicate) as IQueryPartDecorator;
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

        //public abstract CompiledQuery Compile();

        public virtual CompiledQuery Compile()
        {
            var sb = new StringBuilder(100);

            // loop all parts and compile
            foreach (var part in Parts)
            {
                switch (part.OperationType)
                {
                    default:
                        sb.Append(part.Compile());
                        break;
                }
            }

            return new CompiledQuery
            {
                QueryString = sb.ToString(),
                QueryParts = this
            };
        }

        #endregion
    }
}