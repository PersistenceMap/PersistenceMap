using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using PersistanceMap.Compiler;
using PersistanceMap.Sql;

namespace PersistanceMap.QueryBuilder
{
    internal class ParameterQueryPart : IParameterQueryPart, ICallbackHandlerQueryPart, /*INamedQueryPart,*/ IExpressionQueryPart, IQueryPart
    {
        public ParameterQueryPart(IEnumerable<IMapQueryPart> mapOperations)
        {
            // ensure parameter is not null
            mapOperations.EnsureArgumentNotNull("mapOperations");

            Operations = mapOperations.ToList();
        }

        #region IExpressionQueryPart Implementation

        IEnumerable<IMapQueryPart> IExpressionQueryPart.Operations
        {
            get
            {
                return Operations;
            }
        }

        public IList<IMapQueryPart> Operations { get; private set; }

        #endregion

        //#region INamedQueryPart Implementation

        //public string Name { get; private set; }

        //#endregion

        #region ICallbackHandlerQueryPart  Implementation

        public string CallbackParameterName { get; protected set; }

        public virtual bool CanHandleCallback
        {
            get
            {
                return false;
            }
        }

        public virtual string CompileOutParameter(int index)
        {
            return string.Empty;
        }

        public virtual void HandleCallback(IDataReader reader)
        {
        }

        #endregion

        public virtual string Compile()
        {
            var valuePredicate = Operations.FirstOrDefault(o => o.MapOperationType == MapOperationType.Value);
            if (valuePredicate != null)
                return valuePredicate.Compile();

            return string.Empty;
        }
    }

    internal class CallbackParameterQueryPart<T> : ParameterQueryPart, ICallbackQueryPart<T>, IParameterQueryPart, ICallbackHandlerQueryPart, /*INamedQueryPart,*/ IExpressionQueryPart, IQueryPart
    {
        public CallbackParameterQueryPart(IEnumerable<IMapQueryPart> mapOperations)
            : this(mapOperations, null)
        {
        }

        //public CallbackParameterQueryPart(string name, IEnumerable<IMapQueryPart> mapOperations)
        //    : this(name, mapOperations, null)
        //{
        //}

        public CallbackParameterQueryPart(IEnumerable<IMapQueryPart> mapOperations, Action<T> callback)
            : base(mapOperations)
        {
            Callback = callback;
        }

        #region ICallbackQueryPart<T> Implementation

        public Action<T> Callback { get; set; }

        #endregion

        #region ICallbackHandlerQueryPart  Implementation

        public override bool CanHandleCallback
        {
            get
            {
                return Callback != null;
            }
        }

        public override string CompileOutParameter(int index)
        {
            if (!CanHandleCallback)
                return string.Empty;

            /* *Using Output compiles to*                
            declare @p1 datetime
            set @p1='2012-01-01 00:00:00'
            exec SalesByYear @Beginning_Date=@p1 output,@Ending_Date='2014-07-15 00:00:00'
            select @p1                
            */

            //
            // declare @p1 datetime
            // set @p1='2012-01-01 00:00:00'
            //

            CallbackParameterName = string.Format("@p{0}", index);

            var valuePredicate = Operations.FirstOrDefault(o => o.MapOperationType == MapOperationType.Value);

            var value = LambdaExpressionToSqlCompiler.Instance.Compile(valuePredicate/*Expression*/);
            if (value != null)
                return DialectProvider.Instance.GetQuotedValue(value, value.GetType());

            var sb = new StringBuilder();
            sb.AppendLine(string.Format("declare {0} {1}", CallbackParameterName, typeof(T).ToSqlDbType()));
            sb.AppendLine(string.Format("set {0}={1}", CallbackParameterName, value ?? base.Compile()));

            return sb.ToString();
        }

        public override void HandleCallback(IDataReader reader)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override string Compile()
        {
            if (CanHandleCallback && !string.IsNullOrEmpty(CallbackParameterName))
            {
                // create output parameter

                /* *Using Output compiles to*                
                declare @p1 datetime
                set @p1='2012-01-01 00:00:00'
                exec procedure @param1=@p1 output,@param2='2014-07-15 00:00:00'
                select @p1                
                */

                //
                // @param1=@p1 output
                //

                string name = "";

                var valuePredicate = Operations.FirstOrDefault(o => o.MapOperationType == MapOperationType.Value) as INamedQueryPart;
                if (valuePredicate != null)
                    name = valuePredicate.Name;

                return string.Format("{0}={1} output", name, CallbackParameterName);
            }

            // return default
            // @parametername=value
            // exec procedure @parm1=value,@param2=value
            return base.Compile();
        }
    }
}
