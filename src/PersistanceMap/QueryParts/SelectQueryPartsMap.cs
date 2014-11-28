using PersistanceMap.Factories;
using PersistanceMap.QueryBuilder;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistanceMap.QueryParts
{
    public class SelectQueryPartsMap : QueryPartsMap, IQueryPartsMap
    {
        #region Properties

        public IEnumerable<IEntityMap> Joins
        {
            get
            {
                return Parts.Where(p => 
                    (p.OperationType == OperationType.From || p.OperationType == OperationType.Join || p.OperationType == OperationType.LeftJoin || p.OperationType == OperationType.RightJoin || p.OperationType == OperationType.FullJoin)
                    && p is IEntityMap).Cast<IEntityMap>();
            }
        }

        #endregion

        #region IQueryPartsMap Implementation

        public override void Add(IQueryPart map)
        {
            switch (map.OperationType)
            {
                case OperationType.From:
                case OperationType.Join:
                case OperationType.LeftJoin:
                case OperationType.RightJoin:
                case OperationType.FullJoin:
                    //var entity = map as IEntityQueryPart;
                    //entity.EnsureArgumentNotNull("map");

                    //Parts.Add(entity);
                    Parts.Add(map);

                    break;

                case OperationType.Include:
                    var field = map as FieldQueryPart;
                    //if (field == null)
                    //{
                    //    // try to create a field query part
                    //    var expr = map as IExpressionMap;
                    //    if (expr != null)
                    //    {
                    //        var last = Joins.LastOrDefault();
                    //        var id = last != null ? string.IsNullOrEmpty(last.EntityAlias) ? last.Entity : last.EntityAlias : null;
                    //        var ent = last != null ? last.Entity : null;

                    //        field = new FieldQueryPart(FieldHelper.TryExtractPropertyName(expr.Expression), id, ent)
                    //        {
                    //            OperationType = OperationType.Include
                    //        };
                    //    }
                    //}

                    if (field != null)
                    {
                        // add the field to the last QueryPart of type SelectionMap (select a,b,c...)
                        AddToLast(field, OperationType.Select);
                    }
                    break;
                    
                default:
                    Parts.Add(map);
                    break;
            }
        }

        public override CompiledQuery Compile()
        {
            var sb = new StringBuilder(100);

            // loop all parts and compile
            foreach (var part in Parts)
            {
                switch (part.OperationType)
                {
                    case OperationType.OrderBy:
                    case OperationType.OrderByDesc:

                        var index = Parts.IndexOf(part);
                        if (index + 1 < Parts.Count)
                        {
                            var item = Parts[index + 1];
                            if (item.OperationType == OperationType.ThenBy || item.OperationType == OperationType.ThenByDesc)
                            {
                                sb.Append(part.Compile());
                            }
                            else
                                sb.AppendLine(part.Compile());
                        }
                        else
                            sb.AppendLine(part.Compile());
                        break;

                    case OperationType.ThenBy:
                    case OperationType.ThenByDesc:
                        sb.Append(part.Compile());
                        break;

                    default:
                        sb.AppendLine(part.Compile());
                        break;
                }
            }

            return new CompiledQuery
            {
                QueryString = sb.ToString(),
                QueryParts = this
            };
        }

        #endregion
    }
}
