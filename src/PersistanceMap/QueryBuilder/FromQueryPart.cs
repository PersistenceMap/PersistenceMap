
namespace PersistanceMap.QueryBuilder
{
    class FromQueryPart : IEntityQueryPart
    {
        public FromQueryPart(string entity)
            : this(entity, null)
        {
        }

        public FromQueryPart(string entity, string identifier)
        {
            Identifier = identifier;
            Entity = entity;
        }

        public string Entity { get; private set; }

        public string Identifier { get; set; }

        public string Compile()
        {
            if(string.IsNullOrEmpty(Identifier))
                return string.Format("from {0}", Entity);

            return string.Format("from {0} {1}", Entity, Identifier);
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Identifier))
                return string.Format("Entity: {0} [{0}]", Entity);

            return string.Format("Entity: {0} [{0} {1}]", Entity, Identifier);
        }
    }
}
