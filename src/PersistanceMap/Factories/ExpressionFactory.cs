using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PersistanceMap.Factories
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
        public static Expression<Func<T, bool>> CreateKeyExpression<T>(Expression<Func<T>> entity)
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
            var expression = Expression.Equal(left, right);

            //return Expression.Lambda(e1);
            return Expression.Lambda<Func<T, bool>>(expression, new ParameterExpression[] { Expression.Parameter(typeof(T), null) });
        }

        public static Expression<Func<T, bool>> CreateKeyExpression<T>(Expression<Func<object>> entity)
        {
            var valueObject = entity.Compile().Invoke();
            var fields = TypeDefinitionFactory.GetFieldDefinitions<T>(valueObject.GetType());
            var pk = fields.FirstOrDefault(f => f.IsPrimaryKey);
            if (pk == null)
                return null;

            var value = pk.GetValueFunction(entity.Compile().Invoke());

            ParameterExpression pe = Expression.Parameter(typeof(T), "exp");

            // x => (x.Property == value)
            // Create an expression tree that represents the expression 'x.Property == value'.
            var left = Expression.Property(pe, pk.PropertyInfo);
            var right = Expression.Constant(value, pk.MemberType);

            var expression = Expression.Equal(left, right);

            return Expression.Lambda<Func<T, bool>>(expression, new ParameterExpression[] { Expression.Parameter(typeof(T), null) });
        }

        /// <summary>
        /// Creates a lambdaexpression that returnes the id propterty with the value contained by the property
        /// x => x.Property == value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> CreateKeyExpression<T>(Expression<Func<T>> entity, Expression<Func<T, object>> key)
        {
            if (key == null)
                return null;

            var value = key.Compile().Invoke(entity.Compile().Invoke());

            ParameterExpression pe = Expression.Parameter(typeof(T), "exp");

            // x => (x.Property == value)
            // Create an expression tree that represents the expression 'x.Property == value'.
            var left = Expression.Property(pe, FieldHelper.TryExtractPropertyName(key));
            var right = Expression.Constant(value);
            var e1 = Expression.Equal(left, right);

            //return Expression.Lambda(e1);
            return Expression.Lambda<Func<T, bool>>(e1, new ParameterExpression[] { Expression.Parameter(typeof(T), null) });
        }

        //public static Expression<Func<T, bool>> CreateKeyExpression<T>(Expression<Func<object>> entity, Expression<Func<T, bool>> key)
        //{
        //    if (key == null)
        //        return null;

        //    var value = key.Compile().Invoke(entity.Compile().Invoke());

        //    ParameterExpression pe = Expression.Parameter(typeof(T), "exp");

        //    // x => (x.Property == value)
        //    // Create an expression tree that represents the expression 'x.Property == value'.
        //    var left = Expression.Property(pe, GetProperty(key));
        //    var right = Expression.Constant(value);
        //    var e1 = Expression.Equal(left, right);

        //    //return Expression.Lambda(e1);
        //    return Expression.Lambda<Func<T, bool>>(e1, new ParameterExpression[] { Expression.Parameter(typeof(T), null) });
        //}

        public static IEnumerable<Expression<Func<T,bool>>> CreateKeyValueEqualityExpressions<T>(object entity, IEnumerable<FieldDefinition> valueFields, IEnumerable<FieldDefinition> tableFields)
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

                yield return Expression.Lambda<Func<T, bool>>(Expression.Equal(left, right), new ParameterExpression[] { Expression.Parameter(typeof(T), null) });
            }
        }

        public static IEnumerable<LambdaExpression> CreateKeyExpressions<T>(object entity, IEnumerable<FieldDefinition> valueFields)
        {
            ParameterExpression pe = Expression.Parameter(typeof(T), "exp");

            foreach (var valueField in valueFields)
            {
                var value = valueField.GetValueFunction(entity);

                // x => (x.Property == value)
                // Create an expression tree that represents the expression 'x.Property == value'.
                var left = Expression.Property(pe, valueField.PropertyInfo);
                var right = Expression.Constant(value, valueField.MemberType);

                var expression = Expression.Assign(left, right);

                yield return Expression.Lambda(expression, new ParameterExpression[] { Expression.Parameter(typeof(T), null) });
            }
        }

        ///// <summary>
        ///// Extracts the propertyinfo out of a expression
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="expression"></param>
        ///// <returns></returns>
        //public static PropertyInfo GetProperty<T>(Expression<Func<T, object>> expression)
        //{
        //    // sometimes the expression comes in as Convert(originalexpression)
        //    if (expression.Body is UnaryExpression)
        //    {
        //        var exp = (UnaryExpression)expression.Body;
        //        if (exp.Operand is MemberExpression)
        //        {
        //            return (PropertyInfo)((MemberExpression)exp.Operand).Member;
        //        }
                
        //        throw new ArgumentException(string.Format("Property cannot be extracted from Expression {0}", expression.ToString()));
        //    }
            
        //    if (expression.Body is MemberExpression)
        //    {
        //        return (PropertyInfo)((MemberExpression)expression.Body).Member;
        //    }

        //    throw new ArgumentException(string.Format("Property cannot be extracted from Expression {0}", expression.ToString()));
        //}
    }
}
