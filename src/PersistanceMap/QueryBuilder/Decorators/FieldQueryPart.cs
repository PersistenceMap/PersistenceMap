
namespace PersistanceMap.QueryBuilder.Decorators
{
    internal class FieldQueryPart : IEntityQueryPart, IQueryPart
    {
        public FieldQueryPart(string field, string entityalias)
            : this(field, entityalias, null)
        {
        }

        public FieldQueryPart(string field, string entityalias, string entity)
        {
            EntityAlias = entityalias;
            Field = field;
            Entity = entity;
        }

        public MapOperationType MapOperationType { get; set; }

        public string Entity { get; private set; }

        public string EntityAlias { get; set; }

        public string Field { get; private set; }

        public string Compile()
        {
            if (string.IsNullOrEmpty(EntityAlias))
                return Field;

            return string.Format("{0}.{1}", EntityAlias ?? Entity, Field);
        }

        public override string ToString()
        {
            return string.Format("Entity: {0} Field: {1} [{1}.{2}]", Entity, EntityAlias ?? Entity, Field);
        }
    }
}
