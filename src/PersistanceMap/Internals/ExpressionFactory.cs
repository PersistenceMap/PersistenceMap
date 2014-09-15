using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PersistanceMap.Internals
{
    internal static class ExpressionFactory
    {
        public static LambdaExpression CreateExpression(string value)
        {
            Expression<Func<string>> expression = () => value;
            return expression;
        }

        /// <summary>
        /// Creates a lambdaexpression that returnes the id propterty with the value contained by the property
        /// x => x.Property == value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static LambdaExpression CreateKeyExpression<T>(Expression<Func<T>> entity)
        {
            var fields = TypeDefinitionFactory.GetFieldDefinitions<T>();
            var pk = fields.FirstOrDefault(f => f.IsPrimaryKey);
            if (pk == null)
                return null;

            var obj = entity.Compile().Invoke();
            var value = pk.PropertyInfo.GetValue(obj);


            ParameterExpression pe = Expression.Parameter(typeof(T), "exp");

            // x => (x.Property == value)
            // Create an expression tree that represents the expression 'x.Property == value'.
            var left = Expression.Property(pe, pk.PropertyInfo);
            var right = Expression.Constant(value);
            var e1 = Expression.Equal(left, right);

            return Expression.Lambda(e1);
        }

        /// <summary>
        /// Creates a lambdaexpression that returnes the id propterty with the value contained by the property
        /// x => x.Property == value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static LambdaExpression CreateKeyExpression<T>(Expression<Func<T>> entity, Expression<Func<T, object>> key)
        {
            if (key == null)
                return null;

            var value = key.Compile().Invoke(entity.Compile().Invoke());

            ParameterExpression pe = Expression.Parameter(typeof(T), "exp");

            // x => (x.Property == value)
            // Create an expression tree that represents the expression 'x.Property == value'.
            var left = Expression.Property(pe, GetProperty(key));
            var right = Expression.Constant(value);
            var e1 = Expression.Equal(left, right);

            return Expression.Lambda(e1);
        }

        public static LambdaExpression CreateKeyExpression<T>(object entity, IEnumerable<FieldDefinition> valueFields, IEnumerable<FieldDefinition> entityFields)
        {
            ParameterExpression pe = Expression.Parameter(typeof(T), "exp");

            BinaryExpression expression = null;
            //LambdaExpression expression = null;
            foreach (var valueField in valueFields)
            {
                var entityField = entityFields.FirstOrDefault(f => f.FieldName == valueField.FieldName);
                var value = valueField.GetValueFunction(entity);

                // x => (x.Property == value)
                // Create an expression tree that represents the expression 'x.Property == value'.
                var left = Expression.Property(pe, entityField.PropertyInfo);
                var right = Expression.Constant(value);
                
                if (expression != null)
                {
                    var exTmp = Expression.Equal(left, right);
                    expression = Expression.AndAlso(expression, exTmp);

                    var test = Expression.Lambda<Func<T, bool>>(exTmp, new ParameterExpression[] { Expression.Parameter(typeof(T), null) });
                    expression = test.And(


                    //expression = Expression.Lambda<Func<T,bool>>(Expression.AndAlso(expression.Body, exTmp));


                    //InvocationExpression lambdaExpr1Invoke = Expression.Invoke(expression, Expression.Parameter(typeof(T), null));
                    //expression = Expression.AndAlso(lambdaExpr1Invoke, exTmp);
                }
                else
                {
                    expression = Expression.Equal(left, right);
                }
            }

            return Expression.Lambda(expression);
        }

        private static PropertyInfo GetProperty<T>(Expression<Func<T, object>> expression)
        {
            // sometimes the expression comes in as Convert(originalexpression)
            if (expression.Body is UnaryExpression)
            {
                var exp = (UnaryExpression)expression.Body;
                if (exp.Operand is MemberExpression)
                {
                    return (PropertyInfo)((MemberExpression)exp.Operand).Member;
                }
                
                throw new ArgumentException(string.Format("Property cannot be extracted from Expression {0}", expression.ToString()));
            }
            
            if (expression.Body is MemberExpression)
            {
                return (PropertyInfo)((MemberExpression)expression.Body).Member;
            }

            throw new ArgumentException(string.Format("Property cannot be extracted from Expression {0}", expression.ToString()));
        }
    }

    // https://www.google.ch/webhp?sourceid=chrome-instant&ion=1&espv=2&ie=UTF-8#q=expressiontree+combine+multiple+expressions
    // http://blogs.msdn.com/b/meek/archive/2008/05/02/linq-to-entities-combining-predicates.aspx
    public static class Utility
    {
        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression 
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.And);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }
    }

    public class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> map;

        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression replacement;
            if (map.TryGetValue(p, out replacement))
            {
                p = replacement;
            }
            return base.VisitParameter(p);
        }
    }
}
