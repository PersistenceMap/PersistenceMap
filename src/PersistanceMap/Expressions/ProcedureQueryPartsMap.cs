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

        IList<IExpressionQueryPart> _parameters;
        public IList<IExpressionQueryPart> Parameters
        {
            get
            {
                if (_parameters == null)
                    _parameters = new List<IExpressionQueryPart>();
                return _parameters;
            }
        }

        public string ProcedureName { get; private set; }

        #endregion

        #region Add Methods

        internal void Add(ParameterQueryPart part)
        {
            Parameters.Add(part);
        }

        #endregion

        #region IQueryPartsMap Implementation


        #endregion
    }
}
