using System;
using System.Text;

namespace PersistanceMap.QueryParts
{
    public class EntityDelegateQueryPart : DelegateQueryPart, IEntityMap, IQueryPartDecorator, IQueryPart
    {
        public EntityDelegateQueryPart(OperationType operation, Func<string> callback, string entity = null, string entityAlias = null)
            : this(operation, callback, entity, entityAlias, operation.ToString())
        {
        }

        public EntityDelegateQueryPart(OperationType operation, Func<string> callback, string entity = null, string entityAlias = null, string id = null)
            : base(operation, callback, id)
        {
            Entity = entity;
            EntityAlias = entityAlias;
        }

        #region IEntityQueryPart Implementation

        /// <summary>
        /// the
        /// </summary>
        public string Entity { get; private set; }

        /// <summary>
        /// the alias of the entity
        /// </summary>
        public string EntityAlias { get; set; }

        #endregion

        public override string ToString()
        {
            if (Delegate != null)
                return string.Format("{0} - Delegate: [{1}] Operation: [{2}] Entity: [{3}] Alias: [{4}]", GetType().Name, Delegate.ToString(), OperationType.ToString(), Entity, EntityAlias);

            return string.Format("{0} - Delegate: [No delegate defined] Operation: [{1}] Entity: [{2}] Alias: [{3}]", GetType().Name, OperationType.ToString(), Entity, EntityAlias);
        }
    }
}
