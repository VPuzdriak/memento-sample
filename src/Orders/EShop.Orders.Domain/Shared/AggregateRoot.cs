using System.Reflection;

namespace EShop.Orders.Domain.Shared;

public abstract class AggregateRoot
{
    public Guid Id { get; protected set; }
    public int Version { get; protected set; }
    protected List<DomainEvent> Events { get; } = [];

    public IEnumerable<DomainEvent> GetEvents() => Events;
    public void ClearEvents() => Events.Clear();

    public static T? Load<T>(IReadOnlyList<DomainEvent> events) where T : AggregateRoot
    {
        if (events.Count == 0)
        {
            return null;
        }

        var aggregateCtor = typeof(T).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            Type.EmptyTypes,
            null);

        T aggregate = (T)aggregateCtor!.Invoke(null);

        foreach (var @event in events)
        {
            aggregate.Fold(@event);
        }

        return aggregate;
    }

    protected void Raise<T>(T @event) where T : DomainEvent
    {
        Fold(@event);
        Events.Add(@event);
    }

    protected virtual void Fold(DomainEvent @event)
    {
        Version++;
    }
}