using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap
{
    internal static class MapOptionCompiler
    {
        public static IEnumerable<IExpressionMapQueryPart> Compile<T>(params Expression<Func<MapOption<T>, IExpressionMapQueryPart>>[] predicates)
        {
            var parts = new List<IExpressionMapQueryPart>();
            var options = new MapOption<T>();

            foreach (var predicate in predicates)
                parts.Add(predicate.Compile().Invoke(options));

            return parts;
        }

        public static IEnumerable<IExpressionMapQueryPart> Compile<T, T2>(params Expression<Func<MapOption<T, T2>, IExpressionMapQueryPart>>[] predicates)
        {
            var parts = new List<IExpressionMapQueryPart>();
            var options = new MapOption<T, T2>();

            foreach (var predicate in predicates)
                parts.Add(predicate.Compile().Invoke(options));

            return parts;
        }
    }

    /// <summary>
    /// MapOption for simple expressions like From{T}
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MapOption<T>
    {
        public IExpressionMapQueryPart Include<T2>(Expression<Func<T, T2>> predicate)
        {
            throw new NotImplementedException();
        }

        public IExpressionMapQueryPart Identifier(Expression<Func<string>> predicate)
        {
            return new ExpressionMapQueryPart(MapOperationType.Identifier, predicate);
        }
    }

    /// <summary>
    /// MapOption for extended expressions like Join{T,T2}
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class MapOption<T, T2> : MapOption<T>
    {
        public IExpressionMapQueryPart On(Expression<Func<T, T2, bool>> predicate)
        {
            return new ExpressionMapQueryPart(MapOperationType.Join, predicate);
        }

        public IExpressionMapQueryPart On(string identifier, Expression<Func<T, T2, bool>> predicate)
        {
            //var part = new ExpressionMapQueryPart(MapOperationType.Join, predicate);
            //part.Identifier = identifier;
            //return part;

            throw new NotImplementedException("identifier has to be implemented on ExpressionMapQueryPart");
        }

        public IExpressionMapQueryPart On<T3>(Expression<Func<T, T3, bool>> predicate)
        {
            return new ExpressionMapQueryPart(MapOperationType.Join, predicate);
        }

        public IExpressionMapQueryPart On<T3>(string identifier, Expression<Func<T, T3, bool>> predicate)
        {
            //var part = new ExpressionMapQueryPart(MapOperationType.Join, predicate);
            //part.Identifier = identifier;
            //return part;

            throw new NotImplementedException("identifier has to be implemented on ExpressionMapQueryPart");
        }






        public IExpressionMapQueryPart And(Expression<Func<T, T2, bool>> predicate)
        {
            return new ExpressionMapQueryPart(MapOperationType.And, predicate);
        }

        public IExpressionMapQueryPart And(string identifier, Expression<Func<T, T2, bool>> predicate)
        {
            //var part = new ExpressionMapQueryPart(MapOperationType.And, predicate);
            //part.Identifier = identifier;
            //return part;

            throw new NotImplementedException("identifier has to be implemented on ExpressionMapQueryPart");
        }

        public IExpressionMapQueryPart And<T3>(Expression<Func<T, T3, bool>> predicate)
        {
            return new ExpressionMapQueryPart(MapOperationType.And, predicate);
        }

        public IExpressionMapQueryPart And<T3>(string identifier, Expression<Func<T, T3, bool>> predicate)
        {
            //var part = new ExpressionMapQueryPart(MapOperationType.And, predicate);
            //part.Identifier = identifier;
            //return part;

            throw new NotImplementedException("identifier has to be implemented on ExpressionMapQueryPart");
        }





        public IExpressionMapQueryPart Or(Expression<Func<T, T2, bool>> predicate)
        {
            return new ExpressionMapQueryPart(MapOperationType.Or, predicate);
        }

        public IExpressionMapQueryPart Or<T3>(Expression<Func<T, T3, bool>> predicate)
        {
            return new ExpressionMapQueryPart(MapOperationType.Or, predicate);
        }
    }

    //public static class MapOption
    //{
    //    public static IExpressionMapQueryPart On<T, T2>(Expression<Func<T, T2, bool>> predicate)
    //    {
    //        return new ExpressionMapQueryPart(MapOperationType.Join, predicate);
    //    }

    //    public static IExpressionMapQueryPart On<T, T2>(string identifier, Expression<Func<T, T2, bool>> predicate)
    //    {
    //        //var part = new ExpressionMapQueryPart(MapOperationType.Join, predicate);
    //        //part.Identifier = identifier;
    //        //return part;

    //        throw new NotImplementedException("identifier has to be implemented on ExpressionMapQueryPart");
    //    }

    //    //public static IJoinMapOption And<T, T2>(Expression<Func<T, T2, bool>> predicate)
    //    //{
    //    //    throw new NotImplementedException();
    //    //}

    //    public static IExpressionMapQueryPart Or<T, T2>(Expression<Func<T, T2, bool>> predicate)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public static IExpressionMapQueryPart Include<T, T2>(Expression<Func<T, T2>> predicate)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    //public static IExpressionMapQueryPart<T> Include<T, T2>(Expression<Func<T, T2>> predicate)
    //    //{
    //    //    throw new NotImplementedException();
    //    //}

    //    public static IExpressionMapQueryPart Include<T>(Expression<Func<T>> predicate)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public static IExpressionMapQueryPart Identifier(Expression<Func<string>> predicate)
    //    {
    //        return new ExpressionMapQueryPart(MapOperationType.Identifier, predicate);
    //    }
    //}
}
