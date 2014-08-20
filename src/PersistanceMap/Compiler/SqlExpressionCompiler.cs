using PersistanceMap.QueryBuilder;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PersistanceMap.QueryBuilder.Decorators;

namespace PersistanceMap.Compiler
{
    public class SqlExpressionCompiler : IExpressionCompiler
    {
        public virtual CompiledQuery Compile<T>(SelectQueryPartsMap queryParts)
        {
            // get all members on the type to be composed
            var members = typeof(T).GetSelectionMembers();

            // don't set entity alias to prevent fields being set with a default alias of the from expression
            //TODO: should entity also not be set?
            var fields = members.Select(m => m.ToFieldQueryPart(null, null/*from.Entity*/));
            //foreach (var part in queryParts.Parts.Where(p => p.OperationType == OperationType.SelectMap))
            //{
            //    var map = part as IQueryPartDecorator;
            //    if (map == null)
            //        continue;

            //    foreach (var field in fields)
            //    {
            //        if (map.Parts.Any(f => f is IFieldQueryMap && ((IFieldQueryMap)f).Field == field.Field || ((IFieldQueryMap)f).FieldAlias == field.Field))
            //            continue;

            //        map.Add(field);
            //    }
            //}
            QueryPartsFactory.AddFiedlParts(queryParts, fields.ToArray());

            return queryParts.Compile();
        }

        //public virtual CompiledQuery Compile(Type type, SelectQueryPartsMap queryParts)
        //{
        //    // get all members on the type to be composed
        //    var members = type.GetSelectionMembers();

        //    // don't set entity alias to prevent fields being set with a default alias of the from expression
        //    //TODO: should entity also not be set?
        //    var fields = members.Select(m => m.ToFieldQueryPart(null, null/*from.Entity*/));
        //    foreach (var part in queryParts.Parts.Where(p => p.OperationType == OperationType.SelectMap))
        //    {
        //        var map = part as IQueryPartDecorator;
        //        if (map == null)
        //            continue;

        //        foreach (var field in fields)
        //        {
        //            if (map.Parts.Any(f => f is IFieldQueryMap && ((IFieldQueryMap)f).Field == field.Field || ((IFieldQueryMap)f).FieldAlias == field.Field))
        //                continue;

        //            map.Add(field);
        //        }
        //    }

        //    return queryParts.Compile();
        //}

        public virtual CompiledQuery Compile(ProcedureQueryPartsMap queryParts)
        {
            //var builder = new ProcedureQueryCompiler(queryParts);
            //return builder.Compile();
            return queryParts.Compile();
        }
    }
}
