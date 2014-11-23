using System;
using System.Linq.Expressions;
using System.Text;

namespace PersistanceMap.QueryParts
{
    internal class FieldQueryPart : IFieldQueryPart, IEntityQueryPart, IQueryPart
    {
        public FieldQueryPart(string field, string fieldalias, string entityalias = null, string entity = null, string id = null, Expression<Func<object, object>> converter = null)
        {
            EntityAlias = entityalias;
            Field = field;
            FieldAlias = fieldalias;
            Entity = entity;
            ID = id ?? fieldalias ?? field;
            Converter = converter;
        }

        public string ID { get; set; }

        public string Sufix { get; set; }

        /// <summary>
        /// A expression that converts the db value to the object value
        /// </summary>
        public Expression<Func<object,object>> Converter { get; private set; }

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

        public virtual string Compile()
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(EntityAlias) || !string.IsNullOrEmpty(Entity))
                sb.Append(string.Format("{0}.", EntityAlias ?? Entity));

            sb.Append(Field);
            
            if (!string.IsNullOrEmpty(FieldAlias))
                sb.Append(string.Format(" as {0}", FieldAlias));

            if (string.IsNullOrEmpty(Sufix) == false)
                sb.Append(Sufix);

            return sb.ToString();
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0} - Entity: {1} Field: {2} [{2}.{3}]", GetType().Name, Entity, EntityAlias ?? Entity, Field);
        }
    }

    internal class IgnoreFieldQueryPart : FieldQueryPart
    {
        public IgnoreFieldQueryPart(string field, string fieldalias, string entityalias = null, string entity = null, string id = null) :
            base(field, fieldalias, entityalias, entity, id)
        {
        }

        public override string Compile()
        {
            return "";
        }
    }
}
