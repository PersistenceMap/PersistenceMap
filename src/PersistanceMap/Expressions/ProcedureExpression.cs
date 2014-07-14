using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.Expressions
{
    public class ProcedureExpressionBase
    {
        public ProcedureExpressionBase(IDbContext context, string procName)
        {
            context.EnsureArgumentNotNull("context");
            procName.EnsureArgumentNotNullOrEmpty("procName");

            _context = context;
            ProcedureName = procName;
        }

        public string ProcedureName { get; private set; }

        IDbContext _context;
        protected IDbContext Context
        {
            get
            {
                return _context;
            }
        }
    }

    public class ProcedureExpression : ProcedureExpressionBase, IProcedureExpression
    {
        public ProcedureExpression(IDbContext context, string procName)
            : base(context, procName)
        {
        }

        public IProcedureExpression AddParameter()
        {
            throw new NotImplementedException();
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }

    public class ProcedureExpression<T> : ProcedureExpressionBase, IProcedureExpression<T>
    {
        public ProcedureExpression(IDbContext context, string procName)
            : base(context, procName)
        {
        }

        public IProcedureExpression<T> AddParameter()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Execute()
        {
            throw new NotImplementedException();
        }
    }
}
