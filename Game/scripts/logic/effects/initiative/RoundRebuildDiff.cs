using System.Linq;
using Lawfare.scripts.context;
using Lawfare.scripts.logic.initiative.state;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.initiative;

public readonly struct RoundRebuildDiff(IContext context, InitiativeSlotState[] newSlots) : IDiff
{
    public ISubject Subject => null;

    public bool CanMerge(IDiff other) => false;

    public IDiff Merge(IDiff other) => throw new System.NotImplementedException();

    public IDiff Apply()
    {
        context.InitiativeTrack.Slots = newSlots.Select(s => s.Clone()).ToArray();
        context.InitiativeTrack.CurrentIndex = 0;
        return this;
    }
}

