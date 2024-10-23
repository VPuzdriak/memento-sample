using System.Reflection;

namespace EShop.Orders.Domain.Shared;

public abstract class AggregateRoot
{
    private static readonly Dictionary<Type, ConstructorInfo> Constructors = [];

    public Guid Id { get; protected set; }
    private readonly List<DomainEvent> _events = [];

    public IEnumerable<DomainEvent> GetEvents() => _events;
    public void ClearEvents() => _events.Clear();

    public static T Load<T>(IReadOnlyList<DomainEvent> events) where T : AggregateRoot
    {
        var aggregateCtor = GetPrivateEmptyConstructor<T>();

        T emptyAggregate = (T)aggregateCtor!.Invoke(null);
        var aggregate = LoadFromSnapshot(emptyAggregate, events);

        return aggregate;
    }

    public static T LoadFromSnapshot<T>(T snapshot, IReadOnlyList<DomainEvent> events) where T : AggregateRoot
    {
        foreach (var @event in events)
        {
            snapshot.Fold(@event);
        }

        return snapshot;
    }

    protected void Raise<T>(T @event) where T : DomainEvent
    {
        Fold(@event);
        _events.Add(@event);
    }

    protected abstract void Fold(DomainEvent @event);

    private static ConstructorInfo GetPrivateEmptyConstructor<T>() where T : AggregateRoot
    {
        if (Constructors.TryGetValue(typeof(T), out var ctor))
        {
            return ctor;
        }

        ctor = typeof(T).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            Type.EmptyTypes,
            null);

        Constructors[typeof(T)] = ctor ?? throw new InvalidOperationException("Aggregate root must have a private parameterless constructor");

        return ctor;
    }
}