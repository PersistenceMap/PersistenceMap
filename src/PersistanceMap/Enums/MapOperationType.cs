
namespace PersistanceMap
{
    public enum MapOperationType
    {
        None,

        /// <summary>
        /// defines the operation as a from operation
        /// </summary>
        From,

        /// <summary>
        /// defines the operation as a inner join operation
        /// </summary>
        Join,

        LeftJoin,

        RightJoin,

        FullJoin,

        /// <summary>
        /// defines the operation as an alias of a table, a field or a storedprocedure parameter
        /// </summary>
        As,

        /// <summary>
        /// defines the operation that the field is included in the resultset
        /// </summary>
        Include,

        /// <summary>
        /// defines the operation as a join operation
        /// </summary>
        JoinOn,

        AndOn,

        OrOn,

        /// <summary>
        /// defines the value of a storeprocedure parameter
        /// </summary>
        Value,

        /// <summary>
        /// defines the operation to be a parameter
        /// </summary>
        Parameter,

        /// <summary>
        /// defines the operation as the prefix of a out parameter (declare @outparam)
        /// </summary>
        OutParameterPrefix,

        /// <summary>
        /// defines the operation as a sufix of a out parameter (select @outparam)
        /// </summary>
        OutParameterSufix
    }
}
