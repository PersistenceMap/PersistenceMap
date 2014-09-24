using System.Text;

namespace PersistanceMap.QueryParts
{
    internal class EntityQueryPart<T> : QueryPartDecorator, IEntityQueryPart, IQueryPartDecorator, IQueryPart
    {
        public EntityQueryPart(string entity)
            : this(entity, null)
        {
        }

        public EntityQueryPart(string entity, string alias)
            : this(entity, alias, new IExpressionQueryPart[0])
        {
        }

        public EntityQueryPart(string entity, string alias, IQueryPart[] parts)
            : base(parts)
        {
            EntityAlias = alias;
            Entity = entity;
        }

        #region IEntityQueryPart Implementation

        public string Entity { get; private set; }

        public string EntityAlias { get; set; }

        #endregion

        #region IQueryPart Implementation

        public override string Compile()
        {
            var sb = new StringBuilder();

            switch (OperationType)
            {
                case OperationType.From:
                    sb.Append("from");
                    break;

                case OperationType.Join:
                    sb.Append("join");
                    break;

                case OperationType.LeftJoin:
                    sb.Append("left join");
                    break;

                case OperationType.RightJoin:
                    sb.Append("right join");
                    break;

                case OperationType.FullJoin:
                    sb.Append("full join");
                    break;

                case PersistanceMap.OperationType.Update:
                    sb.Append("UPDATE");
                    break;

                case PersistanceMap.OperationType.Insert:
                    sb.Append("INSERT INTO");
                    break;
            }

            sb.Append(string.Format(" {0}{1} ", Entity, string.IsNullOrEmpty(EntityAlias) ? string.Empty : string.Format(" {0}", EntityAlias)));

            // compile all mappings that belong to the part (on, and, or...)
            //Parts.ForEach(a => sb.Append(a.Compile()));
            var value = base.Compile();
            if (!string.IsNullOrEmpty(value))
                sb.Append(value);


            return sb.ToString().RemoveLineBreak();
        }

        #endregion

        public override string ToString()
        {
            if (string.IsNullOrEmpty(EntityAlias))
                return string.Format("Entity: [{0}] Operation: [{1}]", Entity, OperationType);

            return string.Format("Entity: [{0} {1}] Operation: [{2}]", Entity, EntityAlias, OperationType);
        }
    }
}
