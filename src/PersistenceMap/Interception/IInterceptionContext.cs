namespace PersistenceMap.Interception
{
    public interface IInterceptionContext<T> : IInterceptionBuilder<T>
    {
        InterceptorCollection Interceptors { get; }
    }
}
