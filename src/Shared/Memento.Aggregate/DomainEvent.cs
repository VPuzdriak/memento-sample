namespace Memento.Aggregate;

public abstract record DomainEvent(Guid Id, DateTime OccuredAtUtc);