using System.Reflection;

using Memento.Aggregate;

namespace Memento.EventStore;

public abstract class ReadModel
{
    private static readonly Dictionary<Type, ConstructorInfo> Constructors = [];

    public Guid Id { get; protected set; }

    public static TModel Load<TModel>(IReadOnlyList<DomainEvent> events) where TModel : ReadModel
    {
        var readModelCtor = GetPrivateEmptyConstructor<TModel>();

        TModel emptyReadModel = (TModel)readModelCtor!.Invoke(null);
        var readModel = LoadFromSnapshot<TModel>(emptyReadModel, events);

        return readModel;
    }

    public static TModel LoadFromSnapshot<TModel>(TModel snapshot, IReadOnlyList<DomainEvent> events) where TModel : ReadModel
    {
        foreach (var @event in events)
        {
            // Apply trick to call protected method
            ((dynamic)snapshot).Fold(@event);
        }

        return (TModel)snapshot;
    }

    protected abstract void Fold(DomainEvent @event);


    private static ConstructorInfo GetPrivateEmptyConstructor<T>()
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

        Constructors[typeof(T)] = ctor ?? throw new InvalidOperationException("Read model must have a private parameterless constructor");

        return ctor;
    }
}