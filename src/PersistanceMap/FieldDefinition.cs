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
        /// Gets or sets the type that the field has
        /// </summary>
        public Type FieldType { get; set; }

        /// <summary>
        /// The type of the class containing this member
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// The propertyinfo fot this member
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        /// Gets a value indicating if the property is nullable
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// Gets a vlaue indicating if the property is a primary key property
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// Returnes the delegate that is used to get the value from the property
        /// </summary>
        public PropertyGetterDelegate GetValueFunction { get; set; }

        /// <summary>
        /// Returnes a delegate that is used to set the value to the property
        /// </summary>
        public PropertySetterDelegate SetValueFunction { get; set; }

        /// <summary>
        /// The converter that converts the databasevalue to the property value
        /// </summary>
        public Func<object, object> Converter { get; set; }

        public override string ToString()
        {
            return string.Format("Field: {0} [{1}] Member: {2} [{3}]", FieldName, FieldType, MemberName, MemberType);
        }
    }
}
