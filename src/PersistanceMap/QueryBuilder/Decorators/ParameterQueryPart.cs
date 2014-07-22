using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using PersistanceMap.Compiler;
using PersistanceMap.Sql;

namespace PersistanceMap.QueryBuilder.Decorators
{
    internal class ParameterQueryPart : IParameterQueryPart, ICallbackHandlerQueryPart, IQueryMapCollection, IQueryPart
    {
        public ParameterQueryPart(IQueryMap[] mapCollection)
        {
            // ensure parameter is not null
            mapCollection.EnsureArgumentNotNull("mapCollection");

            MapCollection = mapCollection.ToList();
        }

        public MapOperationType MapOperationType { get; set; }

        #region IQueryMapCollection Implementation

        IEnumerable<IQueryMap> IQueryMapCollection.MapCollection
        {
            get
            {
                return MapCollection;
            }
        }

        public IList<IQueryMap> MapCollection { get; private set; }

        #endregion

        #region ICallbackHandlerQueryPart  Implementation

        public string CallbackParameterName { get; protected set; }

        public virtual Type CallbackParameterType 
        {
            get
            {
                return null;
            }
        }

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

        public virtual void TryHandleCallback(object value)
        {
        }

        #endregion

        public virtual string Compile()
        {
            var valuePart = MapCollection.FirstOrDefault(o => o.MapOperationType == MapOperationType.Value);
            if (valuePart != null)
            {
                // compile the part
                return valuePart.Compile();
            }

            return string.Empty;
        }
    }

    internal class CallbackParameterQueryPart<T> : ParameterQueryPart, ICallbackQueryPart<T>, IParameterQueryPart, ICallbackHandlerQueryPart, IQueryMapCollection, IQueryPart
    {
        public CallbackParameterQueryPart(IQueryMap[] mapCollection)
            : this(mapCollection, null)
        {
        }

        public CallbackParameterQueryPart(IQueryMap[] mapCollection, Action<T> callback)
            : base(mapCollection)
        {
            Callback = callback;
        }

        #region ICallbackQueryPart<T> Implementation

        public Action<T> Callback { get; set; }

        #endregion

        #region ICallbackHandlerQueryPart  Implementation

        public override Type CallbackParameterType
        {
            get
            {
                return typeof(T);
            }
        }

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

            CallbackParameterName = string.Format("p{0}", index);

            var valuePredicate = MapCollection.FirstOrDefault(o => o.MapOperationType == MapOperationType.Value);

            // get the return value of the expression
            var value = valuePredicate.Expression.Compile().DynamicInvoke();

            // set the value into the right format
            value = DialectProvider.Instance.GetQuotedValue(value, value.GetType());

            //
            // declare @p1 datetime
            // set @p1='2012-01-01 00:00:00'
            //

            var sb = new StringBuilder();
            sb.AppendLine(string.Format("declare @{0} {1}", CallbackParameterName, typeof(T).ToSqlDbType()));
            sb.AppendLine(string.Format("set @{0}={1}", CallbackParameterName, value ?? base.Compile()));

            return sb.ToString();
        }

        public override void TryHandleCallback(object value)
        {
            try
            {
                Callback((T)value);
            }
            catch (Exception e)
            {
                Trace.WriteLine(String.Format("Value could not be set for callback. Value type: {0} Expected type: {1}", value != null ? value.GetType().Name : "null", typeof (T).Name));
                Trace.WriteLine(e.Message);
            }
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

                var valuePredicate = MapCollection.FirstOrDefault(o => o.MapOperationType == MapOperationType.Value) as INamedQueryPart;
                if (valuePredicate != null)
                    name = valuePredicate.Name;

                if (string.IsNullOrEmpty(name))
                {
                    //throw new NotSupportedException("The Parametername has to be provided when using Output Parameters");
                    Trace.WriteLine("The Parametername has to be provided when using Output Parameters");
                    return base.Compile();
                }

                return string.Format("{0}=@{1} output", name, CallbackParameterName);
            }

            // return default
            // @parametername=value
            return base.Compile();
        }
    }
}
