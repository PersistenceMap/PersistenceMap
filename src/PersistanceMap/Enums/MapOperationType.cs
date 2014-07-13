
namespace PersistanceMap
{
    public enum MapOperationType
    {
        /// <summary>
        /// Defines the Operation as a identifier of a table
        /// </summary>
        Identifier,

        /// <summary>
        /// Defines the operation that the field is included in the resultset
        /// </summary>
        Include,

        /// <summary>
        /// defines the operation as a join operation
        /// </summary>
        Join,

        /// <summary>
        /// 
        /// </summary>
        And,

        /// <summary>
        /// 
        /// </summary>
        Or
    }
}
