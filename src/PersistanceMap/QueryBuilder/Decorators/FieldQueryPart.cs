
namespace PersistanceMap.QueryBuilder.Decorators
{
    internal class FieldQueryPart : IEntityQueryPart, IQueryPart
    {
        public FieldQueryPart(string field, string identifier)
            : this(field, identifier, null)
        {
        }

        public FieldQueryPart(string field, string identifier, string entity)
        {
            Identifier = identifier;
            Field = field;
            Entity = entity;
        }

        public MapOperationType MapOperationType { get; set; }

        public string Entity { get; private set; }

        public string Identifier { get; set; }

        public string Field { get; private set; }

        public string Compile()
        {
            if (string.IsNullOrEmpty(Identifier))
                return Field;

            return string.Format("{0}.{1}", Identifier ?? Entity, Field);
        }

        public override string ToString()
        {
            return string.Format("Entity: {0} Field: {1} [{1}.{2}]", Entity, Identifier ?? Entity, Field);
        }
    }
}
