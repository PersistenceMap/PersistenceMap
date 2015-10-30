using System.Collections.Generic;

namespace PersistanceMap.QueryParts
{
    public class ProcedureQueryPartsContainer : QueryPartsContainer, IQueryPartsContainer
    {
        #region Properties

        private IList<CallbackMap> _callbacks;

        internal IEnumerable<CallbackMap> Callbacks
        {
            get
            {
                if (_callbacks == null)
                {
                    _callbacks = new List<CallbackMap>();
                }

                return _callbacks;
            }
        }

        internal void Add(CallbackMap callback)
        {
            if (_callbacks == null)
            {
                _callbacks = new List<CallbackMap>();
            }

            _callbacks.Add(callback);
        }

        #endregion
    }
}
