using PersistanceMap.QueryBuilder;
using System.Text;

namespace PersistanceMap.QueryParts
{
    public class SelectQueryPartsContainer : QueryPartsContainer, IQueryPartsContainer
    {
        #region IQueryPartsContainer Implementation

        public override void Add(IQueryPart map)
        {
            switch (map.OperationType)
            {
                case OperationType.Include:
                    var field = map as FieldQueryPart;
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
