using System;
using System.Text;

namespace PersistanceMap.QueryParts
{
    public class EntityPart : QueryPartDecorator, IEntityPart, IQueryPart
    {
        public EntityPart(OperationType operation, string entity = null, string entityAlias = null, string id = null)
        {
            Entity = entity;
            EntityAlias = entityAlias;
            OperationType = operation;
            ID = id ?? operation.ToString();
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
            return string.Format("{0} - Operation: [{1}] Entity: [{2}] Alias: [{3}]", GetType().Name, OperationType.ToString(), Entity, EntityAlias);
        }
    }
}
