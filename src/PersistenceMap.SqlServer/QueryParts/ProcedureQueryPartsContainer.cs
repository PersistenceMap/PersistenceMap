using System.Collections.Generic;

namespace PersistenceMap.QueryParts
{
    public class ProcedureQueryPartsContainer : QueryPartsContainer, IQueryPartsContainer
    {
        #region Properties

        private IList<AfterMapCallbackPart> _callbacks;

        internal IEnumerable<AfterMapCallbackPart> Callbacks
        {
            get
            {
                if (_callbacks == null)
                {
                    _callbacks = new List<AfterMapCallbackPart>();
                }

                return _callbacks;
            }
        }

        internal void Add(AfterMapCallbackPart callback)
        {
            if (_callbacks == null)
            {
                _callbacks = new List<AfterMapCallbackPart>();
            }

            _callbacks.Add(callback);
        }

        #endregion
    }
}
