using PersistanceMap.QueryBuilder;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PersistanceMap.Compiler
{
    public class SqlExpressionCompiler : IExpressionCompiler
    {
        public virtual CompiledQuery Compile<T>(SelectQueryPartsMap queryParts)
        {
            var from = queryParts.From;
            if (from == null)
            {
                queryParts.Add(typeof(T).ToFromQueryPart<T>());
                from = queryParts.From;
            }

            // get all members on the type to be composed
            var members = typeof(T).GetSelectionMembers();

            // find all includes that are defined on the join expressions
            var includes = queryParts.Joins.Where(j => j.Operations.Any(o => o.MapOperationType == MapOperationType.Include)).Select(j => new
            {
                j.Entity,
                j.Identifier,
                Operations = j.Operations.Where(o => o.MapOperationType == MapOperationType.Include).ToList()
            }).ToList();

            // find all includes that are defined on the from expression
            if (queryParts.From.Operations.Any(o => o.MapOperationType == MapOperationType.Include))
            {
                includes.Add(new
                {
                    queryParts.From.Entity,
                    queryParts.From.Identifier,
                    Operations = queryParts.From.Operations.Where(o => o.MapOperationType == MapOperationType.Include).ToList()
                });
            }

            foreach (var inc in includes)
            {
                inc.Operations.ForEach(o => queryParts.Add(new FieldQueryPart(ExtractPropertyName(o.Expression), string.IsNullOrEmpty(inc.Identifier) ? inc.Entity : inc.Identifier, inc.Entity), true));
            }

            // don't set identifier to prevent fields being set with a default identifier of the from expression
            //TODO: should entity also not be set?
            foreach (var field in members.Select(m => m.ToFieldQueryPart(/*from.Identifier*/null, from.Entity)))
                queryParts.Add(field, false);

            var builder = new SelectQueryCompiler(queryParts);
            return builder.Compile();
        }

        public CompiledQuery Compile(ProcedureQueryPartsMap queryParts)
        {


            foreach (var param in queryParts.Parameters.Where(p => p.CanHandleCallback))
            {
                //TODO: update queryparts so the ProcedureQueryCompiler knows how to compile the corect sqlstring
                //TODO: output parameter
                throw new NotImplementedException();
            }




            var builder = new ProcedureQueryCompiler(queryParts);
            return builder.Compile();
        }


        private static string ExtractPropertyName(LambdaExpression propertyExpression)
        {
            propertyExpression.EnsureArgumentNotNull("propertyExpression");

            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("Property is not a MemberAccessExpression", "propertyExpression");

            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
                throw new ArgumentException("Property is not a PropertyInfo", "propertyExpression");

            if (propertyInfo.GetGetMethod(true).IsStatic)
                throw new ArgumentException("Property is static", "propertyExpression");

            return memberExpression.Member.Name;
        }

    }
}
