using PersistanceMap.Tracing;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace PersistanceMap
{
    public delegate object PropertyGetterDelegate(object instance);

    public delegate void PropertySetterDelegate(object instance, object value);

    internal static class PropertyExtensions
    {
        public static PropertyGetterDelegate GetPropertyGetter(this PropertyInfo propertyInfo)
        {
            var getMethodInfo = propertyInfo.GetGetMethod();
            if (getMethodInfo == null) 
                return null;

            try
            {
                var objectInstanceParam = Expression.Parameter(typeof(object), "objectInstanceParam");
                var instanceParam = Expression.Convert(objectInstanceParam, propertyInfo.DeclaringType);

                var exprCallPropertyGetFn = Expression.Call(instanceParam, getMethodInfo);
                var oExprCallPropertyGetFn = Expression.Convert(exprCallPropertyGetFn, typeof(object));

                var propertyGetFn = Expression.Lambda<PropertyGetterDelegate>(oExprCallPropertyGetFn, objectInstanceParam).Compile();

                return propertyGetFn;
            }
            catch (Exception ex)
            {
                Logger.TraceLine(ex.Message);
                throw;
            }
        }

        public static PropertySetterDelegate GetPropertySetter(this PropertyInfo propertyInfo)
        {
            var propertySetMethod = propertyInfo.GetSetMethod();
            if (propertySetMethod == null) 
                return null;

            var instance = Expression.Parameter(typeof(object), "i");
            var argument = Expression.Parameter(typeof(object), "a");

            var instanceParam = Expression.Convert(instance, propertyInfo.DeclaringType);
            var valueParam = Expression.Convert(argument, propertyInfo.PropertyType);

            var setterCall = Expression.Call(instanceParam, propertyInfo.GetSetMethod(), valueParam);

            return Expression.Lambda<PropertySetterDelegate>(setterCall, instance, argument).Compile();
        }
    }
}
