
using System;

namespace PersistanceMap
{
    //[Flags]
    public enum OperationType
    {
        None = 0,

        /// <summary>
        /// defines the operation to be the begining of a select operation (select a, b, c)
        /// </summary>
        SelectMap = 1,

        /// <summary>
        /// defines the operation as a from operation
        /// </summary>
        From = 2,

        /// <summary>
        /// defines the operation as a inner join operation
        /// </summary>
        Join = 3,

        LeftJoin = 4,

        RightJoin = 5,

        FullJoin = 6,

        Where = 7,




        /// <summary>
        /// defines the operation as an alias of a table, a field or a storedprocedure parameter
        /// </summary>
        As = 20,

        //AliasMap,

        /// <summary>
        /// defines the operation that the field is included in the resultset
        /// </summary>
        Include = 21,

        /// <summary>
        /// defines the operation that maps the expression needed for a join operation
        /// </summary>
        On = 22,

        And = 24,

        Or = 25,







        /// <summary>
        /// defines the value of a storeprocedure parameter
        /// </summary>
        Value = 40,

        /// <summary>
        /// defines the operation to be a parameter
        /// </summary>
        Parameter = 41,

        /// <summary>
        /// defines the operation as the prefix of a out parameter (declare @outparam)
        /// </summary>
        OutParameterPrefix = 42,

        /// <summary>
        /// defines the operation as a sufix of a out parameter (select @outparam)
        /// </summary>
        OutParameterSufix = 43
    }
}
