using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace PersistanceMap.QueryParts
{
    internal class FieldQueryPart : IFieldMap, IEntityMap, IQueryPart
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
        public Expression<Func<object, object>> Converter { get; private set; }

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

        /// <summary>
        /// The type inside the original table
        /// </summary>
        public Type FieldType { get; set; }

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


        internal static void FiedlPartsFactory(SelectQueryPartsMap queryParts, FieldQueryPart[] fields)
        {
            //TODO: this method should be removed!
            foreach (var map in queryParts.Parts.OfType<IQueryPartDecorator>().Where(p => p.OperationType == OperationType.Select))
            {
                // add all mapped fields to a collection to ensure that they are used in the query
                var unusedMappedFields = map.Parts.OfType<FieldQueryPart>().ToList();

                foreach (var field in fields)
                {
                    // check if the field was allready mapped previously
                    var mappedFields = map.Parts.OfType<FieldQueryPart>().Where(f => string.Equals(f.Field, field.Field, StringComparison.OrdinalIgnoreCase) || string.Equals(f.FieldAlias, field.Field, StringComparison.OrdinalIgnoreCase));
                    if (mappedFields.Any())
                    {
                        foreach (var mappedField in mappedFields)
                        {
                            mappedField.Sufix = ", ";
                            unusedMappedFields.Remove(mappedField);
                        }

                        continue;
                    }

                    if (map.IsSealded)
                        continue;

                    // add the new field
                    field.Sufix = ", ";
                    map.Add(field);
                }

                // remove all mapped fields that were not included in the select fields
                foreach (var field in unusedMappedFields)
                {
                    map.Remove(field);
                }

                var last = map.Parts.LastOrDefault(p => p is FieldQueryPart) as FieldQueryPart;
                if (last != null)
                    last.Sufix = " ";
            }
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
