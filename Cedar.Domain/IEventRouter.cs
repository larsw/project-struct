namespace Cedar.Domain
{
    using System;

    public interface IEventRouter
    {
        void Register<T>(Action<IAggregate, T> handler);

        void RegisterType(Type aggregateType);

        void Dispatch(IAggregate aggregate, object eventMessage);
    }
}
