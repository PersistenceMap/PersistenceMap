using System.Linq;
using System.Text;
using PersistanceMap.Compiler;
using PersistanceMap.QueryBuilder;
using System.Collections.Generic;

namespace PersistanceMap
{
    public class ProcedureQueryPartsMap : QueryPartsMap, IQueryPartsMap
    {
        public ProcedureQueryPartsMap(string procedure)
        {
            procedure.EnsureArgumentNotNullOrEmpty("procedure");

            ProcedureName = procedure;
        }

        #region IQueryPartsMap Implementation

        public override CompiledQuery Compile()
        {
            /* *Using Output compiles to*                
            declare @p1 datetime
            set @p1='2012-01-01 00:00:00'
            exec procedure @param1=@p1 output,@param2='2014-07-15 00:00:00'
            select @p1                
            */

            var sb = new StringBuilder(100);

            // prepare outputparameters
            foreach (var param in Parts.Where(p => p.OperationType == OperationType.OutParameterPrefix))
            {
                // creates a name for the output parameter
                var value = param.Compile();
                if (!string.IsNullOrEmpty(value))
                    sb.AppendLine(value);
            }

            // create the exec statement
            sb.Append(string.Format("exec {0} ", ProcedureName));

            var conv = new LambdaExpressionToSqlCompiler();
            conv.PrefixFieldWithTableName = false;

            // add parameters
            foreach (var param in Parameters)
            {
                var value = param.Compile();
                sb.Append(string.Format("{0}{1}", value, Parameters.Last() == param ? "" : ", "));
            }

            // add the select for all output parameters
            var selectoutput = string.Empty;
            foreach (var param in Parts.Where(p => p.OperationType == OperationType.OutParameterSufix))
            {
                bool separator = true;
                if (string.IsNullOrEmpty(selectoutput))
                {
                    selectoutput = "select";
                    separator = false;
                }

                var value = param.Compile();
                if (!string.IsNullOrEmpty(value))
                    selectoutput = string.Format("{0} {1}{2}", selectoutput, separator ? ", " : "", value);
            }

            sb.AppendLine();
            sb.AppendLine(selectoutput);

            return new CompiledQuery
            {
                QueryString = sb.ToString(),
                QueryParts = this
            };
        }

        #endregion

        #region Properties

        //IEnumerable<IQueryPart> IQueryPartsMap.Parts
        //{
        //    get
        //    {
        //        return Parts;
        //    }
        //}

        //private IList<IQueryPart> _parts;
        //public IList<IQueryPart> Parts
        //{
        //    get
        //    {
        //        if (_parts == null)
        //            _parts = new List<IQueryPart>();
        //        return _parts;
        //    }
        //}

        internal IEnumerable<IParameterQueryPart> Parameters
        {
            get
            {
                return Parts.Where(p => p is IParameterQueryPart).Cast<IParameterQueryPart>();
            }
        }

        public string ProcedureName { get; private set; }

        #endregion
    }
}
