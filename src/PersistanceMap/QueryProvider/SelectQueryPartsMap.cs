using System.Text;
using PersistanceMap.QueryBuilder;
using System.Collections.Generic;
using System.Linq;
using PersistanceMap.QueryBuilder.Decorators;
using System.Linq.Expressions;
using System.Reflection;
using System;
using PersistanceMap.Internals;

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

        //TODO: Rename to Expressions or statements or something else. Joins is wrong because ist From, Joins, Wheres,...
        IList<IEntityQueryPart> _joins;
        public IList<IEntityQueryPart> Joins
        {
            get
            {
                if (_joins == null)
                    _joins = new List<IEntityQueryPart>();
                return _joins;
            }
        }

        //public IEntityQueryPart From { get; private set; }

        //public IQueryPart Where { get; private set; }

        //public IQueryPart Order { get; private set; }

        ///// <summary>
        ///// Indicates if the resultset is a distinct set of fields provided by a 'For' expression
        ///// </summary>
        //internal bool IsResultSetComplete { get; private set; }

        #endregion

        #region Add Methods

        internal void Add(FieldQueryPart field, bool replace)
        {
            if (Fields.Any(f => ((FieldQueryPart)f).Field == field.Field))
            {
                if (!replace)
                    return;

                if (Fields.Any(f => ((FieldQueryPart)f).Field == field.Field && ((FieldQueryPart)f).Entity == field.Entity && ((FieldQueryPart)f).EntityAlias == field.EntityAlias))
                {
                    // remove existing field map
                    Fields.Remove(Fields.First(f => ((FieldQueryPart)f).Field == field.Field));
                }
            }

            Fields.Add(field);
            //TODO: don't use 2 collections!
            Parts.Add(field);
        }

        #endregion

        #region IQueryPartsMap Implementation

        public void Add(IQueryPart map)
        {
            switch (map.MapOperationType)
            {
                case MapOperationType.From:
                case MapOperationType.Join:
                case MapOperationType.LeftJoin:
                case MapOperationType.RightJoin:
                case MapOperationType.FullJoin:
                    var entity = map as IEntityQueryPart;
                    entity.EnsureArgumentNotNull("map");
                    Joins.Add(entity);
                    
                    //TODO: don't use 2 collections!
                    Parts.Add(entity);

                    break;

                case MapOperationType.Include:
                    var field = map as FieldQueryPart;
                    if (field == null)
                    {
                        // try to create a field query part
                        var expr = map as IQueryMap;
                        if (expr != null)
                        {
                            var last = Joins.LastOrDefault();
                            var id = last != null ? string.IsNullOrEmpty(last.EntityAlias) ? last.Entity : last.EntityAlias : null;
                            var ent = last != null ? last.Entity : null;

                            field = new FieldQueryPart(FieldHelper.TryExtractPropertyName(expr.Expression), id, ent)
                            {
                                MapOperationType = MapOperationType.Include
                            };
                        }
                    }

                    if (field != null)
                        Add(field, true);
                    break;
                    
                default:
                    throw new System.NotImplementedException();
            }
        }

        public void AddBefore(MapOperationType operation, IQueryPart part)
        {
            var first = Parts.FirstOrDefault(p => p.MapOperationType == operation);
            var index = Parts.IndexOf(first);
            if (index < 0)
                index = 0;

            Parts.Insert(index, part);
        }

        public void AddAfter(MapOperationType operation, IQueryPart part)
        {
            var first = Parts.LastOrDefault(p => p.MapOperationType == operation);
            var index = Parts.IndexOf(first) + 1;
            //if (index > Parts.Count)
            //    index = 0;

            Parts.Insert(index, part);
        }

        IEnumerable<IQueryPart> IQueryPartsMap.Parts
        {
            get
            {
                return Parts;
            }
        }

        private IList<IQueryPart> _parts;
        public IList<IQueryPart> Parts
        {
            get
            {
                if (_parts == null)
                    _parts = new List<IQueryPart>();
                return _parts;
            }
        }
        
        public CompiledQuery Compile()
        {
            var sb = new StringBuilder(100);
            sb.Append("select ");

            // add resultset fields
            foreach (var field in Fields)
                sb.AppendFormat("{0}{1} ", field.Compile(), Fields.Last() == field ? "" : ",");

            // add joins
            foreach (var join in Joins)
            {
                sb.AppendLine(join.Compile());
            }

            // where

            // order...

            return new CompiledQuery
            {
                QueryString = sb.ToString(),
                QueryParts = this
            };
        }

        #endregion
    }
}
