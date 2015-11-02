using System;
using System.Collections.Generic;

namespace PersistenceMap.QueryParts
{
    public class QueryPart : IQueryPart
    {
        private readonly Lazy<List<IQueryPart>> _parts;

        public QueryPart(OperationType operation)
            : this(operation, null)
        {
        }

        public QueryPart(OperationType operation, Type entityType)
            : this(operation, entityType, operation.ToString())
        {
        }

        public QueryPart(OperationType operation, Type entityType, string id)
        {
            OperationType = operation;
            ID = id ?? operation.ToString();
            EntityType = entityType;
            _parts = new Lazy<List<IQueryPart>>(() => new List<IQueryPart>());
        }

        #region IQueryPart Implementation

        public string ID { get; set; }

        public OperationType OperationType { get; set; }

        public Type EntityType { get; set; }
        
        public IEnumerable<IQueryPart> Parts
        {
            get
            {
                return _parts.Value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if parts can be added to the decorator
        /// </summary>
        public bool IsSealed { get; set; }

        public virtual void Add(IQueryPart part)
        {
            if (IsSealed)
            {
                return;
            }

            _parts.Value.Add(part);
        }

        public void Remove(IQueryPart part)
        {
            if (_parts.Value.Contains(part))
            {
                _parts.Value.Remove(part);
            }
        }

        public virtual string Compile()
        {
            return string.Empty;
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0} - Operation: [{1}] Id: [{2}]", GetType().Name, OperationType, ID ?? string.Empty);
        }
    }
}
