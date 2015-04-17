using System;

namespace PersistanceMap.QueryParts
{
    /// <summary>
    /// Marks a class to contain properties describing fields
    /// </summary>
    public interface IFieldPart
    {
        /// <summary>
        /// Gets the name of the field
        /// </summary>
        string Field { get; }

        /// <summary>
        /// Gets the alias name of the field
        /// </summary>
        string FieldAlias { get; }

        /// <summary>
        /// Gets the name of the entity
        /// </summary>
        string Entity { get; }

        /// <summary>
        /// Gets the alias name of the entity
        /// </summary>
        string EntityAlias { get; set; }
    }
}
