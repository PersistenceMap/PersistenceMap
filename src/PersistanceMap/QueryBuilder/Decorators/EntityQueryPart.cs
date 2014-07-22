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

        public EntityQueryPart(string entity, string identifier)
            : this(entity, identifier, new IQueryMap[0])
        {
        }

        public EntityQueryPart(string entity, string identifier, IQueryMap[] mapCollection)
        {
            // ensure parameter is not null
            mapCollection.EnsureArgumentNotNull("mapCollection");

            MapCollection = mapCollection.ToList();
            Identifier = identifier;
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

        public string Identifier { get; set; }

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

                //case MapOperationType.InnerJoin:
                //    sb.Append("inner join");
                //    break;

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

            sb.Append(string.Format(" {0}{1} ", Entity, string.IsNullOrEmpty(Identifier) ? string.Empty : string.Format(" {0}", Identifier)));

            // compile all mappings that belong to the part (on, and, or...)
            MapCollection.ForEach(a => sb.Append(a.Compile()));

            return sb.ToString();
        }

        #endregion

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Identifier))
                return string.Format("Entity: {0} [{0}]", Entity);

            return string.Format("Entity: {0} [{0} {1}]", Entity, Identifier);
        }
    }
}
