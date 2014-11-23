using System;
using System.Linq.Expressions;

namespace PersistanceMap.QueryParts
{
    public interface IFieldQueryPart : IEntityQueryPart, IQueryPart
    {
        string Field { get; }

        string FieldAlias { get; }

        ///// <summary>
        ///// A expression that converts the db value to the object value
        ///// </summary>
        //Expression<Func<object, object>> Converter { get; }
    }
}
