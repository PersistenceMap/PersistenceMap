
namespace PersistenceMap
{
    /// <summary>
    /// QueryCommands contain a QueryExpression that will be stored on the DatabaseContext. 
    /// All QueryCommands that are contained on a DatabaseContext get executed, in the stored order, when the Commit Method is called on the DatabaseContext.
    /// </summary>
    public interface IQueryCommand
    {
        /// <summary>
        /// Execute the query contained in the command
        /// </summary>
        /// <param name="context"></param>
        void Execute(IDatabaseContext context);
    }
}
