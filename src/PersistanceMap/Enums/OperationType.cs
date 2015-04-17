
using System;

namespace PersistanceMap
{
    public enum OperationType
    {
        None = 0, /* */

        /// <summary>
        /// defines the operation to be the begining of a select operation (select a, b, c)
        /// </summary>
        Select = 1, /* */

        /// <summary>
        /// defines the operation as a from operation
        /// </summary>
        From = 2, /* */

        /// <summary>
        /// defines the operation as a inner join operation
        /// </summary>
        Join = 3, /* */

        LeftJoin = 4,

        RightJoin = 5,

        FullJoin = 6,

        Where = 7, /* */

        Max = 16, /* */

        Min = 17, /* */

        Count = 18, /* */

        /// <summary>
        ///// defines the operation as an alias of a table, a field or a storedprocedure parameter
        ///// </summary>
        //As = 20,

        //AliasMap,

        /// <summary>
        /// defines the operation that the field is included in the resultset
        /// </summary>
        Include = 21, /* */

        Field = 22, /* */

        /// <summary>
        /// defines the operation that maps the expression needed for a join operation
        /// </summary>
        On = 23, /* */

        And = 24, /* */

        Or = 25, /* */

        GroupBy = 30, /* */

        OrderBy = 26, /* */

        OrderByDesc = 27, /* */

        ThenBy = 28, /* */
        ThenByAsc = 46, /* */
        ThenByDesc = 29, /* */


        // Database
        CreateDatabase = 33,
        CreateTable = 34,
        Drop = 35,
        RenameTable = 36,
        AlterTable = 37,
        Column = 38,
        IgnoreColumn = 39, /* */
        TableKeys = 40,
        AddField = 41,
        DropField = 42,




        /// <summary>
        /// Defines the operation to be the beginning of a delete operation 
        /// </summary>
        Delete = 10,

        Update = 11,

        //Set = 12,

        Insert = 13,

        InsertMember = 43,
        InsertValue = 44,
        UpdateValue = 45,
        //Into = 14,

        /// <summary>
        /// defines a values element for a insert
        /// </summary>
        Values = 15,


        ///// <summary>
        ///// defines the value of a storeprocedure parameter
        ///// </summary>
        //Value = 60,


        Procedure = 60, /* */

        /// <summary>
        /// defines the operation to be a parameter
        /// </summary>
        Parameter = 61, /* */

        OutputParameter = 64,

        /// <summary>
        /// Defines the element to be a container containing all elements needed for a output definition
        /// </summary>
        OutParameterDefinition = 66,

        /// <summary>
        /// defines the operation as the definition of a out parameter (declare @outparam)
        /// </summary>
        OutParameterDeclare = 62, /* */

        OutParameterSet = 65,

        /// <summary>
        /// defines the operation as a select parameter definition of a out parameter (select @outparam as outparam)
        /// </summary>
        OutParameterSelect = 63, /* */

        /// <summary>
        /// Defines a Part to be a Member definition that is only used for mappings. These parts are ignored when compiling the sql
        /// </summary>
        IncludeMember = 47
        //AfterMap = 200
    }
}
