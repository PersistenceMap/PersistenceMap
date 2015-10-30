using System;
using System.Collections.Generic;

namespace PersistanceMap.QueryParts
{
    public class ItemsQueryPart : QueryPart, IItemsQueryPart, IQueryPart
    {
        public ItemsQueryPart(OperationType operation, Type entityType = null, string id = null)
            : base(operation, entityType, id)
        {
            Parts = new List<IQueryPart>();
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

        IEnumerable<IQueryPart> IItemsQueryPart.Parts
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

        public override string Compile()
        {
            return string.Empty;
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0} - Operation: [{1}]", GetType().Name, OperationType);
        }
    }
}
