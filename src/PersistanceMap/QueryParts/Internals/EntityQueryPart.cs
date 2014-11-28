using System.Text;

namespace PersistanceMap.QueryParts
{
    internal class EntityQueryPart : QueryPartDecorator, IEntityMap, IQueryPartDecorator, IQueryPart
    {
        public EntityQueryPart(string entity)
            : this(entity, null)
        {
        }

        public EntityQueryPart(string entity, string alias)
            : this(entity, alias, new IQueryPart[0])
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
                //case OperationType.From:
                //    sb.Append("from");
                //    break;

                //case OperationType.Join:
                //    sb.Append("join");
                //    break;

                case OperationType.LeftJoin:
                    sb.Append("left join");
                    break;

                case OperationType.RightJoin:
                    sb.Append("right join");
                    break;

                case OperationType.FullJoin:
                    sb.Append("full join");
                    break;
            }

            sb.Append(string.Format(" {0}{1} ", Entity, string.IsNullOrEmpty(EntityAlias) ? string.Empty : string.Format(" {0}", EntityAlias)));

            sb.Append(base.Compile() ?? "");

            return sb.ToString().RemoveLineBreak();
        }

        #endregion

        public override string ToString()
        {
            if (string.IsNullOrEmpty(EntityAlias))
                return string.Format("{0} - Entity: [{1}] Operation: [{2}]", GetType().Name, Entity, OperationType);

            return string.Format("{0} - Entity: [{1} {2}] Operation: [{3}]", GetType().Name, Entity, EntityAlias, OperationType);
        }
    }
}
