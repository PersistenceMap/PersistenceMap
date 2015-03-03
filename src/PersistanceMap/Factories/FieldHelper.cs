using PersistanceMap.Tracing;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace PersistanceMap.Factories
{
    public static class FieldHelper
    {
        /// <summary>
        /// Extracts the name of the property inside the lambdaexpression
        /// - UnaryExpression: Expression{Func{Warrior, object}} unaryObject = w => w.ID; --> ID
        /// - MemberExpression: Expression{Func{Warrior, int}} memberInt = w => w.ID; --> ID
        /// - BinaryExpression: Expression{Func{Warrior, bool}} binaryInt = w => w.ID == 1; --> ID (Takes the left side and casts to MemberExpression)
        /// - BinaryExpression: Expression{Func{Warrior, bool}} binaryInt = w => 1 == w.ID; --> ID (Takes the right side and casts to MemberExpression)
        /// - Compiled Expression: Expression{Func{int}} binaryInt = () => 5; --> 5
        /// - ToString: Expression{Func{Warrior, bool}} binaryInt = w => 1 == 1; --> w => True
        /// </summary>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        public static string TryExtractPropertyName(LambdaExpression propertyExpression)
        {
            propertyExpression.EnsureArgumentNotNull("propertyExpression");

            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
            {
                // try get the member from the operand of the unaryexpression
                var unary = propertyExpression.Body as UnaryExpression;
                if (unary != null)
                    memberExpression = unary.Operand as MemberExpression;

                if (memberExpression == null)
                {
                    var binary = propertyExpression.Body as BinaryExpression;
                    if (binary != null)
                    {
                        memberExpression = binary.Left as MemberExpression;
                        if (memberExpression == null)
                            memberExpression = binary.Right as MemberExpression;
                    }
                }

                if (memberExpression == null)
                {
                    Logger.TraceLine("## PersistanceMap - Property is not a MemberAccessExpression: {0}", propertyExpression.ToString());

                    try
                    {
                        return propertyExpression.Compile().DynamicInvoke().ToString();
                    }
                    catch (Exception e)
                    {
                        Logger.TraceLine(e.Message);
                        return propertyExpression.ToString();
                    }
                }
            }

            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                Logger.TraceLine(string.Format("## PersistanceMap - Property {0} is not a PropertyInfo", memberExpression.Member));
                return memberExpression.Member.ToString();
            }

            if (propertyInfo.GetGetMethod(true).IsStatic)
            {
                Logger.TraceLine(string.Format("## PersistanceMap - Property {0} is static", memberExpression.Member.Name));
                return memberExpression.Member.Name;
            }

            return memberExpression.Member.Name;
        }

        /// <summary>
        /// Extracts the type of the property inside the lambdaexpression or of the return value
        /// - UnaryExpression: Expression{Func{Warrior, object}} unaryObject = w => w.ID; --> int
        /// - MemberExpression: Expression{Func{Warrior, int}} memberInt = w => w.ID; --> int
        /// - BinaryExpression: Expression{Func{Warrior, bool}} binaryInt = w => w.ID == 1; --> int (Takes the left side and casts to MemberExpression)
        /// - BinaryExpression: Expression{Func{Warrior, bool}} binaryInt = w => 1 == w.ID; --> int (Takes the right side and casts to MemberExpression)
        /// - Compiled Expression: Expression{Func{int}} binaryInt = () => 5; --> int
        /// - Compiled Expression: Expression{Func{Warrior, bool}} binaryInt = w => 1 == 1; --> bool
        /// </summary>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        public static Type TryExtractPropertyType(LambdaExpression propertyExpression)
        {
            propertyExpression.EnsureArgumentNotNull("propertyExpression");

            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
            {
                // try get the member from the operand of the unaryexpression
                var unary = propertyExpression.Body as UnaryExpression;
                if (unary != null)
                    memberExpression = unary.Operand as MemberExpression;

                if (memberExpression == null)
                {
                    var binary = propertyExpression.Body as BinaryExpression;
                    if (binary != null)
                    {
                        memberExpression = binary.Left as MemberExpression;
                        if (memberExpression == null)
                        {
                            memberExpression = binary.Right as MemberExpression;
                        }
                    }
                }

                if (memberExpression == null)
                {
                    try
                    {
                        return propertyExpression.Compile().DynamicInvoke().GetType();
                    }
                    catch (Exception e)
                    {
                        Logger.TraceLine(e.Message);
                        return propertyExpression.Body.Type;
                    }
                }
            }

            return memberExpression.Type;

        }
    }
}
