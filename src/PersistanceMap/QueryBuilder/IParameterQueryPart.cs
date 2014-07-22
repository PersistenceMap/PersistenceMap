
namespace PersistanceMap.QueryBuilder
{
    public interface IParameterQueryPart : 
        /*ICallbackQueryPart,*/
        ICallbackHandlerQueryPart, 
        /*INamedQueryPart, */
        IQueryMapCollection, 
        IQueryPart
    {
    }
}
