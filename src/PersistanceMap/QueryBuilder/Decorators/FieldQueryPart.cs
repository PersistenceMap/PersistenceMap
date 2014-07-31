using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using PersistanceMap.Internals;

namespace PersistanceMap.QueryBuilder.Decorators
{
    //TODO: remove interface IQueryMap! It is not needed!
    internal class FieldQueryPart : IFieldQueryMap, IEntityQueryPart, IQueryPart
    {
        public FieldQueryPart(string field, string entityalias)
            : this(field, entityalias, null)
        {
        }

        public FieldQueryPart(string field, string entityalias, string entity)
            : this(field, null, entityalias, entity)
        {
        }

        public FieldQueryPart(string field, string fieldalias, string entityalias, string entity)
        {
            EntityAlias = entityalias;
            Field = field;
            FieldAlias = fieldalias;
            Entity = entity;
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

        /// <summary>
        /// the name of the field
        /// </summary>
        public string Field { get; private set; }

        /// <summary>
        /// the alias of the field
        /// </summary>
        public string FieldAlias { get; private set; }

        #endregion

        #region IQueryPart Implementation

        public OperationType OperationType { get; set; }

        public string Compile()
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(EntityAlias) || !string.IsNullOrEmpty(Entity))
                sb.Append(string.Format("{0}.", EntityAlias ?? Entity));

            sb.Append(Field);
            
            if (!string.IsNullOrEmpty(FieldAlias))
                sb.Append(string.Format(" as {0}", FieldAlias));

            return sb.ToString();
        }

        public override string ToString()
        {
            return string.Format("Entity: {0} Field: {1} [{1}.{2}]", Entity, EntityAlias ?? Entity, Field);
        }

        #endregion
    }
}
