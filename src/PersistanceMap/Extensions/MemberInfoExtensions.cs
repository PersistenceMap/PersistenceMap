using PersistanceMap.QueryParts;
using System.Reflection;

namespace PersistanceMap
{
    internal static class MemberInfoExtensions
    {
        public static FieldQueryPart ToFieldQueryPart(this MemberInfo member, string alias, string entity)
        {
            return new FieldQueryPart(member.Name, alias, entity)
            {
                OperationType = OperationType.Field
            };
        }
    }
}
