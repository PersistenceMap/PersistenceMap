using System;
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
}
