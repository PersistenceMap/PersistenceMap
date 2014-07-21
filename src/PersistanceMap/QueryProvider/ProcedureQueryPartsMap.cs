using System.Linq;
using System.Text;
using PersistanceMap.Compiler;
using PersistanceMap.QueryBuilder;
using System.Collections.Generic;

namespace PersistanceMap
{
    public class ProcedureQueryPartsMap : IQueryPartsMap
    {
        public ProcedureQueryPartsMap(string procedure)
        {
            procedure.EnsureArgumentNotNullOrEmpty("procedure");

            ProcedureName = procedure;
        }

        #region IQueryPartsMap Implementation

        //public IEnumerable<IQueryMap> Mappings
        //{
        //    get
        //    {
        //        return InternalMap;
        //    }
        //}

        //public void Add(IQueryMap map)
        //{
        //    InternalMap.Add(map);
        //}

        public CompiledQuery Compile()
        {
            //if (_queryParts == null)
            //    return null;

            /* *Using Output compiles to*                
            declare @p1 datetime
            set @p1='2012-01-01 00:00:00'
            exec procedure @param1=@p1 output,@param2='2014-07-15 00:00:00'
            select @p1                
            */

            var sb = new StringBuilder(100);

            // prepare outputparameters
            int i = 1;
            foreach (var param in Parameters.Where(p => p.CanHandleCallback))
            {
                // creates a name for the output parameter
                var definition = param.CompileOutParameter(i);
                if (!string.IsNullOrEmpty(definition))
                {
                    sb.AppendLine(definition);

                    i++;
                }
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
            var lastCallback = Parameters.LastOrDefault(p => p.CanHandleCallback);
            var selectoutput = string.Empty;
            foreach (var param in Parameters.Where(p => p.CanHandleCallback))
            {
                if (string.IsNullOrEmpty(selectoutput))
                    selectoutput = "select";

                if (!string.IsNullOrEmpty(param.CallbackParameterName))
                {
                    selectoutput = string.Format("{0} @{1} as {1}{2}", selectoutput, param.CallbackParameterName, lastCallback == param ? "" : ", ");
                }
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

        //private IList<IQueryMap> _internalMap;
        //private IList<IQueryMap> InternalMap
        //{
        //    get
        //    {
        //        if (_internalMap == null)
        //            _internalMap = new List<IQueryMap>();
        //        return _internalMap;
        //    }
        //}

        IList<IParameterQueryPart> _parameters;
        internal IList<IParameterQueryPart> Parameters
        {
            get
            {
                if (_parameters == null)
                    _parameters = new List<IParameterQueryPart>();
                return _parameters;
            }
        }

        public string ProcedureName { get; private set; }

        #endregion

        #region Add Methods

        internal void Add(IParameterQueryPart part)
        {
            Parameters.Add(part);
            //InternalMap.Add(part);
        }

        #endregion
    }
}
