using PersistanceMap.Compiler;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PersistanceMap.QueryBuilder.Decorators
{
    internal class EntityQueryPart<T> : IEntityQueryPart, IQueryMapCollection, IQueryPart
    {
        public EntityQueryPart(string entity)
            : this(entity, null)
        {
        }

        public EntityQueryPart(string entity, string alias)
            : this(entity, alias, new IQueryMap[0])
        {
        }

        public EntityQueryPart(string entity, string alias, IQueryMap[] mapCollection)
        {
            // ensure parameter is not null
            mapCollection.EnsureArgumentNotNull("mapCollection");

            MapCollection = mapCollection.ToList();
            EntityAlias = alias;
            Entity = entity;
        }

        #region IQueryMapCollection Implementation

        IEnumerable<IQueryMap> IQueryMapCollection.MapCollection
        {
            get
            {
                return MapCollection;
            }
        }

        public IList<IQueryMap> MapCollection { get; private set; }

        #endregion

        #region IEntityQueryPart Implementation

        public string Entity { get; private set; }

        public string EntityAlias { get; set; }

        #endregion

        #region IQueryPart Implementation

        public MapOperationType MapOperationType { get; set; }

        public virtual string Compile()
        {
            var sb = new StringBuilder();

            switch (MapOperationType)
            {
                case MapOperationType.From:
                    sb.Append("from");
                    break;

                case MapOperationType.Join:
                    sb.Append("join");
                    break;

                case MapOperationType.LeftJoin:
                    sb.Append("left join");
                    break;

                case MapOperationType.RightJoin:
                    sb.Append("right join");
                    break;

                case MapOperationType.FullJoin:
                    sb.Append("full join");
                    break;
            }

            sb.Append(string.Format(" {0}{1} ", Entity, string.IsNullOrEmpty(EntityAlias) ? string.Empty : string.Format(" {0}", EntityAlias)));

            // compile all mappings that belong to the part (on, and, or...)
            MapCollection.ForEach(a => sb.Append(a.Compile()));

            return sb.ToString();
        }

        #endregion

        public override string ToString()
        {
            if (string.IsNullOrEmpty(EntityAlias))
                return string.Format("Entity: [{0}] Operation: [{1}]", Entity, MapOperationType);

            return string.Format("Entity: [{0} {1}] Operation: [{2}]", Entity, EntityAlias, MapOperationType);
        }
    }
}
