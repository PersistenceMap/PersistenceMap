using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace PersistanceMap.Internals
{
    internal static class FieldHelper
    {
        internal static string TryExtractPropertyName(LambdaExpression propertyExpression)
        {
            propertyExpression.EnsureArgumentNotNull("propertyExpression");

            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
            {
                //throw new ArgumentException("Property is not a MemberAccessExpression", "propertyExpression");
                Trace.WriteLine("Property is not a MemberAccessExpression");
                return propertyExpression.Body.ToString();
            }

            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                Trace.WriteLine("Property is not a PropertyInfo");
                return memberExpression.Member.ToString();
            }

            if (propertyInfo.GetGetMethod(true).IsStatic)
            {
                //throw new ArgumentException("Property is static", "propertyExpression");
                Trace.WriteLine("Property is static");
                return memberExpression.Member.ToString();
            }

            return memberExpression.Member.Name;
        }
    }
}
