
namespace PersistanceMap
{
    public interface IQueryExpression
    {
        /// <summary>
        /// The database context
        /// </summary>
        IDatabaseContext Context { get; }

        /// <summary>
        /// The container containing all queryparts needed for the sql expression
        /// </summary>
        IQueryPartsMap QueryPartsMap { get; }
    }
}
