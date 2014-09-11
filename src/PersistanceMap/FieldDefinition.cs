using System;
using System.Reflection;

namespace PersistanceMap
{
    public class FieldDefinition
    {
        /// <summary>
        /// Name of the member in the business object
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// Name of the field in the Database or the resultset
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Name of the Entity containing the field
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// the type of the member
        /// </summary>
        public Type MemberType { get; set; }

        /// <summary>
        /// The type of the class containing this member
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// The propertyinfo fot this member
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }

        public bool IsNullable { get; set; }

        public bool IsPrimaryKey { get; set; }

        public PropertyGetterDelegate GetValueFunction { get; set; }

        public PropertySetterDelegate SetValueFunction { get; set; }
    }
}
