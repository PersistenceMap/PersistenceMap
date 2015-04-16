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

    public class EntityMap : QueryPartDecorator, IEntityMap, IQueryPart
    {
        //public EntityMap(OperationType operation, string entity = null, string entityAlias = null)
        //    : this(operation, entity, entityAlias, operation.ToString())
        //{
        //}

        public EntityMap(OperationType operation, string entity = null, string entityAlias = null, string id = null)
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

    public class QueryPart : IQueryPart
    {
        public QueryPart(OperationType operation)
            : this(operation, operation.ToString())
        {
        }

        public QueryPart(OperationType operation, string id)
        {
            OperationType = operation;
            ID = id;
        }

        #region IQueryPart Implementation

        public string ID { get; set; }

        public OperationType OperationType { get; set; }

        public virtual string Compile()
        {
            //var sb = new StringBuilder();

            //foreach (var part in Parts)
            //{
            //    var value = part.Compile();
            //    if (string.IsNullOrEmpty(value))
            //        continue;

            //    sb.Append(value);
            //}

            //return sb.ToString().RemoveLineBreak();
            throw new NotImplementedException();
        }

        #endregion
    }
}
