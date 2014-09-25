using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistanceMap.QueryParts
{
    internal class QueryPartDecorator : IQueryPartDecorator, IQueryPart
    {
        public QueryPartDecorator()
        {
            Parts = new List<IQueryPart>();
        }

        public QueryPartDecorator(IQueryPart[] parts)
        {
            // ensure parameter is not null
            parts.EnsureArgumentNotNull("parts");

            Parts = parts.ToList();
            //ChildSeparator = "";
        }

        //public string ChildSeparator { get; set; }

        #region IQueryPartDecorator Implementation

        public virtual void Add(IQueryPart part)
        {
            if (IsSealded)
                return;

            Parts.Add(part);
        }

        public virtual void AddToLast(IQueryPart part, OperationType operation)
        {
            if (IsSealded)
                return;

            var last = Parts.Last(p => p.OperationType == operation && p is IQueryPartDecorator) as IQueryPartDecorator;
            if (last == null)
                return;

            last.Add(part);
        }

        public virtual void AddToLast(IQueryPart part, Func<IQueryPart, bool> predicate)
        {
            if (IsSealded)
                return;

            var last = Parts.Last(predicate) as IQueryPartDecorator;
            if (last == null)
                return;

            last.Add(part);
        }

        public void Remove(IQueryPart part)
        {
            if (Parts.Contains(part))
                Parts.Remove(part);
        }

        IEnumerable<IQueryPart> IQueryPartDecorator.Parts
        {
            get
            {
                return Parts;
            }
        }

        public IList<IQueryPart> Parts { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating if parts can be added to the decorator
        /// </summary>
        public bool IsSealded { get; set; }

        #endregion

        #region IQueryPart Implementation

        public OperationType OperationType { get; set; }

        public virtual string Compile()
        {
            var sb = new StringBuilder();

            var last = Parts.LastOrDefault();
            foreach (var part in Parts)
            {
                var value = part.Compile();
                if (string.IsNullOrEmpty(value))
                    continue;
                sb.Append(value);
                //switch (OperationType)
                //{
                //    case PersistanceMap.OperationType.Values:
                //    case PersistanceMap.OperationType.None:
                //    case PersistanceMap.OperationType.Insert:
                //        sb.Append(string.Format("{0}{1}", value, part != last ? ChildSeparator : ""));
                //        break;

                //    default:
                //        sb.AppendLine(string.Format("{0}{1}", value, part != last ? ChildSeparator : ""));
                //        break;
                //}
            }

            return sb.ToString().RemoveLineBreak();
        }

        #endregion
    }
}
