
namespace PersistanceMap
{
    /// <summary>
    /// QueryCommands contain a QueryExpression that will be stored on the DatabaseContext. 
    /// All QueryCommands that are contained on a DatabaseContext get executed, in the stored order, when the Commit Method is called on the DatabaseContext.
    /// </summary>
    public interface IQueryCommand
    {
        void Execute(IDatabaseContext context);
    }
}
