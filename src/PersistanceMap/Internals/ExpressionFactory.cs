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

        public static IEnumerable<Expression<Func<T,bool>>> CreateKeyExpressions<T>(object entity, IEnumerable<FieldDefinition> valueFields, IEnumerable<FieldDefinition> entityFields)
        {
            ParameterExpression pe = Expression.Parameter(typeof(T), "exp");

            foreach (var valueField in valueFields)
            {
                var entityField = entityFields.FirstOrDefault(f => f.FieldName == valueField.FieldName);
                var value = valueField.GetValueFunction(entity);

                // x => (x.Property == value)
                // Create an expression tree that represents the expression 'x.Property == value'.
                var left = Expression.Property(pe, entityField.PropertyInfo);
                var right = Expression.Constant(value);

                yield return Expression.Lambda<Func<T, bool>>(Expression.Equal(left, right), new ParameterExpression[] { Expression.Parameter(typeof(T), null) });

                //if (expression != null)
                //{
                //    var exTmp = Expression.Equal(left, right);
                //    expression = Expression.AndAlso(expression, exTmp);
                //}
                //else
                //{
                //    expression = Expression.Equal(left, right);
                //}
            }

            //return Expression.Lambda(expression);
        }

        /// <summary>
        /// Extracts the propertyinfo out of a expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static PropertyInfo GetProperty<T>(Expression<Func<T, object>> expression)
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
}
