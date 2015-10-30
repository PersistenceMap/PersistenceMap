using System;

namespace PersistenceMap.Sql
{
    public class EnumMemberAccess : PartialSqlString, ISqlString
    {
        public EnumMemberAccess(string text, Type enumType)
            : base(text)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException("Type not valid", "enumType");

            EnumType = enumType;
        }

        public Type EnumType { get; private set; }
    }
}
