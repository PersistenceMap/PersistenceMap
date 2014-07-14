using PersistanceMap.QueryBuilder;
using System.Reflection;

namespace PersistanceMap
{
    internal static class MemberInfoExtensions
    {
        public static FieldQueryPart ToFieldQueryPart(this MemberInfo member, string identifier, string entity)
        {
            return new FieldQueryPart(member.Name, identifier, entity);
        }

        //public static FieldQueryPart ToFieldQueryPart(this MemberInfo member, string entity)
        //{
        //    return new FieldQueryPart(member.Name, entity);
        //}
    }
}
