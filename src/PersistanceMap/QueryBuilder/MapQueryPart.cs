using PersistanceMap.Compiler;
using PersistanceMap.Sql;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder
{
    internal class MapQueryPart : IMapQueryPart
    {
        public MapQueryPart(MapOperationType operationtype, LambdaExpression expression)
        {
            MapOperationType = operationtype;
            Expression = expression;

            IdentifierMap = new Dictionary<Type, string>();
        }

        public MapOperationType MapOperationType { get; private set; }

        public LambdaExpression Expression { get; private set; }

        public Dictionary<Type, string> IdentifierMap { get; private set; }

        public virtual string Compile()
        {
            var value = LambdaExpressionToSqlCompiler.Instance.Compile(this/*Expression*/);
            if (value != null)
                return DialectProvider.Instance.GetQuotedValue(value, value.GetType());

            return null;
        }

        internal void AddIdentifier(Type type, string identifier)
        {
            if (!string.IsNullOrEmpty(identifier))
                IdentifierMap.Add(type, identifier);
        }
    }

    //internal class IdentifierMapQueryPart : MapQueryPart, IIdentifierMapQueryPart
    //{
    //    public IdentifierMapQueryPart(MapOperationType operationtype, LambdaExpression expression)
    //        : base(operationtype, expression)
    //    {
    //        IdentifierMap = new Dictionary<Type, string>();
    //    }

    //    public Dictionary<Type, string> IdentifierMap { get; private set; }

    //    internal void AddIdentifier(Type type, string identifier)
    //    {
    //        if (!string.IsNullOrEmpty(identifier))
    //            IdentifierMap.Add(type, identifier);
    //    }
    //}

    internal class NamedMapQueryPart : MapQueryPart
    {
        public NamedMapQueryPart(MapOperationType operationtype, string name, LambdaExpression expression)
            : base(operationtype, expression)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public override string Compile()
        {
            if (string.IsNullOrEmpty(Name))
                return base.Compile();

            return string.Format("{0}={1}", Name, base.Compile());
        }
    }

    //public delegate void MapOptionCallback<T>(T value);

    //internal class CallbackMapQueryPart<T> : NamedMapQueryPart
    //{
    //    public CallbackMapQueryPart(MapOperationType operationtype, LambdaExpression expression)
    //        : this(operationtype, null, expression, null)
    //    {
    //    }

    //    public CallbackMapQueryPart(MapOperationType operationtype, string name, LambdaExpression expression)
    //        : this(operationtype, name, expression, null)
    //    {
    //    }

    //    public CallbackMapQueryPart(MapOperationType operationtype, string name, LambdaExpression expression, Action<T> callback)
    //        : base(operationtype, name, expression)
    //    {
    //        if (callback != null)
    //            name.EnsureArgumentNotNullOrEmpty("name", "Name cannot be null or empty when using output parameters");

    //        Callback = callback;
    //    }

    //    public Action<T> Callback { get; private set; }

    //    public override string Compile()
    //    {
    //        return base.Compile();
    //    }
    //}

    //internal class CallbackMapQueryPart<T> : NamedMapQueryPart
    //{
    //    public CallbackMapQueryPart(MapOperationType operationtype, LambdaExpression expression)
    //        : this(operationtype, null, expression, null)
    //    {
    //    }

    //    public CallbackMapQueryPart(MapOperationType operationtype, string name, LambdaExpression expression)
    //        : this(operationtype, name, expression, null)
    //    {
    //    }

    //    public CallbackMapQueryPart(MapOperationType operationtype, string name, LambdaExpression expression, Expression<Action<T>> callback)
    //        : base(operationtype, name, expression)
    //    {
    //        if (callback != null)
    //            name.EnsureArgumentNotNullOrEmpty("name", "Name cannot be null or empty when using output parameters");

    //        Callback = callback;
    //    }

    //    public Expression<Action<T>> Callback { get; private set; }

    //    public override string Compile()
    //    {
    //        return base.Compile();
    //    }
    //}
}
