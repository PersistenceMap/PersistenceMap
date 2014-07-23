using PersistanceMap.QueryBuilder;
using System.Reflection;
using PersistanceMap.QueryBuilder.Decorators;

namespace PersistanceMap
{
    internal static class MemberInfoExtensions
    {
        public static FieldQueryPart ToFieldQueryPart(this MemberInfo member, string alias, string entity)
        {
            return new FieldQueryPart(member.Name, alias, entity);
        }

        //public static FieldQueryPart ToFieldQueryPart(this MemberInfo member, string entity)
        //{
        //    return new FieldQueryPart(member.Name, entity);
        //}
    }
}
