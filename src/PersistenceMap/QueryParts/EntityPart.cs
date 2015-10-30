using System;

namespace PersistenceMap.QueryParts
{
    public class EntityPart : ItemsQueryPart, IEntityPart, IQueryPart
    {
        public EntityPart(OperationType operation, string entity = null, string entityAlias = null, Type entityType = null, string id = null)
            : base(operation, entityType, id)
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
            return string.Format("{0} - Operation: [{1}] Entity: [{2}] Alias: [{3}]", GetType().Name, OperationType, Entity, EntityAlias);
        }
    }
}
