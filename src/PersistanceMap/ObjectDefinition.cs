using System;

namespace PersistanceMap
{
    public class ObjectDefinition
    {
        public string Name { get; set; }

        public Type ObjectType { get; set; }

        //public bool IsNullable { get; set; }

        /// <summary>
        /// The converter that converts the databasevalue to the property value
        /// </summary>
        public Func<object, object> Converter { get; set; }
    }
}
