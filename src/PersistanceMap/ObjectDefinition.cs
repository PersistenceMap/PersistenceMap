using System;

namespace PersistanceMap
{
    public class ObjectDefinition
    {
        /// <summary>
        /// The name of the field
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type of the field
        /// </summary>
        public Type ObjectType { get; set; }
        
        /// <summary>
        /// The converter that converts the databasevalue to the property value
        /// </summary>
        public Func<object, object> Converter { get; set; }
    }
}
