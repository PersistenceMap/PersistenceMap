using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistanceMap.QueryParts
{
    public class QueryPartDecorator : IQueryPartDecorator, IQueryPart
    {
        public QueryPartDecorator(string id = null)
        {
            Parts = new List<IQueryPart>();
            ID = id;
        }

        public QueryPartDecorator(OperationType operation, string id = null)
        {
            Parts = new List<IQueryPart>();
            OperationType = operation;
            ID = id;
        }

        #region IQueryPartDecorator Implementation

        public virtual void Add(IQueryPart part)
        {
            if (IsSealed)
            {
                return;
            }

            Parts.Add(part);
        }

        public void Remove(IQueryPart part)
        {
            if (Parts.Contains(part))
            {
                Parts.Remove(part);
            }
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
        public bool IsSealed { get; set; }

        #endregion

        #region IQueryPart Implementation

        public string ID { get; set; }

        public OperationType OperationType { get; set; }

        public virtual string Compile()
        {
            return string.Empty;
        }

        #endregion
    }
}
