using Lawfare.scripts.logic.initiative;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.initiative;

public class HasActedDiff(ISubject subject, bool newValue) : IDiff
{
    public ISubject Subject { get; } = subject;
    
    private bool _oldValue = subject is IHasInitiative { HasActed: true };
    private bool _newValue = newValue;

    public bool CanMerge(IDiff other) => false;

    public IDiff Merge(IDiff other)
    {
        throw new System.NotImplementedException();
    }

    public IDiff Apply()
    {
        if (subject is IHasInitiative initiativeSubject)
        {
            initiativeSubject.HasActed = _newValue;
        }
        return this;
    }
}