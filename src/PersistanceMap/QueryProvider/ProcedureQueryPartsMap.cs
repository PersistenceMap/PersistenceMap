using System.Linq;
using System.Text;
using PersistanceMap.Compiler;
using PersistanceMap.QueryBuilder;
using System.Collections.Generic;

namespace PersistanceMap
{
    public class ProcedureQueryPartsMap : IQueryPartsMap
    {
        public ProcedureQueryPartsMap(string procedure)
        {
            procedure.EnsureArgumentNotNullOrEmpty("procedure");

            ProcedureName = procedure;
        }

        #region Properties

        IList<IParameterQueryPart> _parameters;
        internal IList<IParameterQueryPart> Parameters
        {
            get
            {
                if (_parameters == null)
                    _parameters = new List<IParameterQueryPart>();
                return _parameters;
            }
        }

        public string ProcedureName { get; private set; }

        #endregion

        #region Add Methods

        internal void Add(IParameterQueryPart part)
        {
            Parameters.Add(part);
        }

        #endregion

        #region IQueryPartsMap Implementation


        #endregion
    }
}
