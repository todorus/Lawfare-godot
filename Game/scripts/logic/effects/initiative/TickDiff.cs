using Lawfare.scripts.context;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.initiative;

public readonly struct TickDiff(IContext context, int oldIndex, int newIndex) : IDiff
{
    public ISubject Subject => null;
    
    private readonly int _oldIndex = oldIndex;
    private readonly int _newIndex = newIndex;

    public bool CanMerge(IDiff other) => false;

    public IDiff Merge(IDiff other) => throw new System.NotImplementedException();

    public IDiff Apply()
    {
        context.InitiativeTrack.CurrentIndex = newIndex;
        return this;
    }
}

