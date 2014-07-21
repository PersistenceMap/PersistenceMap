using System;
using System.Linq.Expressions;
using System.Reflection;

namespace PersistanceMap.Internals
{
    internal static class FieldHelper
    {
        internal static string ExtractPropertyName(LambdaExpression propertyExpression)
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
