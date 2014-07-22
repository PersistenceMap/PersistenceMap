
namespace PersistanceMap
{
    public enum MapOperationType
    {
        None,

        From,

        Join,

        InnerJoin,

        LeftOuterJoin,

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
        JoinOn,

        /// <summary>
        /// 
        /// </summary>
        AndOn,

        /// <summary>
        /// 
        /// </summary>
        OrOn,

        /// <summary>
        /// defines the value of a storeprocedure parameter
        /// </summary>
        Value,

        Parameter,

        OutputParameterDefinition,
    }
}
