
namespace PersistanceMap
{
    public enum MapOperationType
    {
        /// <summary>
        /// Defines the Operation as a identifier of a table or a storedprocedure parameter
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
        Or,

        /// <summary>
        /// defines the value of a storeprocedure parameter
        /// </summary>
        Value
    }
}
