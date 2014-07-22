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

        public void Add(IQueryPart map)
        {
            switch (map.MapOperationType)
            {
                case MapOperationType.From:
                case MapOperationType.Join:
                    var entity = map as IEntityQueryPart;
                    entity.EnsureArgumentNotNull("map");
                    Joins.Add(entity);
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
                            var id = last != null ? string.IsNullOrEmpty(last.Identifier) ? last.Entity : last.Identifier : null;
                            var ent = last != null ? last.Entity : null;

                            field = new FieldQueryPart(FieldHelper.ExtractPropertyName(expr.Expression), id, ent)
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

        internal void Add(FieldQueryPart field, bool replace)
        {
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

        //internal void Add<T>(EntityQueryPart<T> entity)
        //{
        //    if (Joins.Any(j => j.Entity == entity.Entity && j.Identifier == entity.Identifier))
        //    {
        //        var entname = entity.Entity;
        //        entity.Identifier = string.Format("{0}0", entname);
                
        //        int id = 1;
        //        Joins.Where(j => j.Entity == entity.Entity && j.Identifier == entity.Identifier)
        //            .ToList()
        //            .ForEach(e => e.Identifier = string.Format("{0}{1}", entname, id++));
        //    }

        //    From = entity;
        //}

        //internal void Add(IEntityQueryPart join)
        //{
        //    if ((From != null && From.Entity == join.Entity && From.Identifier == join.Identifier) || Joins.Any(j => j.Entity == join.Entity && j.Identifier == join.Identifier))
        //    {
        //        var entname = join.Entity;

        //        if (From != null && From.Entity == join.Entity && From.Identifier == join.Identifier)
        //            From.Identifier = string.Format("{0}0", entname);

        //        int id = 1;
        //        join.Identifier = string.Format("{0}{1}", entname, id++);
        //        Joins.Where(j => j.Entity == join.Entity && j.Identifier == join.Identifier)
        //            .ToList()
        //            .ForEach(e => e.Identifier = string.Format("{0}{1}", entname, id++));
        //    }

        //    Joins.Add(join);
        //}

        #endregion

        #region IQueryPartsMap Implementation

        public CompiledQuery Compile()
        {
            //if (_queryParts == null)
            //    return null;

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
