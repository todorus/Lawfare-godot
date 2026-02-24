using Godot;
using Lawfare.scripts.context;
using Lawfare.scripts.logic.initiative;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.initiative;

public readonly struct InitiativeDiff(IContext context, ISubject subject, int originalIndex, int updatedIndex, bool becameStaggered) : IDiff
{
    public ISubject Subject { get; } = subject;
    public int OriginalIndex { get; } = originalIndex;
    public int UpdatedIndex { get; } = updatedIndex;
    public bool BecameStaggered { get; } = becameStaggered;

    public bool CanMerge(IDiff other) => other is InitiativeDiff o && ReferenceEquals(Subject, o.Subject);

    public IDiff Merge(IDiff other)
    {
        var o = (InitiativeDiff)other;
        return new InitiativeDiff(context, Subject, OriginalIndex, o.UpdatedIndex, o.BecameStaggered);
    }

    public IDiff Apply()
    {
        var state = context.InitiativeTrack;
        var occupant = Subject as IHasInitiative;

        // Clear original slot only if it still holds this subject
        if (OriginalIndex < state.Slots.Length &&
            ReferenceEquals(state.Slots[OriginalIndex].Occupant, occupant))
        {
            Initiative.SetSlot(state, OriginalIndex, null, false);
        }

        Initiative.SetSlot(state, UpdatedIndex, occupant, becameStaggered);

        return this;
    }
}