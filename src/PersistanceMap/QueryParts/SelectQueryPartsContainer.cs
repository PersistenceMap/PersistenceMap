
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
        
        #endregion
    }
}
