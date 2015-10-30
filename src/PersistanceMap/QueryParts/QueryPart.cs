using System;

namespace PersistanceMap.QueryParts
{
    public class QueryPart : IQueryPart
    {
        public QueryPart(OperationType operation, Type entityType)
            : this(operation, entityType, operation.ToString())
        {
        }

        public QueryPart(OperationType operation, Type entityType, string id)
        {
            OperationType = operation;
            ID = id ?? operation.ToString();
            EntityType = entityType;
        }

        #region IQueryPart Implementation

        public string ID { get; set; }

        public OperationType OperationType { get; set; }

        public Type EntityType { get; set; }

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
