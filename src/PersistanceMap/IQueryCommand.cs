
namespace PersistanceMap
{
    public interface IQueryCommand
    {
        void Execute(IDatabaseContext context);
    }
}
