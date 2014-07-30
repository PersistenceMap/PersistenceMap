using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using PersistanceMap.Internals;

namespace PersistanceMap.QueryBuilder.Decorators
{
    //TODO: remove interface IQueryMap! It is not needed!
    internal class FieldQueryPart : IFieldQueryMap, IEntityQueryPart, IQueryMap, IQueryPart
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
            : this(field, fieldalias, entityalias, entity, null)
        {
        }

        public FieldQueryPart(string field, string fieldalias, string entityalias, string entity, LambdaExpression expression)
        {
            EntityAlias = entityalias;
            Field = field;
            FieldAlias = fieldalias;
            Entity = entity;
            Expression = expression;

            AliasMap = new Dictionary<Type, string>();
        }

        //TODO: remove interface IQueryMap! It is not needed!
        #region IFieldQueryMap Implementation

        public LambdaExpression Expression { get; private set; }

        /// <summary>
        /// Defines a mapping for types and the alias that the entity has
        /// </summary>
        public Dictionary<Type, string> AliasMap { get; private set; }

        #endregion

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

        public MapOperationType MapOperationType { get; set; }

        public string Compile()
        {
            if (Expression != null)
            {
                //TODO: Is this correct???? or shuld the expression be compiled? or should the expression be translated?
                return FieldHelper.TryExtractPropertyName(Expression);
            }

            //if (string.IsNullOrEmpty(EntityAlias))
            //    return Field;

            //return string.Format("{0}.{1}", EntityAlias ?? Entity, Field);

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
