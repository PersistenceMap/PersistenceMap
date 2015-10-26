using PersistanceMap.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PersistanceMap.Factories
{
    internal static class ExpressionFactory
    {
        /// <summary>
        /// Creates a lambdaexpression that returnes the id propterty with the value contained by the property. The ID is provided by the typedefinitionfactory and is based on common conventions.
        /// x => x.Property == value
        /// </summary>
        /// <typeparam name="T">The object type that the satement is created with</typeparam>
        /// <param name="property">The property returning the value in the object</param>
        /// <returns>A expression providing a equality statement</returns>
        public static Expression<Func<T, bool>> CreateEqualityExpression<T>(Expression<Func<T>> property)
        {
            // get the field that represents the primary key
            var fields = TypeDefinitionFactory.GetFieldDefinitions<T>();
            var pk = fields.FirstOrDefault(f => f.IsPrimaryKey);
            if (pk == null)
                return null;

            var obj = property.Compile().Invoke();
            var value = pk.PropertyInfo.GetValue(obj);


            ParameterExpression pe = Expression.Parameter(typeof(T), "exp");

            // x => (x.Property == value)
            // Create an expression tree that represents the expression 'x.Property == value'.
            var left = Expression.Property(pe, pk.PropertyInfo);
            var right = Expression.Constant(value);
            var expression = Expression.Equal(left, right);

            return Expression.Lambda<Func<T, bool>>(expression, new ParameterExpression[] { Expression.Parameter(typeof(T), null) });
        }

        /// <summary>
        /// Creates a equality statement/expression by finding the key property using a custom convention
        /// Expression: x => x.Property == value
        /// </summary>
        /// <typeparam name="T">The object type that the satement is created with</typeparam>
        /// <param name="valueExpression">The expression providing the value that will be used in the equality statement</param>
        /// <returns>A expression providing a equality statement</returns>
        public static Expression<Func<T, bool>> CreateEqualityExpression<T>(Expression<Func<object>> valueExpression)
        {
            var valueObject = valueExpression.Compile().Invoke();
            var fields = TypeDefinitionFactory.GetFieldDefinitions<T>(valueObject.GetType());
            var pk = fields.FirstOrDefault(f => f.IsPrimaryKey);
            if (pk == null)
                return null;

            var value = pk.GetValueFunction(valueExpression.Compile().Invoke());

            ParameterExpression pe = Expression.Parameter(typeof(T), "exp");

            // x => (x.Property == value)
            // Create an expression tree that represents the expression 'x.Property == value'.
            var left = Expression.Property(pe, pk.PropertyInfo);
            var right = Expression.Constant(value, pk.MemberType);

            var expression = Expression.Equal(left, right);

            return Expression.Lambda<Func<T, bool>>(expression, new ParameterExpression[] { Expression.Parameter(typeof(T), null) });
        }

        /// <summary>
        /// Creates a lambdaexpression that returnes the id propterty with the constant value returned by the expression
        /// Expression: x => x.Property == value
        /// </summary>
        /// <typeparam name="T">The type of the value provided by the value expression</typeparam>
        /// <param name="valueExpression">The expression providing the value that will be used in the equality statement</param>
        /// <param name="key">The function providing the property that will be used in the equality statement</param>
        /// <returns>A expression providing a equality statement</returns>
        public static Expression<Func<T, bool>> CreateEqualityExpression<T>(Expression<Func<T>> valueExpression, Expression<Func<T, object>> key)
        {
            if (key == null)
                return null;

            var value = key.Compile().Invoke(valueExpression.Compile().Invoke());

            ParameterExpression pe = Expression.Parameter(typeof(T), "exp");

            // x => (x.Property == value)
            // Create an expression tree that represents the expression 'x.Property == value'.
            var left = Expression.Property(pe, key.TryExtractPropertyName());
            var right = Expression.Constant(value);
            var e1 = Expression.Equal(left, right);

            return Expression.Lambda<Func<T, bool>>(e1, new ParameterExpression[] { Expression.Parameter(typeof(T), null) });
        }

        public static IEnumerable<Expression<Func<T, bool>>> CreateEqualityExpressions<T>(object entity, IEnumerable<FieldDefinition> valueFields, IEnumerable<FieldDefinition> tableFields)
        {
            ParameterExpression pe = Expression.Parameter(typeof(T), "exp");

            foreach (var valueField in valueFields)
            {
                var entityField = tableFields.FirstOrDefault(f => f.FieldName == valueField.FieldName);
                var value = valueField.GetValueFunction(entity);

                // x => (x.Property == value)
                // Create an expression tree that represents the expression 'x.Property == value'.
                var left = Expression.Property(pe, entityField.PropertyInfo);
                var right = Expression.Constant(value);

                yield return Expression.Lambda<Func<T, bool>>(Expression.Equal(left, right), new ParameterExpression[] { Expression.Parameter(typeof (T), null) });
            }
        }
    }
}
