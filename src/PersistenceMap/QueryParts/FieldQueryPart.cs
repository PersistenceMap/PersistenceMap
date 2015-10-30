using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace PersistenceMap.QueryParts
{
    public class FieldQueryPart : QueryPart, IFieldPart, IEntityPart, IQueryPart
    {
        public FieldQueryPart(string field, string fieldalias, string entityalias = null, string entity = null, Type entityType = null, string id = null, Expression<Func<object, object>> converter = null, OperationType operation = OperationType.Field)
            : base(operation, entityType, id)
        {
            EntityAlias = entityalias;
            Field = field;
            FieldAlias = fieldalias;
            Entity = entity;
            ID = id ?? fieldalias ?? field;
            Converter = converter;
            OperationType = operation;
        }

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

        //public OperationType OperationType { get; set; }

        public override string Compile()
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(EntityAlias) || !string.IsNullOrEmpty(Entity))
                sb.Append(string.Format("{0}.", EntityAlias ?? Entity));

            sb.Append(Field);
            
            if (!string.IsNullOrEmpty(FieldAlias))
                sb.Append(string.Format(" AS {0}", FieldAlias));

            if (string.IsNullOrEmpty(Sufix) == false)
                sb.Append(Sufix);

            return sb.ToString();
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0} - Operation [{1}] Entity: [{2}] Field: [{3}] [{3}.{4}]", GetType().Name, OperationType, Entity, EntityAlias ?? Entity, Field);
        }


        internal static void FiedlPartsFactory(SelectQueryPartsContainer queryParts, FieldQueryPart[] fields)
        {
            //TODO: this method should be removed!
            foreach (var map in queryParts.Parts.OfType<IItemsQueryPart>().Where(p => p.OperationType == OperationType.Select))
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

                    if (map.IsSealed)
                    {
                        continue;
                    }

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
        public IgnoreFieldQueryPart(string field, string fieldalias, string entityalias = null, string entity = null, Type entityType = null, string id = null) :
            base(field, fieldalias, entityalias, entity, entityType, id)
        {
            OperationType = OperationType.IgnoreColumn;
        }

        public override string Compile()
        {
            return "";
        }
    }
}
