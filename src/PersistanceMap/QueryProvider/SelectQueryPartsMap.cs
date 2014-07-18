using System.Text;
using PersistanceMap.QueryBuilder;
using System.Collections.Generic;
using System.Linq;

namespace PersistanceMap
{
    public class SelectQueryPartsMap : IQueryPartsMap
    {
        #region Properties

        IList<IEntityQueryPart> _fields;
        public IList<IEntityQueryPart> Fields
        {
            get
            {
                if (_fields == null)
                    _fields = new List<IEntityQueryPart>();
                return _fields;
            }
        }

        IList<ISelectQueryPart> _joins;
        public IList<ISelectQueryPart> Joins
        {
            get
            {
                if (_joins == null)
                    _joins = new List<ISelectQueryPart>();
                return _joins;
            }
        }

        public ISelectQueryPart From { get; private set; }

        public IQueryPart Where { get; private set; }

        public IQueryPart Order { get; private set; }

        /// <summary>
        /// Indicates if the resultset is a distinct set of fields provided by a 'For' expression
        /// </summary>
        internal bool IsResultSetComplete { get; private set; }

        #endregion

        #region Add Methods

        internal void Add(FieldQueryPart field, bool replace)
        {
            // dont add any more fields when the resultset is defined
            if (IsResultSetComplete)
                return;

            if (Fields.Any(f => ((FieldQueryPart)f).Field == field.Field))
            {
                if (!replace)
                    return;


                if (Fields.Any(f => ((FieldQueryPart)f).Field == field.Field && ((FieldQueryPart)f).Entity == field.Entity && ((FieldQueryPart)f).Identifier == field.Identifier))
                {
                    // remove existing field map
                    Fields.Remove(Fields.First(f => ((FieldQueryPart)f).Field == field.Field));
                }
            }

            Fields.Add(field);
        }

        internal void Add<T>(FromQueryPart<T> entity)
        {
            if (Joins.Any(j => j.Entity == entity.Entity && j.Identifier == entity.Identifier))
            {
                var entname = entity.Entity;
                entity.Identifier = string.Format("{0}0", entname);
                
                int id = 1;
                Joins.Where(j => j.Entity == entity.Entity && j.Identifier == entity.Identifier)
                    .ToList()
                    .ForEach(e => e.Identifier = string.Format("{0}{1}", entname, id++));
            }

            From = entity;
        }

        internal void Add<T>(JoinQueryPart<T> join)
        {
            if ((From != null && From.Entity == join.Entity && From.Identifier ==join.Identifier) || Joins.Any(j => j.Entity == join.Entity && j.Identifier == join.Identifier))
            {
                var entname = join.Entity;

                if (From != null && From.Entity == join.Entity && From.Identifier == join.Identifier)
                    From.Identifier = string.Format("{0}0", entname);

                int id = 1;
                join.Identifier = string.Format("{0}{1}", entname, id++);
                Joins.Where(j => j.Entity == join.Entity && j.Identifier == join.Identifier)
                    .ToList()
                    .ForEach(e => e.Identifier = string.Format("{0}{1}", entname, id++));
            }

            Joins.Add(join);
        }

        #endregion

        #region IQueryPartsMap Implementation

        

        #endregion
    }
}
