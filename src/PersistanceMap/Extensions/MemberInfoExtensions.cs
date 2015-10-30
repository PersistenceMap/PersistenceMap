using System.Reflection;
using PersistanceMap.QueryParts;

namespace PersistanceMap
{
    internal static class MemberInfoExtensions
    {
        public static FieldQueryPart ToFieldQueryPart(this MemberInfo member, string alias, string entity)
        {
            return new FieldQueryPart(member.Name, alias, entity, entityType: member.DeclaringType)
            {
                OperationType = OperationType.Field
            };
        }
    }
}
