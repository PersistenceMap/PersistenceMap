using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder
{
    internal class ParameterQueryPart : IParameterQueryPart, ICallbackHandlerQueryPart, INamedQueryPart, IExpressionQueryPart, IQueryPart
    {
        public ParameterQueryPart(IEnumerable<IMapQueryPart> mapOperations)
            : this(null, mapOperations)
        {
        }

        public ParameterQueryPart(string name, IEnumerable<IMapQueryPart> mapOperations)
        {
            // ensure parameter is not null
            mapOperations.EnsureArgumentNotNull("mapOperations");

            Operations = mapOperations.ToList();
            Name = name;
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

        #region INamedQueryPart Implementation

        public string Name { get; private set; }

        #endregion
        
        #region ICallbackHandlerQueryPart  Implementation

        public virtual bool CanHandleCallback
        {
            get
            {
                return false;
            }
        }

        public virtual void HandleCallback(IDataReader reader)
        {
        }

        #endregion

        public virtual string Compile()
        {
            throw new NotImplementedException();
        }
    }

    internal class CallbackParameterQueryPart<T> : ParameterQueryPart, ICallbackQueryPart<T>, IParameterQueryPart, ICallbackHandlerQueryPart, INamedQueryPart, IExpressionQueryPart, IQueryPart
    {
        public CallbackParameterQueryPart(IEnumerable<IMapQueryPart> mapOperations)
            : this(null, mapOperations, null)
        {
        }

        public CallbackParameterQueryPart(string name, IEnumerable<IMapQueryPart> mapOperations)
            : this(name, mapOperations, null)
        {
        }

        public CallbackParameterQueryPart(string name, IEnumerable<IMapQueryPart> mapOperations, Action<T> callback)
            : base(name, mapOperations)
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

        public override void HandleCallback(IDataReader reader)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override string Compile()
        {
            base.Compile();
            throw new NotImplementedException();
        }
    }
}
