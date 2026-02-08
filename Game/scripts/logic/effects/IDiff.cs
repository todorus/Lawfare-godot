using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects;

public interface IDiff<T> : IDiff
{
    public T Original { get; }
    public T Updated { get; }
    
    public bool CanMerge(IDiff<T> other);
    public IDiff<T> Merge(IDiff<T> other);
}

public interface IDiff
{
    public ISubject Subject { get; }
    public bool CanMerge(IDiff other);
    public IDiff Merge(IDiff other);

    public IDiff Apply();
}