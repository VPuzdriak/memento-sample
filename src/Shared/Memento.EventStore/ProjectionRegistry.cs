namespace Memento.EventStore;

public sealed class ProjectionRegistry
{
    private readonly Dictionary<Type, ProjectionSpecs> _projections = [];

    public void AddProjectionSpecs(Type type, ProjectionSpecs specs)
    {
        _projections.Add(type, specs);
    }

    public ProjectionSpecs? GetProjectionSpecs(Type type) => _projections.GetValueOrDefault(type);
    public ProjectionSpecs? GetProjectionSpecs<T>() => GetProjectionSpecs(typeof(T));
}