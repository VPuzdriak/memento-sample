namespace Memento.EventStore;

public abstract record ProjectionSpecs(string Name, string? Version);

public sealed record ProjectionSpecs<T>(string Name, string? Version) : ProjectionSpecs(Name, Version);