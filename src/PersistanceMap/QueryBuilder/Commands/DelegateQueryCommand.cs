using System;

namespace PersistanceMap.QueryBuilder.Commands
{
    /// <summary>
    /// Represents a QueryCommand that containes a expression that will be executed on Commit
    /// </summary>
    public class DelegateQueryCommand : IQueryCommand
    {
        readonly Action _predicate;

        public DelegateQueryCommand(Action predicate)
        {
            _predicate = predicate;
        }

        public void Execute(IDatabaseContext context)
        {
            _predicate.Invoke();
        }
    }
}
