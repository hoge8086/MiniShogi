using System;
using System.Collections.Generic;
using System.Text;

namespace Shogi.Business.Domain.Event
{
    // 参考:<https://github.com/ardalis/DomainEventsDemo/tree/master/DomainEventsDemo/UdiDomainEvents>
    // Uid Dahan提唱(あまり推奨はされてないが)
    // <https://docs.microsoft.com/ja-jp/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-events-design-implementation>

    public static class DomainEvents
    {
        public delegate void DomainEventHander<DomainEvent>(DomainEvent domainEvent) where DomainEvent : IDomainEvent;

        private static List<Delegate> handlers = new List<Delegate>();
        public static void Raise<DomainEvent>(DomainEvent domainEvent) where DomainEvent : IDomainEvent
        {
            handlers.ForEach(h =>
            {
                if (h is DomainEventHander<DomainEvent> handler)
                    handler(domainEvent);
            });
        }

        public static void AddHandler<DomainEvent>(DomainEventHander<DomainEvent> handler)  where DomainEvent  : IDomainEvent
        {
            if(handler != null)
                handlers.Add(handler);
        }
        public static void RemoveHandler<DomainEvent>(DomainEventHander<DomainEvent> handler)  where DomainEvent  : IDomainEvent
        {
            if(handler != null)
                handlers.Remove(handler);
        }
    }
}
