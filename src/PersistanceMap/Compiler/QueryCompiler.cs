using PersistanceMap.QueryBuilder;
using System.Linq;
using System.Text;
using PersistanceMap.Sql;

namespace PersistanceMap.Compiler
{
    public abstract class QueryCompiler
    {
        public abstract CompiledQuery Compile();
    }

    /// <summary>
    /// Compiles all elements of a QueryPartsContainer to a CompiledQuery
    /// </summary>
    public class SelectQueryCompiler : QueryCompiler
    {
        private readonly SelectQueryPartsMap _queryParts;

        public SelectQueryCompiler(SelectQueryPartsMap queryParts)
        {
            _queryParts = queryParts;
        }

        public override CompiledQuery Compile()
        {
            if (_queryParts == null)
                return null;

            var sb = new StringBuilder(100);
            sb.Append("select ");

            // add resultset fields
            foreach (var field in _queryParts.Fields)
                sb.AppendFormat("{0}{1} ", field.Compile(), _queryParts.Fields.Last() == field ? "" : ",");

            // add from
            sb.AppendFormat("{0} \r\n", _queryParts.From.Compile());

            // add joins
            foreach (var join in _queryParts.Joins)
                sb.Append(join.Compile());

            // where

            // order...

            return new CompiledQuery
            {
                QueryString = sb.ToString(),
                QueryParts = _queryParts
            };
        }
    }

    /// <summary>
    /// Compiles all elements of a QueryPartsContainer to a CompiledQuery
    /// </summary>
    public class ProcedureQueryCompiler : QueryCompiler
    {
        private readonly ProcedureQueryPartsMap _queryParts;

        public ProcedureQueryCompiler(ProcedureQueryPartsMap queryParts)
        {
            _queryParts = queryParts;
        }

        public override CompiledQuery Compile()
        {
            if (_queryParts == null)
                return null;

            var sb = new StringBuilder(100);
            sb.Append(string.Format("exec {0} ", _queryParts.ProcedureName));

            var conv = new LambdaExpressionToSqlCompiler();
            conv.PrefixFieldWithTableName = false;

            foreach (var param in _queryParts.Parameters)
            {
                //TODO: operations can also be MapOperationType.Identifier!
                var valuePredicate = param.Operations.FirstOrDefault(o => o.MapOperationType == MapOperationType.Value);
                if (valuePredicate != null)
                {
                    var obj = conv.Compile(valuePredicate.Expression);
                    var value = DialectProvider.Instance.GetQuotedValue(obj, obj.GetType());

                    sb.Append(string.Format("{0}{1}", value, _queryParts.Parameters.Last() == param ? "" : ", "));
                }
            }

            return new CompiledQuery
            {
                QueryString = sb.ToString(),
                QueryParts = _queryParts
            };
        }
    }
}
