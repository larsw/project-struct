namespace Cedar.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class ConventionEventRouter : IEventRouter
    {
        private readonly IDictionary<Type, Action<IAggregate, object>> _handlers = new Dictionary<Type, Action<IAggregate, object>>();
        private readonly bool _throwOnApplyNotFound;
        //private IAggregate _registered;

        public ConventionEventRouter(bool throwOnApplyNotFound = false)
        {
            _throwOnApplyNotFound = throwOnApplyNotFound;
        }

        public virtual void Register<T>(Action<IAggregate, T> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            Register(typeof(T), (aggregate, @event) => handler(aggregate, (T)@event));
        }

        public void RegisterType(Type aggregateType)
        {
            if (aggregateType == null)
            {
                throw new ArgumentNullException(nameof(aggregateType));
            }

            //_registered = aggregate;

            var applyMethods = aggregateType
                .GetRuntimeMethods()
                .Where(m => !m.IsStatic
                    && m.Name == "Apply"
                    && m.GetParameters().Length == 1
                    && m.ReturnType == typeof(void))
                .Select(m => new { Method = m, MessageType = m.GetParameters().Single().ParameterType });

            foreach (var apply in applyMethods)
            {
                MethodInfo applyMethod = apply.Method;
                _handlers.Add(apply.MessageType, (aggregate, m) => applyMethod.Invoke(aggregate, new[] { m }));
            }
        }

        public virtual void Dispatch(IAggregate aggregate, object eventMessage)
        {
            if (eventMessage == null)
            {
                throw new ArgumentNullException("eventMessage");
            }

            if (_handlers.TryGetValue(eventMessage.GetType(), out var handler))
            {
                handler(aggregate, eventMessage);
            }
            else if (_throwOnApplyNotFound)
            {
                aggregate.ThrowHandlerNotFound(eventMessage);
            }
        }

        private void Register(Type messageType, Action<IAggregate, object> handler)
        {
            _handlers[messageType] = handler;
        }
    }
}
