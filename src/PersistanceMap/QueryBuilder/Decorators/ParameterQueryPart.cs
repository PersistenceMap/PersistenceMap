﻿using System;
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

        /// <summary>
        /// Gets the name of the output parameter
        /// </summary>
        public string CallbackName { get; set; }

        /// <summary>
        /// Gets the type of the output parameter
        /// </summary>
        public virtual Type CallbackType
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets if the instance containes a registered callback
        /// </summary>
        public virtual bool CanHandleCallback
        {
            get
            {
                return false;
            }
        }
        
        /// <summary>
        /// Try to handle the callback
        /// </summary>
        /// <param name="value"></param>
        public virtual bool TryHandleCallback(object value)
        {
            return false;
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

        /// <summary>
        /// Gets the type of the output parameter
        /// </summary>
        public override Type CallbackType
        {
            get
            {
                return typeof(T);
            }
        }

        /// <summary>
        /// Gets if the instance containes a registered callback
        /// </summary>
        public override bool CanHandleCallback
        {
            get
            {
                return Callback != null;
            }
        }

        /// <summary>
        /// Try to handle the callback
        /// </summary>
        /// <param name="value"></param>
        public override bool TryHandleCallback(object value)
        {
            if (!CanHandleCallback)
                return false;

            try
            {
                Callback((T)value);
            }
            catch (Exception e)
            {
                Trace.WriteLine(String.Format("Value could not be set for callback. Value type: {0} Expected type: {1}", value != null ? value.GetType().Name : "null", typeof (T).Name));
                Trace.WriteLine(e.Message);

                return false;
            }

            return true;
        }

        #endregion

        public override string Compile()
        {
            if (CanHandleCallback && !string.IsNullOrEmpty(CallbackName))
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

                return string.Format("{0}=@{1} output", name, CallbackName);
            }

            // return default
            // @parametername=value
            return base.Compile();
        }
    }
}