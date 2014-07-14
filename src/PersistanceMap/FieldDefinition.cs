using System;
using System.Reflection;

namespace PersistanceMap
{
    public class FieldDefinition
    {
        public string Name { get; set; }

        public string FieldName { get; set; }

        public string EntityName { get; set; }

        public Type FieldType { get; set; }

        public Type EntityType { get; set; }

        public PropertyInfo PropertyInfo { get; set; }

        public bool IsNullable { get; set; }

        public PropertyGetterDelegate GetValueFunction { get; set; }

        public PropertySetterDelegate SetValueFunction { get; set; }
    }
}
