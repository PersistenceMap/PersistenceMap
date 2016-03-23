
namespace PersistenceMap.QueryParts
{
    public class SelectQueryPartsContainer : QueryPartsContainer, IQueryPartsContainer
    {
        #region IQueryPartsContainer Implementation

        public override void Add(IQueryPart part)
        {
            switch (part.OperationType)
            {
                case OperationType.Include:
                    var field = part as FieldQueryPart;
                    if (field != null)
                    {
                        // add the field to the last QueryPart of type SelectionMap (select a,b,c...)
                        AddToLast(field, OperationType.Select);
                    }

                    if (AggregatePart == null)
                    {
                        AggregatePart = part;
                        AggregateType = part.EntityType;
                    }

                    break;
                    
                default:
                    base.Add(part);
                    break;
            }
        }
        
        #endregion
    }
}
