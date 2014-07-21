using PersistanceMap.QueryBuilder;
using System.Linq;
using System.Text;
using PersistanceMap.Sql;

namespace PersistanceMap.Compiler
{
    //public abstract class QueryCompiler
    //{
    //    public abstract CompiledQuery Compile();
    //}

    /// <summary>
    /// Compiles all elements of a QueryPartsContainer to a CompiledQuery
    /// </summary>
    //public class SelectQueryCompiler : QueryCompiler
    //{
    //    private readonly SelectQueryPartsMap _queryParts;

    //    public SelectQueryCompiler(SelectQueryPartsMap queryParts)
    //    {
    //        _queryParts = queryParts;
    //    }

    //    public override CompiledQuery Compile()
    //    {
    //        if (_queryParts == null)
    //            return null;

    //        var sb = new StringBuilder(100);
    //        sb.Append("select ");

    //        // add resultset fields
    //        foreach (var field in _queryParts.Fields)
    //            sb.AppendFormat("{0}{1} ", field.Compile(), _queryParts.Fields.Last() == field ? "" : ",");

    //        // add from
    //        sb.AppendFormat("{0} \r\n", _queryParts.From.Compile());

    //        // add joins
    //        foreach (var join in _queryParts.Joins)
    //            sb.Append(join.Compile());

    //        // where

    //        // order...

    //        return new CompiledQuery
    //        {
    //            QueryString = sb.ToString(),
    //            QueryParts = _queryParts
    //        };
    //    }
    //}

    /// <summary>
    /// Compiles all elements of a QueryPartsContainer to a CompiledQuery
    /// </summary>
    //public class ProcedureQueryCompiler : QueryCompiler
    //{
    //    private readonly ProcedureQueryPartsMap _queryParts;

    //    public ProcedureQueryCompiler(ProcedureQueryPartsMap queryParts)
    //    {
    //        _queryParts = queryParts;
    //    }

    //    public override CompiledQuery Compile()
    //    {
    //        if (_queryParts == null)
    //            return null;

    //        /* *Using Output compiles to*                
    //        declare @p1 datetime
    //        set @p1='2012-01-01 00:00:00'
    //        exec procedure @param1=@p1 output,@param2='2014-07-15 00:00:00'
    //        select @p1                
    //        */

    //        var sb = new StringBuilder(100);

    //        // prepare outputparameters
    //        int i = 1;
    //        foreach (var param in _queryParts.Parameters.Where(p => p.CanHandleCallback))
    //        {
    //            // creates a name for the output parameter
    //            var definition = param.CompileOutParameter(i);
    //            if (!string.IsNullOrEmpty(definition))
    //            {
    //                sb.AppendLine(definition);

    //                i++;
    //            }
    //        }

    //        // create the exec statement
    //        sb.Append(string.Format("exec {0} ", _queryParts.ProcedureName));

    //        var conv = new LambdaExpressionToSqlCompiler();
    //        conv.PrefixFieldWithTableName = false;

    //        // add parameters
    //        foreach (var param in _queryParts.Parameters)
    //        {
    //            var value = param.Compile();
    //            sb.Append(string.Format("{0}{1}", value, _queryParts.Parameters.Last() == param ? "" : ", "));
    //        }

    //        // add the select for all output parameters
    //        var lastCallback = _queryParts.Parameters.LastOrDefault(p => p.CanHandleCallback);
    //        var selectoutput = string.Empty;
    //        foreach (var param in _queryParts.Parameters.Where(p => p.CanHandleCallback))
    //        {
    //            if (string.IsNullOrEmpty(selectoutput))
    //                selectoutput = "select";

    //            if (!string.IsNullOrEmpty(param.CallbackParameterName))
    //            {
    //                selectoutput = string.Format("{0} @{1} as {1}{2}", selectoutput, param.CallbackParameterName, lastCallback == param ? "" : ", ");
    //            }
    //        }

    //        sb.AppendLine();
    //        sb.AppendLine(selectoutput);

    //        return new CompiledQuery
    //        {
    //            QueryString = sb.ToString(),
    //            QueryParts = _queryParts
    //        };
    //    }
    //}
}
