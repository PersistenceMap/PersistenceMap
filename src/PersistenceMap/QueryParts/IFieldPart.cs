using System;
using System.Linq.Expressions;

namespace PersistenceMap.QueryParts
{
    /// <summary>
    /// Marks a class to contain properties describing fields
    /// </summary>
    public interface IFieldPart : IQueryPart
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

        /// <summary>
        /// A expression that converts the db value to the object value
        /// </summary>
        Expression<Func<object, object>> Converter { get; }
    }
}
